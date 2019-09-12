using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Cameyo.RdpMon
{
    partial class Service : ServiceBase
    {
        Thread mainThread = null;
        bool Stopping = false;
        EventWaitHandle waitEvent;
        SessionMon sessionMon = null;
        ProcessMon processMon = null;

        public Service()
        {
            sessionMon = new SessionMon();
            processMon = new ProcessMon();
            CanHandleSessionChangeEvent = true;
            CanHandlePowerEvent = true;
            InitializeComponent();
            try
            {
                var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                var rule = new EventWaitHandleAccessRule(users, 
                    EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify, 
                    AccessControlType.Allow);
                var security = new EventWaitHandleSecurity();
                security.AddAccessRule(rule);
                waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, @"Global\RdpMonRefresh", out var createdNew, security);
                Log("created global event");
            }
            catch (Exception ex)
            {
                Log("* failed creating global refresh event: " + ex.ToString());
                waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset);   // Fallback object
            }
        }

        void MainThread()
        {
            var logprefix = "MainThread: ";
            var upsince = DateTime.UtcNow;
            var iteration = 0;
            var myExeName = System.IO.Path.GetFileNameWithoutExtension(Utils.MyExe());
            var myPid = System.Diagnostics.Process.GetCurrentProcess().Id;

            var connectionsAggregator = new ConnectMon(true, true);
            while (!Stopping)
            {
                try
                {
                    Log(logprefix + "iteration #" + (++iteration));
                    connectionsAggregator.Aggregate(DateTime.MinValue);
                }
                catch (Exception ex)
                {
                    Log(logprefix + "* exception: " + ex.ToString());
                }

                var myProcesses = System.Diagnostics.Process.GetProcessesByName(myExeName);
                var guiProcesses = from n in myProcesses where n.Id != myPid select n;
                int wait;
                if (guiProcesses.Any())
                {
                    Log(logprefix + "GUI boost mode");
                    wait = 10 * 1000;
                }
                else
                    wait = 3 * 60 * 1000;   // CPU saving mode
                waitEvent.WaitOne(wait);
            }
            Log(logprefix + "quitting");
        }

        public void _OnStart(string[] args)
        {
            OnStart(args);
        }
        
        protected override void OnStart(string[] args)
        {
            Log("OnStart: creating MainThread");
            var threadStart = new ThreadStart(MainThread);
            mainThread = new Thread(threadStart);
            mainThread.Start();
            Log("OnStart: out");
        }

        protected override void OnStop()
        {
            Stopping = true;
            waitEvent.Set();
            //if (mainThread != null)
            //    mainThread.Interrupt();
        }

        protected override void OnSessionChange(SessionChangeDescription sessionChange)
        {
            sessionMon.OnSessionChange(sessionChange);
        }

        static void Log(string msg) { Utils.Log(msg); }
    }
}
