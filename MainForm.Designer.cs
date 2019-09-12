namespace Cameyo.RdpMon
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.lvImages = new System.Windows.Forms.ImageList(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.connectionsTab = new System.Windows.Forms.TabPage();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.connectsLv = new System.Windows.Forms.ListView();
            this.colIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFailCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSuccessCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFirstTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLastTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLogins = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolstripPanel = new System.Windows.Forms.Panel();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.connectsToolStrip = new System.Windows.Forms.ToolStrip();
            this.filterBtnLegits = new System.Windows.Forms.ToolStripButton();
            this.filterBtnAttacks = new System.Windows.Forms.ToolStripButton();
            this.filterBtnUnknown = new System.Windows.Forms.ToolStripButton();
            this.sessionsTab = new System.Windows.Forms.TabPage();
            this.sessionsMainPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.sessionProcessesLv = new System.Windows.Forms.ListView();
            this.colSessionProcessName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sessionsLv = new System.Windows.Forms.ListView();
            this.colSessionStarted = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSessionUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSessionState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSessionEnded = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSessionAddr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colWtsSessionId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sessionsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sessionsShadowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionsToolstripPanel = new System.Windows.Forms.Panel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.sessionsToolstrip = new System.Windows.Forms.ToolStrip();
            this.systemProcsBtn = new System.Windows.Forms.ToolStripButton();
            this.tabs.SuspendLayout();
            this.connectionsTab.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.toolstripPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.connectsToolStrip.SuspendLayout();
            this.sessionsTab.SuspendLayout();
            this.sessionsMainPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.sessionsContextMenuStrip.SuspendLayout();
            this.sessionsToolstripPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.sessionsToolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Tick += new System.EventHandler(this.OnRefreshTimer);
            // 
            // lvImages
            // 
            this.lvImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lvImages.ImageStream")));
            this.lvImages.TransparentColor = System.Drawing.Color.Transparent;
            this.lvImages.Images.SetKeyName(0, "Attack.png");
            this.lvImages.Images.SetKeyName(1, "Legit.png");
            this.lvImages.Images.SetKeyName(2, "Unknown.png");
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(903, 153);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.connectionsTab);
            this.tabs.Controls.Add(this.sessionsTab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1200, 589);
            this.tabs.TabIndex = 0;
            // 
            // connectionsTab
            // 
            this.connectionsTab.Controls.Add(this.mainPanel);
            this.connectionsTab.Location = new System.Drawing.Point(4, 22);
            this.connectionsTab.Name = "connectionsTab";
            this.connectionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.connectionsTab.Size = new System.Drawing.Size(1192, 563);
            this.connectionsTab.TabIndex = 0;
            this.connectionsTab.Text = "Connections";
            this.connectionsTab.UseVisualStyleBackColor = true;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.panel1);
            this.mainPanel.Controls.Add(this.toolstripPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(3, 3);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1186, 557);
            this.mainPanel.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.connectsLv);
            this.panel1.Controls.Add(this.statusStrip);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1186, 529);
            this.panel1.TabIndex = 9;
            // 
            // connectsLv
            // 
            this.connectsLv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIP,
            this.colFailCount,
            this.colSuccessCount,
            this.colFirstTime,
            this.colLastTime,
            this.colDuration,
            this.colLogins});
            this.connectsLv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectsLv.FullRowSelect = true;
            this.connectsLv.Location = new System.Drawing.Point(0, 0);
            this.connectsLv.Name = "connectsLv";
            this.connectsLv.Size = new System.Drawing.Size(1186, 507);
            this.connectsLv.SmallImageList = this.lvImages;
            this.connectsLv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.connectsLv.TabIndex = 5;
            this.connectsLv.UseCompatibleStateImageBehavior = false;
            this.connectsLv.View = System.Windows.Forms.View.Details;
            // 
            // colIP
            // 
            this.colIP.Text = "IP";
            this.colIP.Width = 160;
            // 
            // colFailCount
            // 
            this.colFailCount.Text = "Failures";
            this.colFailCount.Width = 80;
            // 
            // colSuccessCount
            // 
            this.colSuccessCount.Text = "Success";
            this.colSuccessCount.Width = 80;
            // 
            // colFirstTime
            // 
            this.colFirstTime.Text = "First attempt";
            this.colFirstTime.Width = 160;
            // 
            // colLastTime
            // 
            this.colLastTime.Text = "Last attempt";
            this.colLastTime.Width = 160;
            // 
            // colDuration
            // 
            this.colDuration.Text = "Duration";
            this.colDuration.Width = 160;
            // 
            // colLogins
            // 
            this.colLogins.Text = "Logins";
            this.colLogins.Width = 450;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatsLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 507);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 9, 0);
            this.statusStrip.Size = new System.Drawing.Size(1186, 22);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatsLabel
            // 
            this.toolStripStatsLabel.Name = "toolStripStatsLabel";
            this.toolStripStatsLabel.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatsLabel.Text = "Stats";
            // 
            // toolstripPanel
            // 
            this.toolstripPanel.Controls.Add(this.toolStripContainer);
            this.toolstripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolstripPanel.Location = new System.Drawing.Point(0, 0);
            this.toolstripPanel.Name = "toolstripPanel";
            this.toolstripPanel.Size = new System.Drawing.Size(1186, 28);
            this.toolstripPanel.TabIndex = 8;
            // 
            // toolStripContainer
            // 
            this.toolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1186, 0);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(1186, 28);
            this.toolStripContainer.TabIndex = 7;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.connectsToolStrip);
            // 
            // connectsToolStrip
            // 
            this.connectsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.connectsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.connectsToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.connectsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterBtnLegits,
            this.filterBtnAttacks,
            this.filterBtnUnknown});
            this.connectsToolStrip.Location = new System.Drawing.Point(3, 0);
            this.connectsToolStrip.Name = "connectsToolStrip";
            this.connectsToolStrip.Size = new System.Drawing.Size(87, 31);
            this.connectsToolStrip.TabIndex = 0;
            // 
            // filterBtnLegits
            // 
            this.filterBtnLegits.Checked = true;
            this.filterBtnLegits.CheckOnClick = true;
            this.filterBtnLegits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterBtnLegits.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterBtnLegits.Image = ((System.Drawing.Image)(resources.GetObject("filterBtnLegits.Image")));
            this.filterBtnLegits.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterBtnLegits.Name = "filterBtnLegits";
            this.filterBtnLegits.Size = new System.Drawing.Size(28, 28);
            this.filterBtnLegits.Text = "Successful connections";
            this.filterBtnLegits.ToolTipText = "Show legitimate connections";
            this.filterBtnLegits.Click += new System.EventHandler(this.OnConnectFilterClick);
            // 
            // filterBtnAttacks
            // 
            this.filterBtnAttacks.Checked = true;
            this.filterBtnAttacks.CheckOnClick = true;
            this.filterBtnAttacks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterBtnAttacks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterBtnAttacks.Image = ((System.Drawing.Image)(resources.GetObject("filterBtnAttacks.Image")));
            this.filterBtnAttacks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterBtnAttacks.Name = "filterBtnAttacks";
            this.filterBtnAttacks.Size = new System.Drawing.Size(28, 28);
            this.filterBtnAttacks.Text = "Likely brute-force attempts";
            this.filterBtnAttacks.ToolTipText = "Show illegitimate connections";
            this.filterBtnAttacks.Click += new System.EventHandler(this.OnConnectFilterClick);
            // 
            // filterBtnUnknown
            // 
            this.filterBtnUnknown.Checked = true;
            this.filterBtnUnknown.CheckOnClick = true;
            this.filterBtnUnknown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterBtnUnknown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterBtnUnknown.Image = ((System.Drawing.Image)(resources.GetObject("filterBtnUnknown.Image")));
            this.filterBtnUnknown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterBtnUnknown.Name = "filterBtnUnknown";
            this.filterBtnUnknown.Size = new System.Drawing.Size(28, 28);
            this.filterBtnUnknown.Text = "Low-intensity connection failures";
            this.filterBtnUnknown.ToolTipText = "Show uncertain connections";
            this.filterBtnUnknown.Click += new System.EventHandler(this.OnConnectFilterClick);
            // 
            // sessionsTab
            // 
            this.sessionsTab.Controls.Add(this.sessionsMainPanel);
            this.sessionsTab.Location = new System.Drawing.Point(4, 22);
            this.sessionsTab.Name = "sessionsTab";
            this.sessionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.sessionsTab.Size = new System.Drawing.Size(1192, 563);
            this.sessionsTab.TabIndex = 1;
            this.sessionsTab.Text = "Sessions";
            this.sessionsTab.UseVisualStyleBackColor = true;
            // 
            // sessionsMainPanel
            // 
            this.sessionsMainPanel.Controls.Add(this.panel3);
            this.sessionsMainPanel.Controls.Add(this.sessionsLv);
            this.sessionsMainPanel.Controls.Add(this.sessionsToolstripPanel);
            this.sessionsMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsMainPanel.Location = new System.Drawing.Point(3, 3);
            this.sessionsMainPanel.Name = "sessionsMainPanel";
            this.sessionsMainPanel.Size = new System.Drawing.Size(1186, 557);
            this.sessionsMainPanel.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 377);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1186, 180);
            this.panel3.TabIndex = 9;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.sessionProcessesLv);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1186, 180);
            this.panel5.TabIndex = 0;
            // 
            // sessionProcessesLv
            // 
            this.sessionProcessesLv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSessionProcessName});
            this.sessionProcessesLv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionProcessesLv.FullRowSelect = true;
            this.sessionProcessesLv.Location = new System.Drawing.Point(0, 0);
            this.sessionProcessesLv.Name = "sessionProcessesLv";
            this.sessionProcessesLv.Size = new System.Drawing.Size(1186, 180);
            this.sessionProcessesLv.SmallImageList = this.lvImages;
            this.sessionProcessesLv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.sessionProcessesLv.TabIndex = 9;
            this.sessionProcessesLv.UseCompatibleStateImageBehavior = false;
            this.sessionProcessesLv.View = System.Windows.Forms.View.Details;
            // 
            // colSessionProcessName
            // 
            this.colSessionProcessName.Text = "Processes";
            this.colSessionProcessName.Width = 630;
            // 
            // sessionsLv
            // 
            this.sessionsLv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSessionStarted,
            this.colSessionUser,
            this.colSessionState,
            this.colSessionEnded,
            this.colSessionAddr,
            this.colWtsSessionId});
            this.sessionsLv.ContextMenuStrip = this.sessionsContextMenuStrip;
            this.sessionsLv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsLv.FullRowSelect = true;
            this.sessionsLv.Location = new System.Drawing.Point(0, 28);
            this.sessionsLv.Name = "sessionsLv";
            this.sessionsLv.Size = new System.Drawing.Size(1186, 529);
            this.sessionsLv.SmallImageList = this.lvImages;
            this.sessionsLv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.sessionsLv.TabIndex = 8;
            this.sessionsLv.UseCompatibleStateImageBehavior = false;
            this.sessionsLv.View = System.Windows.Forms.View.Details;
            this.sessionsLv.SelectedIndexChanged += new System.EventHandler(this.OnSessionsLvSelectionChanged);
            // 
            // colSessionStarted
            // 
            this.colSessionStarted.Text = "Started";
            this.colSessionStarted.Width = 160;
            // 
            // colSessionUser
            // 
            this.colSessionUser.Text = "User";
            this.colSessionUser.Width = 120;
            // 
            // colSessionState
            // 
            this.colSessionState.Text = "State";
            this.colSessionState.Width = 140;
            // 
            // colSessionEnded
            // 
            this.colSessionEnded.Text = "Ended";
            this.colSessionEnded.Width = 160;
            // 
            // colSessionAddr
            // 
            this.colSessionAddr.Text = "IP";
            this.colSessionAddr.Width = 160;
            // 
            // colWtsSessionId
            // 
            this.colWtsSessionId.Text = "Session ID";
            this.colWtsSessionId.Width = 80;
            // 
            // sessionsContextMenuStrip
            // 
            this.sessionsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sessionsShadowMenuItem});
            this.sessionsContextMenuStrip.Name = "sessionsContextMenuStrip";
            this.sessionsContextMenuStrip.Size = new System.Drawing.Size(144, 26);
            this.sessionsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.SessionsContextMenuStrip_Opening);
            // 
            // sessionsShadowMenuItem
            // 
            this.sessionsShadowMenuItem.Name = "sessionsShadowMenuItem";
            this.sessionsShadowMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sessionsShadowMenuItem.Text = "Shadow view";
            this.sessionsShadowMenuItem.Click += new System.EventHandler(this.SessionsShadowMenuItem_Click);
            // 
            // sessionsToolstripPanel
            // 
            this.sessionsToolstripPanel.Controls.Add(this.toolStripContainer1);
            this.sessionsToolstripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sessionsToolstripPanel.Location = new System.Drawing.Point(0, 0);
            this.sessionsToolstripPanel.Name = "sessionsToolstripPanel";
            this.sessionsToolstripPanel.Size = new System.Drawing.Size(1186, 28);
            this.sessionsToolstripPanel.TabIndex = 10;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1186, 3);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1186, 28);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.sessionsToolstrip);
            // 
            // sessionsToolstrip
            // 
            this.sessionsToolstrip.Dock = System.Windows.Forms.DockStyle.None;
            this.sessionsToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.sessionsToolstrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.sessionsToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemProcsBtn});
            this.sessionsToolstrip.Location = new System.Drawing.Point(3, 0);
            this.sessionsToolstrip.Name = "sessionsToolstrip";
            this.sessionsToolstrip.Size = new System.Drawing.Size(26, 25);
            this.sessionsToolstrip.TabIndex = 0;
            // 
            // systemProcsBtn
            // 
            this.systemProcsBtn.CheckOnClick = true;
            this.systemProcsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.systemProcsBtn.Image = ((System.Drawing.Image)(resources.GetObject("systemProcsBtn.Image")));
            this.systemProcsBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.systemProcsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.systemProcsBtn.Name = "systemProcsBtn";
            this.systemProcsBtn.Size = new System.Drawing.Size(23, 22);
            this.systemProcsBtn.Text = "Successful connections";
            this.systemProcsBtn.ToolTipText = "Show / hide system processes";
            this.systemProcsBtn.Click += new System.EventHandler(this.OnSessionsFilterClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 589);
            this.Controls.Add(this.tabs);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RDP Monitor";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.tabs.ResumeLayout(false);
            this.connectionsTab.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolstripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.connectsToolStrip.ResumeLayout(false);
            this.connectsToolStrip.PerformLayout();
            this.sessionsTab.ResumeLayout(false);
            this.sessionsMainPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.sessionsContextMenuStrip.ResumeLayout(false);
            this.sessionsToolstripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.sessionsToolstrip.ResumeLayout(false);
            this.sessionsToolstrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.ImageList lvImages;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage connectionsTab;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView connectsLv;
        private System.Windows.Forms.ColumnHeader colIP;
        private System.Windows.Forms.ColumnHeader colFailCount;
        private System.Windows.Forms.ColumnHeader colSuccessCount;
        private System.Windows.Forms.ColumnHeader colFirstTime;
        private System.Windows.Forms.ColumnHeader colLastTime;
        private System.Windows.Forms.ColumnHeader colDuration;
        private System.Windows.Forms.ColumnHeader colLogins;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatsLabel;
        private System.Windows.Forms.Panel toolstripPanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip connectsToolStrip;
        private System.Windows.Forms.ToolStripButton filterBtnLegits;
        private System.Windows.Forms.ToolStripButton filterBtnAttacks;
        private System.Windows.Forms.ToolStripButton filterBtnUnknown;
        private System.Windows.Forms.TabPage sessionsTab;
        private System.Windows.Forms.Panel sessionsMainPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ListView sessionProcessesLv;
        private System.Windows.Forms.ColumnHeader colSessionProcessName;
        private System.Windows.Forms.ListView sessionsLv;
        private System.Windows.Forms.ColumnHeader colWtsSessionId;
        private System.Windows.Forms.ColumnHeader colSessionUser;
        private System.Windows.Forms.ColumnHeader colSessionState;
        private System.Windows.Forms.ColumnHeader colSessionStarted;
        private System.Windows.Forms.ColumnHeader colSessionEnded;
        private System.Windows.Forms.ColumnHeader colSessionAddr;
        //private System.Windows.Forms.ColumnHeader colPastSessionUID;
        private System.Windows.Forms.Panel sessionsToolstripPanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip sessionsToolstrip;
        private System.Windows.Forms.ToolStripButton systemProcsBtn;
        private System.Windows.Forms.ContextMenuStrip sessionsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem sessionsShadowMenuItem;
    }
}

