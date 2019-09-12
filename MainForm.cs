using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using Microsoft.Win32;

namespace Cameyo.RdpMon
{
    public partial class MainForm : Form
    {
        DateTime fromDate;
        DateTime lastConnectRefresh = DateTime.MinValue;
        DateTime lastSessionsRefresh = DateTime.MinValue;
        Addrs addrs = null;
        ListViewColumnSorter connectsSorter, sessionsSorter;
        long totalLegits = 0, totalAttackers = 0, totalAttempts = 0;
        int nla = 0;
        bool dbg = false;

        public ColumnHeader ColDuration => colDuration;
        public ColumnHeader ColFailCount => colFailCount;
        public ColumnHeader ColSuccessCount => colSuccessCount;
        public ColumnHeader ColFirstTime => colFirstTime;
        public ColumnHeader ColLastTime => colLastTime;
        public ColumnHeader ColIP => colIP;

        public ColumnHeader ColWtsSessionId => colWtsSessionId;
        public ColumnHeader ColSessionUser => colSessionUser;
        public ColumnHeader ColSessionState => colSessionState;
        public ColumnHeader ColSessionStarted => colSessionStarted;
        public ColumnHeader ColSessionEnded => colSessionEnded;
        public ColumnHeader ColSessionAddr => colSessionAddr;

        public MainForm()
        {
            InitializeComponent();
            fromDate = DateTime.MinValue;
            connectsSorter = new ListViewColumnSorter(this);
            sessionsSorter = new ListViewColumnSorter(this);
            connectsLv.ColumnClick += OnLvColumnClick;
            sessionsLv.ColumnClick += OnLvColumnClick;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            // Check NLA status
            nla = 0;
            try
            {
                var regKey = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", false);
                if (regKey != null)
                    nla = (int)regKey.GetValue("SecurityLayer");
            }
            catch { nla = -1; }

            // First load
            connectsSorter.SortColumn = colLastTime.DisplayIndex;
            connectsSorter.Order = SortOrder.Descending;
            sessionsSorter.SortColumn = colSessionStarted.DisplayIndex;
            sessionsSorter.Order = SortOrder.Descending;
            SetDoubleBuffered(connectsLv);
            SetDoubleBuffered(sessionsLv);
            SetDoubleBuffered(sessionProcessesLv);
            RefreshLV(true);
            refreshTimer.Interval = dbg ? 5000 : 5000;
            refreshTimer.Enabled = true;
        }

        void RefreshLV(bool initialLoad)
        {
            if (initialLoad || tabs.SelectedIndex == 0)
                RefreshConnectionsLV(initialLoad);
            if (initialLoad || tabs.SelectedIndex == 1)
                RefreshSessionsLV(initialLoad, WTS.ListSessions());
        }

        void RefreshConnectionsLV(bool initialLoad)
        {
            int exitCode = 0;
            var startedLvUpdate = false;
            var lv = connectsLv;

            // Initial load or update?
            if (initialLoad)
            {
                lastConnectRefresh = DateTime.MinValue;
                startedLvUpdate = true;
                lv.BeginUpdate();
                lv.ListViewItemSorter = null;
                lv.Items.Clear();
                totalAttackers = totalAttempts = totalLegits = 0;
            }
            var now = DateTime.UtcNow;

            if (dbg)
                Utils.ExecProg(Utils.MyExe(), "-collect", ref exitCode, 60000, false);
            var _from = (lastConnectRefresh < fromDate ? fromDate : lastConnectRefresh);

            // Aggregate logins
            var aggregator = new ConnectMon(/*fromDate*/false, false);
            var _addrs = aggregator.Aggregate(_from);
            var changes = _addrs.Items.Count;
            addrs = _addrs;
            foreach (var item in addrs.Items)
            {
                var ip = item.Key;
                var addr = item.Value;
                var isAttack = addr.IsAttack();
                var isLegit = addr.IsLegit();
                //var isAttack = (addr.SuccessCount == 0);
                //if (isAttack && !addr.IsAttack())
                //    continue;
                if (addr.Last < lastConnectRefresh)
                    continue;

                // Filter
                if (isAttack && !filterBtnAttacks.Checked)
                    continue;
                if (isLegit && !filterBtnLegits.Checked)
                    continue;
                if (!isAttack && !isLegit && !filterBtnUnknown.Checked)
                    continue;

                if (!startedLvUpdate)
                {
                    startedLvUpdate = true;
                    lv.BeginUpdate();
                    lv.ListViewItemSorter = null;
                }

                var existingidx = -1;
                for (int i = 0; i < lv.Items.Count; i++)
                {
                    if (lv.Items[i].Text == ip)
                    {
                        existingidx = i;
                        break;
                    }
                }

                ListViewItem lvi;
                if (existingidx != -1)
                    lvi = lv.Items[existingidx];
                else
                {
                    lvi = new ListViewItem();
                    lvi.SubItems.AddRange(new[] { "", "", "", "", "", "" });
                }
                lvi.SubItems[colIP.DisplayIndex].Text = ip;
                lvi.SubItems[colFailCount.DisplayIndex].Text = addr.FailCount.ToString();
                lvi.SubItems[colSuccessCount.DisplayIndex].Text = addr.SuccessCount.ToString();
                lvi.SubItems[colFirstTime.DisplayIndex].Text = addr.First.ToLocalTime().ToString("MM/dd HH:mm:ss");
                lvi.SubItems[colLastTime.DisplayIndex].Text = addr.Last.ToLocalTime().ToString("MM/dd HH:mm:ss");
                if (addr.UserNames.Count() <= 5)
                    lvi.SubItems[colLogins.DisplayIndex].Text = string.Join(", ", addr.UserNames);
                else
                    lvi.SubItems[colLogins.DisplayIndex].Text = string.Join(", ", addr.UserNames.Take(5)) + $"... ({addr.UserNames.Count})";
                if (isAttack)
                {
                    lvi.ImageIndex = 0;
                    if (addr.IsOngoing()) //|| (lastRefresh != DateTime.MinValue && attack.Last > lastRefresh))
                    {
                        lvi.SubItems[colDuration.DisplayIndex].Text = "ongoing";
                        lvi.UseItemStyleForSubItems = false;
                        lvi.SubItems[colDuration.DisplayIndex].ForeColor = Color.Red;
                        //lvi.ImageIndex = 1;
                    }
                    else
                        lvi.SubItems[colDuration.DisplayIndex].Text = Utils.DurationStr(addr.Last.Subtract(addr.First));
                }
                else if (isLegit)
                {
                    lvi.ImageIndex = 1;
                }
                else   // Neither attack nor legit; not enough data
                {
                    lvi.ImageIndex = 2;
                }
                lvi.Tag = addr;
                if (existingidx == -1)
                {
                    lv.Items.Add(lvi);
                    if (isAttack)
                        totalAttackers++;
                    else if (isLegit)
                        totalLegits++;
                }
            }

            if (!initialLoad)
            {
                totalAttempts = 0;
                for (int i = 0; i < lv.Items.Count; i++)
                {
                    // Update "Ongoing" items that are no longer ongoing
                    if (lv.Items[i].SubItems[colDuration.DisplayIndex].Text == "ongoing")
                    {
                        var _addr = (Addr)lv.Items[i].Tag;
                        if (!_addr.IsOngoing())
                        {
                            lv.Items[i].SubItems[colDuration.DisplayIndex].Text = Utils.DurationStr(_addr.Last.Subtract(_addr.First));
                            lv.Items[i].UseItemStyleForSubItems = true;
                            lv.Items[i].ForeColor = Color.Black;
                            //lv.Items[i].ImageIndex = 0;
                        }
                    }
                    var addr = (Addr)lv.Items[i].Tag;
                    totalAttempts += addr.FailCount;
                }
            }
            if (startedLvUpdate)
            {
                lv.ListViewItemSorter = connectsSorter;
                lv.Sort();
                lv.EndUpdate();
                lv.ListViewItemSorter = null;
            }
            lastConnectRefresh = now;

            // Statistics
            toolStripStatsLabel.Text = totalLegits + " legitimate users, " + totalAttackers + " suspected addresses";
            if (totalAttempts > 0)
            {
                toolStripStatsLabel.Text += ", " + totalAttempts + " password attempts";
                if (nla <= 0)
                    toolStripStatsLabel.Text += ", WARNING: NLA not activated on this machine!";
            }
        }

        void RefreshSessionsLV(bool initialLoad, List<WTS.SessionInfo> activeSessions)
        {
            //var logprefix = "RefreshSessionsLV: ";
            var startedLvUpdate = false;
            var lv = sessionsLv;

            // Initial load or update?
            if (initialLoad)
            {
                lastSessionsRefresh = DateTime.MinValue;
                startedLvUpdate = true;
                lv.BeginUpdate();
                lv.ListViewItemSorter = null;
                lv.Items.Clear();
            }
            var now = DateTime.UtcNow;

            using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
            {
                var _lastDbModif = DbProps.Get(db, "LastSessionChange");
                var lastDbModif = (_lastDbModif != null ? (DateTime)_lastDbModif : DateTime.MinValue);
                var table = db.GetCollection<Session>("Session");
                foreach (var dbSession in table.FindAll())
                {
                    if (dbSession.Start < lastSessionsRefresh && dbSession.End != null)
                    {
                        if (DateTime.UtcNow.Subtract(dbSession.End.Value) < TimeSpan.FromSeconds(60) &&
                            FindSessionLvItem(dbSession.SessionUid, out var lvi))
                        {
                            // Update just-ended session
                            lvi.SubItems[ColSessionState.DisplayIndex].Text = "Ended";
                            lvi.SubItems[ColSessionEnded.DisplayIndex].Text = (dbSession.End != null ? dbSession.End.Value.ToLocalTime().ToString("MM/dd HH:mm:ss") : "");
                            lvi.ImageIndex = -1;
                        }
                        continue;
                    }

                    var equivalentActiveSession = GetEquivalentActiveSession(dbSession, activeSessions);
                    if (!startedLvUpdate)
                    {
                        startedLvUpdate = true;
                        lv.BeginUpdate();
                        lv.ListViewItemSorter = null;
                    }

                    var existingFound = false;
                    if (equivalentActiveSession != null)
                    {
                        if (FindSessionLvItem(dbSession.SessionUid, out var lvi))
                        {
                            lvi.SubItems[ColSessionStarted.DisplayIndex].Text = dbSession.Start.ToLocalTime().ToString("MM/dd HH:mm:ss");
                            lvi.SubItems[ColSessionState.DisplayIndex].Text = equivalentActiveSession.StateStr();
                            lvi.Tag = dbSession;
                            existingFound = true;
                        }
                    }
                    if (existingFound)
                        continue;

                    // Add / update in list
                    {
                        var adding = false;
                        if (!FindSessionLvItem(dbSession.SessionUid, out var lvi)) // Special case: sometimes LvItem may be found in list if the DB has missed its Ended time (i.e. if the service was down while the session ended)
                        {
                            // Usual case
                            lvi = new ListViewItem();
                            lvi.SubItems.AddRange(new[] { "", "", "", "", "", "", "" });
                            adding = true;
                        }

                        lvi.SubItems[ColWtsSessionId.DisplayIndex].Text = dbSession.WtsSessionId.ToString();
                        lvi.SubItems[ColSessionUser.DisplayIndex].Text = (dbSession.User ?? "").ToString();
                        lvi.SubItems[ColSessionStarted.DisplayIndex].Text = dbSession.Start.ToLocalTime().ToString("MM/dd HH:mm:ss");
                        if (dbSession.End != null)
                            lvi.SubItems[ColSessionState.DisplayIndex].Text = "Ended";
                        if (equivalentActiveSession != null)
                        {
                            lvi.ImageIndex = 1;
                            lvi.SubItems[ColSessionEnded.DisplayIndex].Text = "ongoing";
                        }
                        else
                            lvi.SubItems[ColSessionEnded.DisplayIndex].Text = (dbSession.End != null ? dbSession.End.Value.ToLocalTime().ToString("MM/dd HH:mm:ss") : "");

                        lvi.SubItems[ColSessionAddr.DisplayIndex].Text = (dbSession.Addr == "127.0.0.1" ? "localhost" : dbSession.Addr);
                        lvi.Tag = dbSession;
                        if (adding)
                            lv.Items.Add(lvi);
                    }
                }
            }
            if (startedLvUpdate)
            {
                lv.ListViewItemSorter = sessionsSorter;
                lv.Sort();
                lv.EndUpdate();
                lv.ListViewItemSorter = null;
            }
            lastSessionsRefresh = now;
        }

        bool FindSessionLvItem(long sessionUid, out ListViewItem lvi)
        {
            var lv = sessionsLv;
            for (int i = 0; i < lv.Items.Count; i++)
            {
                var _session = (Session)lv.Items[i].Tag;
                if (_session.SessionUid == sessionUid)
                {
                    lvi = lv.Items[i];
                    return true;
                }
            }
            lvi = null;
            return false;
        }

        WTS.SessionInfo GetEquivalentActiveSession(RdpMon.Session dbSession, List<WTS.SessionInfo> activeSessions)
        {
            WTS.SessionInfo ret = null;
            if (dbSession.End == null)
            {
                // Check if there's an equivalent active session for this entry
                var activeSessionQuery = from n in activeSessions where (dbSession.SessionUid == n.SessionUID()) select n;
                if (activeSessionQuery.Count() == 1)
                    ret = activeSessionQuery.First();
            }
            return ret;
        }

        private void OnRefreshTimer(object sender, EventArgs e)
        {
            var logprefix = "OnRefreshTimer: ";
            try
            {
                RefreshLV(false);
            }
            catch (Exception ex)
            {
                Log(logprefix + "* exception: " + ex.ToString());
            }
        }

        private void OnLvColumnClick(object sender, ColumnClickEventArgs e)
        {
            var senderLv = (ListView)sender;
            ListViewColumnSorter sorter = null;
            if (senderLv == connectsLv)
                sorter = connectsSorter;
            else if (senderLv == sessionsLv)
                sorter = sessionsSorter;
            else
                return;
                
            senderLv.BeginUpdate();
            senderLv.ListViewItemSorter = sorter;
            if (e.Column == sorter.SortColumn)
            {
                if (sorter.Order == SortOrder.Ascending)
                    sorter.Order = SortOrder.Descending;
                else
                    sorter.Order = SortOrder.Ascending;
            }
            else
            {
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }
            senderLv.Sort();
            senderLv.EndUpdate();
            senderLv.ListViewItemSorter = null;
        }

        private void OnConnectFilterClick(object sender, EventArgs e)
        {
            RefreshConnectionsLV(true);
        }

        private void OnSessionsFilterClick(object sender, EventArgs e)
        {
            OnSessionsLvSelectionChanged(sessionsLv, null);
        }

        private void OnSessionsLvSelectionChanged(object sender, EventArgs e)
        {
            if (sessionsLv.SelectedItems.Count != 1)
                return;
            var item = sessionsLv.SelectedItems[0];
            var session = (Cameyo.RdpMon.Session)item.Tag;
            var sessionUid = session.SessionUid;
            IEnumerable<Cameyo.RdpMon.Process> processes = null;
            using (var db = new LiteDatabase("Filename=" + Utils.MyPath("RdpMon.db") + ";utc=true"))
            {
                var table = db.GetCollection<Process>("Process");
                processes = table.Find(Query.EQ("ExecInfos[*].SessionUid", sessionUid));
            }

            sessionProcessesLv.BeginUpdate();
            sessionProcessesLv.ListViewItemSorter = null;
            sessionProcessesLv.Items.Clear();
            var sysProcs = systemProcsBtn.Checked;
            foreach (var process in processes)
            {
                var path = process.Path;
                if (!sysProcs)
                {
                    if (path == @"%SYSTEM%\CONSENT.EXE" ||
                        path == @"%SYSTEM%\CTFMON.EXE" ||
                        path == @"%SYSTEM%\DWM.EXE" ||
                        path == @"%SYSTEM%\FONTDRVHOST.EXE" ||
                        path == @"%SYSTEM%\LOGONUI.EXE" ||
                        path == @"%SYSTEM%\RDPCLIP.EXE" ||
                        path == @"%SYSTEM%\SIHOST.EXE" ||
                        path == @"%SYSTEM%\SVCHOST.EXE" ||
                        path == @"%SYSTEM%\TASKHOSTW.EXE" ||
                        path == @"%SYSTEM%\TSTHEME.EXE" ||
                        path == @"%SYSTEM%\USERINIT.EXE" ||
                        path == @"%SYSTEM%\WINLOGON.EXE" ||
                        path == @"%HDROOT%\REMOTEAPPPILOT\REMOTEAPPPILOT.EXE" ||
                        path == @"%HDROOT%\REMOTEAPPPILOT\VIRTPH.EXE")
                        continue;
                }
                sessionProcessesLv.Items.Add(path);
            }
            sessionProcessesLv.EndUpdate();
        }

        private void SessionsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var isActive = (sessionsLv.SelectedItems.Count > 0);   // Are selected sessions currently active
            var activeSessions = WTS.ListSessions();
            for (int i = 0; i < sessionsLv.SelectedItems.Count; i++)
            {
                var session = (Cameyo.RdpMon.Session)sessionsLv.SelectedItems[i].Tag;
                var equivalentActiveSession = GetEquivalentActiveSession(session, activeSessions);
                if (equivalentActiveSession == null)
                {
                    isActive = false;
                    break;
                }
            }
            sessionsShadowMenuItem.Enabled = isActive;
        }

        private void SessionsShadowMenuItem_Click(object sender, EventArgs e)
        {
            var activeSessions = WTS.ListSessions();
            var wtsSessionIds = new List<long>();
            for (int i = 0; i < sessionsLv.SelectedItems.Count; i++)
            {
                var session = (Cameyo.RdpMon.Session)sessionsLv.SelectedItems[i].Tag;
                var equivalentActiveSession = GetEquivalentActiveSession(session, activeSessions);
                if (equivalentActiveSession != null)   // Just in case the session has ended since menu was displayed
                    wtsSessionIds.Add(session.WtsSessionId);
            }
            if (!wtsSessionIds.Any())
                return;
            var dialog = new ShadowForm();
            dialog.wtsSessionIds = wtsSessionIds.ToArray();
            dialog.ShowDialog();
        }

        private static void Log(string msg) { Utils.Log(msg); }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}
