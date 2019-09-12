using System;
using System.Diagnostics.Eventing.Reader;
using System.ServiceProcess;
using System.Threading;
using System.Xml;
using LiteDB;

namespace Cameyo.RdpMon
{
    public class SessionMon
    {
        public SessionMon()
        {
            // Catch up with existing sessions, if not already registered in DB
            var logprefix = "SessionMon(): ";
            var activeSessions = WTS.ListSessions();
            using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
            {
                var table = db.GetCollection<Session>("Session");
                foreach (var wtsInfo in activeSessions)
                {
                    if (string.IsNullOrEmpty(wtsInfo.UserName))   // Skipping user-less sessions such as #0 and #65535
                        continue;
                    var sessionUID = wtsInfo.SessionUID();
                    var session = table.FindById(sessionUID);
                    if (session == null)
                    {
                        Log(logprefix + "found active WTS session to catch up with: #" + wtsInfo.ID + "/" + wtsInfo.UserName);
                        session = new Session
                        {
                            SessionUid = sessionUID,
                            WtsSessionId = wtsInfo.ID,
                            Start = wtsInfo.LogonTime,
                            End = null,
                            User = wtsInfo.UserName,
                            Addr = null,   // Perhaps we could still try and obtain IP
                            Flags = 0,
                        };
                        table.Insert(session);
                        DbProps.Set(db, "LastSessionChange", DateTime.UtcNow);
                    }
                }
            }
        }
        
        public void OnSessionChange(SessionChangeDescription sessionChange)
        {
            var wtsSessionId = sessionChange.SessionId;
            var logprefix = "SessionMon #" + wtsSessionId.ToString("D2") + " [" + sessionChange.Reason.ToString() + "]: ";
            //EventLog.WriteEntry("OnSessionChange", changeDescription.Reason.ToString() + " Session ID: " + changeDescription.SessionId.ToString());
            Log(logprefix + "@");
            WTS.SessionInfo wtsInfo;
            var utcNow = DateTime.UtcNow;
            try
            {
                switch (sessionChange.Reason)
                {
                    // SessionLogon
                    case SessionChangeReason.SessionLogon:
                        Thread.Sleep(3000 * 1);   // Wait for session to be logged in event log
                        var provider = "Microsoft-Windows-TerminalServices-LocalSessionManager/Operational";
                        var fromUtc = utcNow.Subtract(TimeSpan.FromMinutes(1));
                        //var toUtc = utcNow.Add(TimeSpan.FromMinutes(1));
                        var query = "*[" +
                                    "(System/EventID=21 and UserData/EventXML/SessionID=" + wtsSessionId + ")" +
                                    " and " +
                                    "System[TimeCreated[@SystemTime>'" + fromUtc.ToString("yyyy-MM-dd") + "T" + fromUtc.ToString("HH:mm:ss") + ".000000000Z" + "']]" +
                                    /*" and " +
                                    "System[TimeCreated[@SystemTime<'" + toUtc.ToString("yyyy-MM-dd") + "T" + toUtc.ToString("HH:mm:ss") + ".000000000Z" + "']] " +*/
                                    "]";
                        var eventsQuery = new EventLogQuery(provider, PathType.LogName, query);
                        {
                            var logReader = new EventLogReader(eventsQuery);
                            for (var evt = logReader.ReadEvent(); evt != null; evt = logReader.ReadEvent())
                            {
                                var xmldoc = new XmlDocument();
                                var xml = evt.ToXml();
                                var ns = new XmlNamespaceManager(xmldoc.NameTable);
                                //ns.AddNamespace("ns", "http://schemas.microsoft.com/win/2004/08/events/event");
                                ns.AddNamespace("ns", "Event_NS");
                                xmldoc.LoadXml(xml);
                                var nodes = xmldoc.SelectNodes("//ns:*", ns);
                                if (nodes != null)
                                {
                                    int _wtsSessionId = -1;
                                    string user = null, addr = null;
                                    for (int i = 0; i < nodes.Count; i++)
                                    {
                                        if (nodes[i].Name == "SessionID")
                                            int.TryParse(nodes[i].InnerText, out _wtsSessionId);
                                        if (nodes[i].Name == "User")
                                            user = nodes[i].InnerText;
                                        if (nodes[i].Name == "Address")
                                            addr = nodes[i].InnerText;
                                    }
                                    Log(logprefix + "session=" + _wtsSessionId + ", user=" + user + ", addr=" + addr);
                                    if (_wtsSessionId != -1)
                                    {
                                        wtsInfo = WTS.QuerySessionInfo(_wtsSessionId);
                                        if (wtsInfo != null)
                                        {
                                            using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
                                            {
                                                var table = db.GetCollection<Session>("Session");
                                                var session = new Session
                                                {
                                                    SessionUid = Session.GetSessionUid(_wtsSessionId, wtsInfo.LogonTime),
                                                    WtsSessionId = _wtsSessionId,
                                                    Start = wtsInfo.LogonTime,
                                                    End = null,
                                                    User = wtsInfo.UserName,
                                                    Addr = addr,
                                                    Flags = 0,
                                                };
                                                table.Insert(session);
                                                DbProps.Set(db, "LastSessionChange", DateTime.UtcNow);
                                            }
                                        }
                                        else
                                            Log(logprefix + "* could not find WTS session #" + _wtsSessionId);
                                    }
                                    else
                                        Log(logprefix + "* could not parse session ID from event log: " + xml);
                                }
                                else
                                    Log(logprefix + "* could not read event log: " + xml);
                            }
                        }
                        break;

                    // SessionLogoff
                    case SessionChangeReason.SessionLogoff:
                        wtsInfo = WTS.QuerySessionInfo(wtsSessionId);
                        if (wtsInfo != null)
                        {
                            using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
                            {
                                var table = db.GetCollection<Session>("Session");
                                var sessionUID = Session.GetSessionUid(wtsSessionId, wtsInfo.LogonTime);
                                var session = table.FindById(sessionUID);
                                if (session != null)
                                {
                                    session.End = DateTime.UtcNow;
                                    table.Update(session);
                                }
                                DbProps.Set(db, "LastSessionChange", DateTime.UtcNow);
                            }
                        }
                        else
                            Log(logprefix + "* could not find WTS session #" + wtsSessionId);
                        break;

                    // Other events
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
            }
        }

        static void Log(string msg) { Utils.Log(msg); }
    }
    
    //
    // Session
    public class Session
    {
        [BsonId]
        public long SessionUid { get; set; }
        public long WtsSessionId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string User { get; set; }
        public long Flags { get; set; }
        public string Addr { get; set; }

        static public long GetSessionUid(long wtsSessionId, DateTime start)
        {
            long ret = start.Ticks;
            ret &= ~(0x7ff0000000000000);
            long hiBit = (wtsSessionId);
            hiBit <<= 48;
            ret |= hiBit;
            return ret;
        }

        static public long GetSessionUid(Session session)
        {
            return GetSessionUid(session.WtsSessionId, session.Start);
        }
    }
}