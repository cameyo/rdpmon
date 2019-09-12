using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using LiteDB;
using FileMode = System.IO.FileMode;

namespace Cameyo.RdpMon
{
    public class ProcessMon
    {
        class ProcessInfo
        {
            public string FullFileName;
            public string ShortName;
            public uint WtsSessionId;
            public uint ParentPid;
        }
        
        ManagementEventWatcher procStopWatch, procStartWatch, moduleLoadWatch;
        string sysdir, windir, hdroot, usersdir, commonProfile, progData, progFilesX86, progFiles;
        DeviceNameResolver deviceNameResolver;
        Dictionary<uint, ProcessInfo> processInfos;
        object processInfosLock;
        
        public ProcessMon()
        {
            sysdir = Environment.SystemDirectory.ToUpperInvariant(); // C:\WINDOWS\SYSTEM32
            windir = Path.GetDirectoryName(sysdir).ToUpperInvariant(); // C:\WINDOWS
            hdroot = Path.GetPathRoot(Environment.SystemDirectory).ToUpperInvariant(); // C:\
            usersdir = Path.Combine(hdroot, "USERS").ToUpperInvariant();  // C:\USERS, for lack of a better option!
            commonProfile = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)).ToUpperInvariant();
            progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToUpperInvariant();
            progFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToUpperInvariant();
            progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToUpperInvariant();
            deviceNameResolver = new DeviceNameResolver();
            processInfos = new Dictionary<uint, ProcessInfo>();
            processInfosLock = new object();

            procStartWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            procStartWatch.EventArrived += OnProcessCreate;
            procStartWatch.Start();
            procStopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            procStopWatch.EventArrived += OnProcessProcStop;
            procStopWatch.Start();
            moduleLoadWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ModuleLoadTrace"));
            moduleLoadWatch.EventArrived += OnModuleLoad;
            moduleLoadWatch.Start();
        }

        ~ProcessMon()
        {
            moduleLoadWatch.Stop();
            procStartWatch.Stop();
            procStopWatch.Stop();
        }

        void OnNewProcessReady(uint pid, ProcessInfo processInfo)
        {
            var logprefix = "ProcessMon.OnNewProcessReady: ";
            try
            {
                var shortName = processInfo.ShortName;
                var wtsSessionId = processInfo.WtsSessionId;
                var parentPid = processInfo.ParentPid;
                logprefix = "ProcessMon.OnNewProcessReady(" + pid + ", " + shortName + ", #" + wtsSessionId + "): ";
                var normalizedPath = NormalizePath(processInfo.FullFileName);
                Log(logprefix + normalizedPath + ", parent=" + parentPid.ToString());
                using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
                {
                    var hash = Utils.FileHashSHA256(processInfo.FullFileName);
                    if (hash != null)
                    {
                        var wtsInfo = WTS.QuerySessionInfo((int)wtsSessionId);
                        long sessionUid = 0;
                        if (wtsInfo != null)
                            sessionUid = Session.GetSessionUid(wtsSessionId, wtsInfo.LogonTime);
                        else
                            Log(logprefix + "* failed finding session #" + wtsSessionId);
                        var table = db.GetCollection<Cameyo.RdpMon.Process>("Process");
                        var item = table.FindById(hash);
                        var execInfo = new ExecInfo
                        {
                            SessionUid = sessionUid,
                            Flags = 0,
                            Pid = pid,
                            ParentPid = parentPid,
                            Start = DateTime.UtcNow,
                        };
                        if (item != null)
                        {
                            var _list = item.ExecInfos.ToList();
                            _list.Add(execInfo);
                            item.ExecInfos = _list.ToArray();
                            table.Update(item);
                        }
                        else if (item == null)
                        {
                            item = new Process();
                            item.ExecInfos = new [] { execInfo };
                            item.ProcessId = hash;
                            item.HashType = 1;
                            item.Flags = 0;
                            item.Path = normalizedPath;
                            //item.Start = DateTime.UtcNow;
                            table.Insert(item);
                            DbProps.Set(db, "LastProcessChange", DateTime.UtcNow);
                        }
                    }
                    else
                        Log(logprefix + "failed hashing file");
                }
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
            }
        }

        // OnProcessCreate: aggregates the initial process info into processInfos (no FullFileName yet)
        void OnProcessCreate(object sender, EventArrivedEventArgs e)
        {
            var logprefix = "ProcessMon.ProcCreate: ";
            //const uint E_INVALIDARG = 0x80070057;
            try
            {
                var shortName = (string)e.NewEvent.Properties["ProcessName"].Value;
                var wtsSessionId = (uint)e.NewEvent.Properties["SessionID"].Value;
                var pid = (uint)(e.NewEvent.Properties["ProcessID"].Value);
                var parentPid = (uint)(e.NewEvent.Properties["ParentProcessID"].Value);
                logprefix = "ProcessMon.ProcCreate(" + pid + ", " + shortName + ", #" + wtsSessionId + "): ";
                lock (processInfosLock)
                {
                    var processInfo = new ProcessInfo
                    {
                        ParentPid = parentPid,
                        ShortName = shortName,
                        WtsSessionId = wtsSessionId,
                    };
                    try
                    {
                        processInfos.Add(pid, processInfo);
                    }
                    catch (Exception ex)
                    {
                        Log(logprefix + "* exception adding processInfo: " + ex.ToString());
                    }
                }
                Log(logprefix + $"Parent={parentPid}");
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
            }
        }

        // OnModuleLoad: completes processinfo with FullFileName and calls OnNewProcessReady
        void OnModuleLoad(object sender, EventArrivedEventArgs e)
        {
            var logprefix = "ProcessMon.ModLoad: ";
            var pid = (uint)e.NewEvent.Properties["ProcessID"].Value;
            ProcessInfo processInfo = null;
            lock (processInfosLock)
            {
                if (!processInfos.TryGetValue(pid, out processInfo))
                    return;   // Unknown process
                if (!string.IsNullOrEmpty(processInfo.FullFileName))
                    return;   // FullFileName already filled
            }
            string str = "", dosName = "";
            try
            {
                dosName = deviceNameResolver.GetDosName((string)e.NewEvent.Properties["FileName"].Value);
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
            }
            str += $"PID={e.NewEvent.Properties["ProcessID"].Value} => {dosName}";
            Log(logprefix + str);
            processInfo.FullFileName = dosName;
            lock (processInfosLock)
            {
                processInfos[pid] = processInfo;
            }
            OnNewProcessReady(pid, processInfo);
        }

        void OnProcessProcStop(object sender, EventArrivedEventArgs e)
        {
            var logprefix = "ProcessMon.ProcStop: ";
            var pid = (uint)e.NewEvent.Properties["ProcessID"].Value;
            lock (processInfosLock)
            {
                if (processInfos.ContainsKey(pid))
                {
                    processInfos.Remove(pid);
                    Log(logprefix + $"{pid} ({e.NewEvent.Properties["ProcessName"].Value}): forgetting");
                    return;
                }
            }
            Log(logprefix + $"{pid} ({e.NewEvent.Properties["ProcessName"].Value}): UNKNOWN, skipping");
        }
        
        public string NormalizePath(string path)
        {
            var ret = Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
            
            // sysdir:   C:\WINDOWS\SYSTEM32
            // windir:   C:\WINDOWS
            // hdroot:   C:\
            // usersdir: C:\USERS
            ret = ret
                .Replace(sysdir + "\\", "%SYSTEM%\\")
                .Replace(windir + "\\", "%WINDOWS%\\")
                .Replace(progData + "\\", "%PROGRAMDATA%\\")
                .Replace(commonProfile + "\\", "%COMMON PROFILE%\\")
                .Replace(progFilesX86 + "\\", "%PROGRAM FILES (X86)%\\")
                .Replace(progFiles + "\\", "%PROGRAM FILES%\\");
            
            if (ret.StartsWith(usersdir + "\\"))
            {
                ret = ret.Remove(0, (usersdir + "\\").Length);
                var pos = ret.IndexOf("\\");  // The one after username
                if (pos != -1)
                    ret = ret.Remove(0, pos + 1);
                ret = "%PROFILE%\\" + ret;
            }
            ret = ret.Replace(hdroot, "%HDROOT%\\");
            return ret;
        }

        class DeviceNameResolver
        {
            [System.Runtime.InteropServices.DllImport("kernel32")]
            private static extern long QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, long ucchMax);

            Dictionary<string, string> DosToDeviceNames;   // In the form: Key="\Device\HarddiskVolume3\" Value="C:\" 

            public DeviceNameResolver()
            {
                DosToDeviceNames = new Dictionary<string, string>();
                var logicalDrives = Environment.GetLogicalDrives();
                var deviceNameSb = new StringBuilder(250);
                foreach (var logicalDrive in logicalDrives)
                {
                    if (QueryDosDevice(logicalDrive.Replace(@"\", ""), deviceNameSb, 250) > 0)
                    {
                        // End both paths with '\' for faster real-time operations 
                        var _logicalDrive = logicalDrive;
                        var deviceName = deviceNameSb.ToString();
                        if (!deviceName.EndsWith(@"\")) deviceName += @"\";
                        if (!_logicalDrive.EndsWith(@"\")) _logicalDrive += @"\";
                        DosToDeviceNames[deviceName] = _logicalDrive;
                    }
                }
            }
            
            public string GetDosName(string deviceName)
            {
                foreach (var knownDeviceName in DosToDeviceNames)
                {
                    if (deviceName.StartsWith(knownDeviceName.Key))
                    {
                        return (deviceName.Replace(knownDeviceName.Key, knownDeviceName.Value));
                    }
                }
                return deviceName;   // No corresponding DOS name found, return the original name
            }
        }
        
        static void Log(string msg) { Utils.Log(msg); }
    }
   
    // Process
    public class Process
    {
        public byte[] ProcessId { get; set; }
        public byte HashType { get; set; }
        public ExecInfo[] ExecInfos { get; set; }
        public long Flags { get; set; }
        public string Path { get; set; }
    }
    public class ExecInfo
    {
        public long SessionUid { get; set; }
        public DateTime Start { get; set; }
        public UInt32 Pid { get; set; }
        public UInt32 ParentPid { get; set; }
        public long Flags { get; set; }
    }
}