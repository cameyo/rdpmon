using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using LiteDB;
using System.Management;
using System.Net.Sockets;
using System.Security.Cryptography;
using Microsoft.Win32;
using FileMode = System.IO.FileMode;

namespace Cameyo.RdpMon
{
    // ConnectMon
    public class ConnectMon
    {
        public const int FailureEvtId = 4625;
        public const int SuccessEvtId = 4648;
        public bool dbg;
        int iteration = 0;
        Addrs addrs;
        private bool updateDb;
        private bool updateFw;

        public ConnectMon(/*DateTime _fromDate*/bool _updateDb, bool _updateFw)
        {
            //fromDate = _fromDate;
            updateDb = _updateDb;
            updateFw = _updateFw;
            //dbg = File.Exists("c:\\HaxLogs.txt") || File.Exists("c:\\ruby_doc.ico");
            addrs = new Addrs(/*_fromDate*/_updateDb, _updateFw);
        }

        public Addrs Aggregate(DateTime _fromUtc)
        {
            var logprefix = "Scan: ";
            Log(logprefix + "in");
            var timer = DateTime.UtcNow;
            try
            {
                var provider = "Security";
                using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
                {
                    var _lastDbModif = DbProps.Get(db, "LastAddrChange");
                    var lastDbModif = (_lastDbModif != null ? (DateTime)_lastDbModif : DateTime.MinValue);
                    var fromUtc = (_fromUtc == DateTime.MinValue ? lastDbModif : _fromUtc);
                    //var fromLocal = fromUtc.ToLocalTime();
                    var query = "*[" +
                                "(System/EventID=" + SuccessEvtId.ToString() + " or " + "System/EventID=" + FailureEvtId.ToString() + ")" +
                                " and " +
                                "System[TimeCreated[@SystemTime>'" + fromUtc.ToString("yyyy-MM-dd") + "T" + fromUtc.ToString("HH:mm:ss") + ".000000000Z" + "']]" +
                                "]";

                    // Skip if DB hasn't changed since fromUtc
                    if (lastDbModif != null && lastDbModif < fromUtc)
                    {
                        Log(logprefix + "out: DB unchanged, skipping");
                        iteration++;
                        return addrs;
                    }
                    
                    var addrTable = db.GetCollection<Addr>("Addr");
                    if (updateDb)
                    {
                        var eventsQuery = new EventLogQuery(provider, PathType.LogName, query);
                        if (dbg)
                        {
                            var rand = new Random(Environment.TickCount);
                            var dbgRand = true;
                            for (int i = 0; i < 200; i++)
                            {
                                var success = ((dbgRand ? rand.Next(200) : i) > 400);
                                var ip = string.Format("{0}.{1}.{2}.{3}", 132, 154, 255, (dbgRand ? rand.Next(50) : i) + 1);
                                var now = DateTime.UtcNow;
                                var utcTime = dbgRand ? now.Subtract(TimeSpan.FromMinutes(rand.Next(60)))
                                                      : new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                                var userName = "User" + (dbgRand ? rand.Next(9) : i) + 1;
                                if (utcTime > fromUtc)
                                    addrs.Aggregate(addrTable, ip, utcTime, success, userName);
                            }
                        }
                        else
                        {
                            var logReader = new EventLogReader(eventsQuery);
                            for (var evt = logReader.ReadEvent(); evt != null; evt = logReader.ReadEvent())
                                addrs.Aggregate(addrTable, evt, dbg);
                        }
                        if (addrs.lastDbChange != null)
                            DbProps.Set(db, "LastAddrChange", DateTime.UtcNow);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
            }

            var perfDuration = DateTime.UtcNow.Subtract(timer);
            Log(logprefix + "out: " + addrs.Items.Count.ToString() + " addrs found, took " + perfDuration.TotalSeconds + " seconds");
            iteration++;
            return addrs;
        }

        static void Log(string msg) { Utils.Log(msg); }
    }
    
    //
    // Addr
    public class Addr
    {
        public string AddrId { get; set; }
        public long Type { get; set; }
        public long Flags { get; set; }
        public long Prot { get; set; }
        public int FailCount { get; set; }
        public int SuccessCount { get; set; }
        public DateTime First { get; set; }
        public DateTime Last { get; set; }
        public HashSet<string> UserNames { get; set; }
        //public int EvtId { get; set; }

        [Flags] public enum ProtFlag { None = 0, Blocked = 1 };
        
        public Addr()
        {
            FailCount = SuccessCount = 0;
            First = DateTime.MaxValue;
            Last = DateTime.MinValue;
            UserNames = new HashSet<string>();
        }
        
        public bool IsOngoing() { return (DateTime.UtcNow.Subtract(this.Last).TotalMinutes < 2); }

        public bool IsAttack(int offset = 0) { return (FailCount >= 10 + offset); }

        public bool IsLegit() { return (SuccessCount > 0 && FailCount < 100); }
    }

    // DbProps
    public class DbProps
    {
        public class Prop
        {
            public string PropId { get; set; }
            public object Val { get; set; }
        }

        public static void Set(LiteDatabase db, string name, object val)
        {
            var table = db.GetCollection<Prop>("Prop");
            table.Upsert(new Prop {PropId = name, Val = val});
        }
        
        public static object Get(LiteDatabase db, string name)
        {
            var table = db.GetCollection<Prop>("Prop");
            var item = table.FindById(name);
            if (item != null)
                return item.Val;
            else
                return null;
        }
    }
        
    // Addrs
    public class Addrs
    {
        public Dictionary<string, Addr> Items;
        private DateTime mostRecentAddr;
        public DateTime? lastDbChange = null;
        private bool updateDb;
        private bool updateFw;

        public Addrs(bool _updateDb, bool _updateFw)
        {
            updateDb = _updateDb;
            updateFw = _updateFw;
            Items = new Dictionary<string, Addr>();
            using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
            {
                var addrTable = db.GetCollection<Addr>("Addr");
                var storedAddrs = addrTable.FindAll().ToArray();
                mostRecentAddr = DateTime.MinValue;
                for (int i = 0; i < storedAddrs.Length; i++)
                {
                    var item = storedAddrs[i];
                    AggregateMulti(addrTable, item.AddrId, item.SuccessCount, item.FailCount, item.First, item.Last, item.UserNames);
                    if (item.Last > mostRecentAddr)
                        mostRecentAddr = item.Last;
                }
            }
        }

        public void Aggregate(LiteCollection<Addr> addrTable, EventRecord evt, bool dbg)
        {
            var evtid = evt.Id;
            int ipIndex, userIndex;
            bool success;
            if (evtid == 4625)
            {
                ipIndex = 19;
                userIndex = 5;
                success = false;
            }
            else if (evtid == 4648)
            {
                ipIndex = 12;
                userIndex = 5;
                success = true;
            }
            else // Should never happen
            {
                ipIndex = 1;
                userIndex = 5;
                success = false;
            }

            if (evt.Properties.Count <= ipIndex || evt.Properties[ipIndex].Value.GetType() != typeof(string)) // Shouldn't happen as far as I know //19
                return;
            if (evt.TimeCreated == null) // Shouldn't happen
                return;
            var ip = (string)evt.Properties[ipIndex].Value;
            if (ip == "::1" || ip == "127.0.0.1" || ip == "-" || ip == "0.0.0.0")
                return;
            var userName = (string)evt.Properties[userIndex].Value;
            var localTime = (DateTime)evt.TimeCreated;
            Aggregate(addrTable, ip, localTime.ToUniversalTime(), success, userName);
        }

        public void Aggregate(LiteCollection<Addr> addrTable, string ip,
            DateTime utcTime, bool success, string userName)
        {
            ///if (evttime <= LastStored) // Already added when DB was loaded
            ///    return;
            AggregateMulti(addrTable, ip, success ? 1 : 0, success ? 0 : 1, utcTime, utcTime, new [] { userName });
        }

        private void AggregateMulti(LiteCollection<Addr> addrTable, string ip, 
            int successCount, int failCount, DateTime first, DateTime last, IEnumerable<string> userNames)
        {
            var logprefix = "AggregateM(" + ip + "): ";
            var existing = Items.TryGetValue(ip, out var addr); 
            if (!existing)
            {
                addr = new Addr();
                addr.AddrId = ip;
                Items.Add(ip, addr);
            }
            addr.FailCount += failCount;
            addr.SuccessCount += successCount;
            if (first < addr.First)
                addr.First = first;
            if (last > addr.Last)
                addr.Last = last;
            foreach (var userName in userNames)
            {
                if (!string.IsNullOrEmpty(userName))
                    addr.UserNames.Add( userName);
            }

            if (updateDb)
            {
                var added = addrTable.Upsert(addr);
                if (added)
                    Log(logprefix + "adding new db entry");
                lastDbChange = DateTime.UtcNow;
                
                /* BlackList protection rule: TBD
                if (updateFw && addr.IsAttack(-1) && (addr.Prot & (long)Addr.ProtFlag.Blocked) == 0)
                {
                    Log(logprefix + "adding fw deny rule");
                    var fw = new WinFirewall();
                    if (fw.AddIp(false, ip, 3389, false, "RdpMon.BlackList (%PROTOCOL%)", 
                        "Brute-force addresses black-listed by RdpMon"))
                    {
                        addr.Prot |= (long)Addr.ProtFlag.Blocked;
                        addrTable.Update(addr);
                    }
                    else
                        Log(logprefix + "* failed adding fw deny rule!");
                }
                */
            }
        }

        public void Merge(LiteCollection<Addr> addrTable, Addrs newItems)
        {
            foreach (var newItem in newItems.Items)
            {
                var ip = newItem.Key;
                var addr = newItem.Value;
                AggregateMulti(addrTable, ip, addr.SuccessCount, addr.FailCount, addr.First, addr.Last, addr.UserNames);
            }
        }
                        
        public static void Log(string msg)
        {
            Utils.Log(msg);
        }
    }

    
    //
    // Windows Firewall
    public class WinFirewall
    {
        const string guidFWPolicy2 = "{E2B3C97F-6AE1-41AC-817A-F6F92166D7DD}";
        const string guidRWRule = "{2C5BC43E-3369-4C33-AB0C-BE9469677AF4}";
        [Flags] public enum FwProfile { None = 0, Domain = 1, Private = 2, Public = 5 };
        [Flags] public enum FwAction { Block = 0, Allow =1 };
        public enum FwProtocol { None, Tcp, Udp, Any };

        Type typeFWRule;
        INetFwPolicy2 fwPolicy2;

        public WinFirewall()
        {
            var typeFWPolicy2 = Type.GetTypeFromCLSID(new Guid(guidFWPolicy2));
            typeFWRule = Type.GetTypeFromCLSID(new Guid(guidRWRule));
            fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(typeFWPolicy2);
        }
        
        public bool AddIp(bool clear, string ips, int port, bool allow, string _ruleName, string ruleDescription)
        {
            var ret = true;
            for (int i = 0; i < 2; i++)
            {
                var protocol = (i == 0 ? FwProtocol.Tcp : FwProtocol.Udp);
                var logprefix = $"AddIp({clear}, {ips}, {protocol.ToString().ToUpper()}, {allow}): ";
                var ruleName = _ruleName.Replace("%PROTOCOL%", protocol.ToString().ToUpper());
                string fqips = "";
                if (!string.IsNullOrEmpty(ips))
                {
                    foreach (var _ip in ips.Split(','))
                    {
                        var ip = _ip.Trim();
                        if (!string.IsNullOrEmpty(ips))
                            fqips += ",";
                        fqips += ip + "/255.255.255.255";   // Fully-qualified IP
                    }
                }

                // Try finding existing rule
                var ruleExists = false;
                INetFwRule oldRule = null;
                try
                {
                    oldRule = fwPolicy2.Rules.Item(ruleName);
                    ruleExists = true;
                }
                catch (FileNotFoundException) { ruleExists = false; }
                catch (Exception ex) { Log(logprefix + "* failed looking up for rule: " + ex.ToString()); }

                // Update existing rule
                if (ruleExists)
                {
                    try
                    {
                        var addrs = oldRule.RemoteAddresses;
                        if (clear)
                            oldRule.RemoteAddresses = fqips;
                        else
                        {
                            if (addrs.Contains(fqips)) // Since addresses are comma-separated, it's enough to search IP as string
                                Log(logprefix + "fw rule already includes ip");
                            else
                            {
                                Log(logprefix + "adding address into existing rule");
                                if (string.IsNullOrEmpty(addrs))
                                    oldRule.RemoteAddresses = fqips;
                                else
                                    oldRule.RemoteAddresses = addrs + "," + fqips;
                                Log(logprefix + "added address");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(logprefix + "* failed updating fw rule: " + ex.ToString());
                    }
                    continue;
                }

                // Create new fw rule (first time)
                try
                {
                    Log(logprefix + "creating new fw rule");
                    var newRule = (INetFwRule)Activator.CreateInstance(typeFWRule);
                    newRule.Name = ruleName;
                    newRule.Description = ruleDescription;
                    newRule.Protocol = (int)(protocol == FwProtocol.Udp ? NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP : NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
                    newRule.LocalPorts = port.ToString();
                    newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                    newRule.Enabled = true;
                    newRule.Grouping = "@firewallapi.dll,-23255";
                    //newRule.Profiles = fwPolicy2.CurrentProfileTypes;
                    newRule.Profiles = (int)(FwProfile.Domain | FwProfile.Private | FwProfile.Public);
                    newRule.Action = (allow ? NET_FW_ACTION_.NET_FW_ACTION_ALLOW : NET_FW_ACTION_.NET_FW_ACTION_BLOCK);
                    newRule.RemoteAddresses = fqips;
                    fwPolicy2.Rules.Add(newRule);
                }
                catch (Exception ex)
                {
                    Log(logprefix + "* failed adding new rule: " + ex.ToString());
                    ret = false;
                }
            }
            return ret;
        }

        public bool ModifyRule(string name, bool enabled, string remoteAddrs = null, string[] exceptIps = null)
        {
            var newRule = (INetFwRule)Activator.CreateInstance(typeFWRule);
            newRule.Name = name;
            INetFwRule existingRule = null;
            try
            {
                existingRule = fwPolicy2.Rules.Item(newRule.Name);
                existingRule.Enabled = enabled;

                // remoteAddrs: comma-separated IPs
                if (!string.IsNullOrEmpty(remoteAddrs))
                    existingRule.RemoteAddresses = remoteAddrs;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void AddRule(string name, int port, bool allow, FwProtocol protocol, string description, string remoteAddrs = null, bool enabled = true)
        {
            var newRule = (INetFwRule)Activator.CreateInstance(typeFWRule);
            newRule.Name = name;
            bool exists = false;
            try
            {
                var existingRule = fwPolicy2.Rules.Item(newRule.Name);
                exists = true;
            }
            catch { }
            if (exists)
                return;
            newRule.Description = description;
            newRule.Protocol = (int)(protocol == FwProtocol.Udp ? NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP : NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
            newRule.LocalPorts = port.ToString();
            newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            newRule.Enabled = enabled;
            newRule.Grouping = "@firewallapi.dll,-23255";
            //newRule.Profiles = fwPolicy2.CurrentProfileTypes;
            newRule.Profiles = (int)(FwProfile.Domain | FwProfile.Private | FwProfile.Public);
            newRule.Action = (allow ? NET_FW_ACTION_.NET_FW_ACTION_ALLOW : NET_FW_ACTION_.NET_FW_ACTION_BLOCK);
            fwPolicy2.Rules.Add(newRule);
        }

        public void DbgNow()
        {
            
        }

        public void GetRdpRules()
        {
            var dbgStr = "";
            var dbgSeparator = ": ";
            foreach (INetFwRule item in fwPolicy2.Rules)
            {
                if (item.Enabled &&
                    item.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN &&
                    item.LocalPorts == "3389")
                    //!string.IsNullOrEmpty(item.LocalPorts) && item.LocalPorts != "*")
                {
                    dbgStr += "[" + item.Name + "]" + dbgSeparator;
                    dbgStr += (FwAction)item.Action + dbgSeparator;

                    // Protocol
                    var protocol = FwProtocol.None;
                    if (item.Protocol == (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP)
                        protocol = FwProtocol.Udp;
                    else if (item.Protocol == (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP)
                        protocol = FwProtocol.Tcp;
                    else if (item.Protocol == (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY)
                        protocol = FwProtocol.Any;
                    dbgStr += item.LocalPorts + "/" + protocol.ToString() + dbgSeparator;

                    // Programs
                    if (!string.IsNullOrEmpty(item.ApplicationName))
                        dbgStr += "Apps=" + item.ApplicationName + dbgSeparator;

                    // Profiles
                    var profiles = FwProfile.None;
                    if ((item.Profiles & (int)FwProfile.Domain) != 0)
                        profiles |= FwProfile.Domain;
                    if ((item.Profiles & (int)FwProfile.Private) != 0)
                        profiles |= FwProfile.Private;
                    if ((item.Profiles & (int)FwProfile.Public) != 0)
                        profiles |= FwProfile.Public;
                    dbgStr += profiles + "\n";
                }
            }
            MessageBox.Show(dbgStr);

        }
                        
        public static void Log(string msg)
        {
            Utils.Log(msg);
        }
    }

    //
    // Utils
    public static class Utils
    {
        static public string MyPath()
        {
            return Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        static public string MyPath(string file)
        {
            var myDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            return PathCombine(myDir, file);
        }

        static public string MyExe()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        static public bool ExecProg(string fileName, string args, ref int exitCode, int timeout, bool elevate)
        {
            try
            {
                var procStartInfo = new ProcessStartInfo(fileName, args);
                var proc = new System.Diagnostics.Process();
                if (elevate)
                {
                    procStartInfo.Verb = "runas";
                    procStartInfo.UseShellExecute = true;
                }
                else
                    procStartInfo.UseShellExecute = (!Path.GetExtension(fileName).Equals(".exe", StringComparison.InvariantCultureIgnoreCase));
                proc.StartInfo = procStartInfo;
                proc.Start();
                if (timeout > 0)
                {
                    if (proc.WaitForExit(timeout))
                        exitCode = proc.ExitCode;   // Process has finished
                    else
                        exitCode = -99;             // Timeout has passed
                }
                return true;
            }
            catch { }
            return false;
        }


        public static bool SendServiceCmd(string svcName, int command)
        {
            try
            {
                var service = new System.ServiceProcess.ServiceController(svcName);
                service.ExecuteCommand(command);
                return true;
            }
            catch (Exception ex)
            {
                Log("* could not access the service controller: " + ex.ToString());
                return false;
            }
        }

                static public string StartService(string svcName) { return StartService(svcName, TimeSpan.MaxValue); }
        static public string StartService(string svcName, TimeSpan timeout)
        {
            var logprefix = "StartService(" + svcName + "): ";
            try
            {
                var sc = new System.ServiceProcess.ServiceController(svcName);
                if (sc.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                {
                    // Start the service if the current status is stopped.
                    Log("Starting service: " + svcName);
                    try
                    {
                        // Start the service, and wait until its status is "Running".
                        sc.Start();
                        sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, timeout);

                        // Display the current service status.
                        Log(logprefix + "service status is now set to " + sc.Status.ToString());
                    }
                    catch (System.ServiceProcess.TimeoutException)
                    {
                        Log(logprefix + "timeout reached (non-critical)");
                    }
                    catch (InvalidOperationException)
                    {
                        return "Could not start service";
                    }
                }
                sc.Close();
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
                return "Could not access the service controller";
            }
            return null;
        }

        static public string StopService(string svcName) { return StopService(svcName, TimeSpan.MaxValue); }
        static public string StopService(string svcName, TimeSpan timeout)
        {
            var logprefix = "StopService(" + svcName + "): ";
            try
            {
                var sc = new System.ServiceProcess.ServiceController(svcName);
                if (sc.Status != System.ServiceProcess.ServiceControllerStatus.Stopped)
                {
                    // Start the service if the current status is stopped.
                    Log(logprefix + "stopping");
                    try
                    {
                        // Start the service, and wait until its status is "Running".
                        sc.Stop();
                        sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, timeout);

                        // Display the current service status.
                        Log(logprefix + "service status is now set to " + sc.Status.ToString());
                    }
                    catch (System.ServiceProcess.TimeoutException)
                    {
                        Log(logprefix + "timeout reached (non-critical)");
                    }
                    catch (InvalidOperationException)
                    {
                        return "Could not stop service";
                    }
                }
                sc.Close();
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
                return "Could not access the service controller";
            }
            return null;
        }

        // CombinePath: a safer alternative to Path.Combine().
        // To my great surprise & sadness, Path.Combine("c:\\now", "\\anything") -> "\\anything"
        // In other words, whenever path2 starts with '\' or '/', it suppresses path1, which is a security issue!
        public static string PathCombine(string path1, string path2, string path3 = null)
        {
            // 1 param
            if (path2 == null)
                return path1;
            
            // 2+ params
            if (!string.IsNullOrEmpty(path1) && path1.EndsWith(":"))
                path1 += "\\";
            while (!string.IsNullOrEmpty(path2) && (path2.StartsWith("\\") || path2.StartsWith("/")))
                path2 = path2.Remove(0, 1);
            if (path3 == null)
                return System.IO.Path.Combine(path1, path2);
            
            // 3 params
            while (!string.IsNullOrEmpty(path3) && (path3.StartsWith("\\") || path3.StartsWith("/")))
                path3 = path3.Remove(0, 1);
            return System.IO.Path.Combine(path1, path2, path3);
        }

        public static bool IsElevated()
        {
            try
            {
                var regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                if (regKey != null)
                {
                    regKey.Close();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsSystemUser()
        {
            bool isSystem;
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                isSystem = identity.IsSystem;
            }
            return isSystem;
        }

        public static uint? RegReadDword(RegistryKey baseKey, string path, string item)
        {
            try
            {
                var key = baseKey.OpenSubKey(path, false);
                if (key == null)
                    return null;
                var val = (int)key.GetValue(item, null);
                key.Close();
                return (uint)val;
            }
            catch
            {
                return null;
            }
        }

        public static bool RegWriteDword(RegistryKey baseKey, string path, string item, uint value)
        {
            try
            {
                var key = baseKey.CreateSubKey(path);
                if (key == null)
                    return false;
                key.SetValue(item, (int)value, RegistryValueKind.DWord);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string RegReadStr(RegistryKey baseKey, string path, string item)
        {
            try
            {
                var key = baseKey.OpenSubKey(path, false);
                if (key == null)
                    return null;
                var val = (string)key.GetValue(item, null);
                key.Close();
                return (string)val;
            }
            catch
            {
                return null;
            }
        }

        public static bool RegWriteStr(RegistryKey baseKey, string path, string item, string value)
        {
            try
            {
                var key = baseKey.CreateSubKey(path);
                if (key == null)
                    return false;
                key.SetValue(item, (string)value, RegistryValueKind.String);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        static public string TimeAgo(DateTime now, DateTime dt)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(now.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE)
            {
                return "a minute ago";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE)
            {
                return "an hour ago";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR)
            {
                return "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        static public string DurationStr(DateTime dt)
        {
            return DurationStr(DateTime.Now.Subtract(dt));
        }

        static public string DurationStr(TimeSpan ts)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            //const int HOUR = 60 * MINUTE;
            //const int DAY = 24 * HOUR;
            //const int MONTH = 30 * DAY;

            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 0)
            {
                return "";
            }
            if (delta < 1 * MINUTE)
            {
                if (ts.Seconds <= 0)
                    return "";
                return ts.Seconds == 1 ? "one second" : ts.Seconds + " seconds";
            }
            else if (delta < 2 * MINUTE)
            {
                return "a minute";
            }
            else if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " minutes";
            }
            else if (delta < 90 * MINUTE)
            {
                return "an hour";
            }
            else //if (delta < 24 * HOUR)
            {
                if (ts.TotalHours == 1)
                    return ts.TotalHours + " hour";
                else
                    return Math.Truncate(ts.TotalHours) + " hours";
            }
            /*if (delta < 48 * HOUR)
            {
                return "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " days";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month" : months + " months";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year" : years + " years";
            }*/
        }

        public static long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern void OutputDebugString(string lpOutputString);

        public static void HardLog(string component, string msg)
        {
            var now = DateTime.UtcNow;
            var line = "[TID=" + Thread.CurrentThread.ManagedThreadId + "] " + component;
            if (Globals.IsSystem)
                line += " (System)";
            line += "> " + msg + "\r\n";
            OutputDebugString(line);
            line = now.ToString("MM/dd HH:mm:ss.fff") + " " + line;

            var logFile = Utils.MyPath("RdpMon.log");
            try
            {
                // Log rotation
                long length;
                try
                {
                    length = new System.IO.FileInfo(logFile).Length;
                }
                catch
                {
                    length = 0;   // Log file doesn't exist yet
                }

                var mb = 1024 * 1024;
                var limit = 150 * mb;
                if (length > limit)
                {
                    var mutexName = "RdpMon.HardLog." + component;
                    var mutex = new Mutex(true, mutexName);
                    mutex.WaitOne();
                    try
                    {
                        length = new System.IO.FileInfo(logFile).Length;
                        if (length > limit)
                        {
                            var i = 1;
                            while (File.Exists(Path.ChangeExtension(logFile, string.Format(".{0}.log", i))))
                                i++;
                            try
                            {
                                File.Move(logFile, Path.ChangeExtension(logFile, string.Format(".{0}.log", i)));
                            }
                            catch { }
                        }
                    }
                    catch { }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }

                // Write log event
                using (var sw = File.AppendText(logFile))
                {
                    sw.Write(line);
                }
            }
            catch
            {
                //System.Diagnostics.Debug.WriteLine("** Couldn't write to log file: " + logFile);
            }
            if (msg.Contains("*") && component != "Error")
                HardLog("Error", component + ": " + msg);
        }
 
        public static byte[] FileHashSHA256(string fileName)
        {
            try
            {
                var hash = new SHA256Managed();
                var fileInfo = new FileInfo(fileName);
                var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var ret = hash.ComputeHash(fileStream);
                fileStream.Close();
                return ret;
            }
            catch
            {
                return null;
            }
        }

        public static void Log(string msg)
        {
            Utils.HardLog("RdpMon", msg);
        }
    }

    // Globals
    public static class Globals
    {
        public static bool IsSystem = false;
    }
}
