namespace DOMOPlatform
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // —— 菜单栏 ——
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuModule;
        private System.Windows.Forms.ToolStripMenuItem mnuModuleInit;
        private System.Windows.Forms.ToolStripMenuItem mnuModuleSave;
        private System.Windows.Forms.ToolStripMenuItem mnuModuleClose;
        private System.Windows.Forms.ToolStripMenuItem mnuService;
        private System.Windows.Forms.ToolStripMenuItem mnuServiceConnect;
        private System.Windows.Forms.ToolStripMenuItem mnuServiceDisconnect;

        // —— 主内容区 ——
        private System.Windows.Forms.TabControl tabModules;

        // —— 报警日志 ——
        private System.Windows.Forms.ListBox lstAlarms;
        private System.Windows.Forms.Label lblAlarmHeader;
        private System.Windows.Forms.Panel pnlAlarm;

        // —— 状态栏 ——
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblPosition;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
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
        /// 设计器支持所必需的方法，不要修改。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModule = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModuleInit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModuleSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModuleClose = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuService = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuServiceConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuServiceDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tabModules = new System.Windows.Forms.TabControl();
            this.pnlAlarm = new System.Windows.Forms.Panel();
            this.lblAlarmHeader = new System.Windows.Forms.Label();
            this.lstAlarms = new System.Windows.Forms.ListBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.pnlAlarm.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuModule,
            this.mnuService});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip.Size = new System.Drawing.Size(1884, 35);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(62, 29);
            this.mnuFile.Text = "文件";
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(130, 30);
            this.mnuFileExit.Text = "退出";
            this.mnuFileExit.Click += new System.EventHandler(this.MnuFileExit_Click);
            // 
            // mnuModule
            // 
            this.mnuModule.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuModuleInit,
            this.mnuModuleSave,
            this.mnuModuleClose});
            this.mnuModule.Name = "mnuModule";
            this.mnuModule.Size = new System.Drawing.Size(62, 29);
            this.mnuModule.Text = "模组";
            // 
            // mnuModuleInit
            // 
            this.mnuModuleInit.Name = "mnuModuleInit";
            this.mnuModuleInit.Size = new System.Drawing.Size(190, 30);
            this.mnuModuleInit.Text = "初始化全部";
            this.mnuModuleInit.Click += new System.EventHandler(this.MnuModuleInit_Click);
            // 
            // mnuModuleSave
            // 
            this.mnuModuleSave.Name = "mnuModuleSave";
            this.mnuModuleSave.Size = new System.Drawing.Size(190, 30);
            this.mnuModuleSave.Text = "保存全部";
            this.mnuModuleSave.Click += new System.EventHandler(this.MnuModuleSave_Click);
            // 
            // mnuModuleClose
            // 
            this.mnuModuleClose.Name = "mnuModuleClose";
            this.mnuModuleClose.Size = new System.Drawing.Size(190, 30);
            this.mnuModuleClose.Text = "关闭全部";
            this.mnuModuleClose.Click += new System.EventHandler(this.MnuModuleClose_Click);
            // 
            // mnuService
            // 
            this.mnuService.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuServiceConnect,
            this.mnuServiceDisconnect});
            this.mnuService.Name = "mnuService";
            this.mnuService.Size = new System.Drawing.Size(62, 29);
            this.mnuService.Text = "服务";
            // 
            // mnuServiceConnect
            // 
            this.mnuServiceConnect.Name = "mnuServiceConnect";
            this.mnuServiceConnect.Size = new System.Drawing.Size(190, 30);
            this.mnuServiceConnect.Text = "连接模拟服务";
            this.mnuServiceConnect.Click += new System.EventHandler(this.MnuServiceConnect_Click);
            // 
            // mnuServiceDisconnect
            // 
            this.mnuServiceDisconnect.Name = "mnuServiceDisconnect";
            this.mnuServiceDisconnect.Size = new System.Drawing.Size(190, 30);
            this.mnuServiceDisconnect.Text = "断开服务";
            this.mnuServiceDisconnect.Click += new System.EventHandler(this.MnuServiceDisconnect_Click);
            // 
            // tabModules
            // 
            this.tabModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabModules.Location = new System.Drawing.Point(0, 35);
            this.tabModules.Margin = new System.Windows.Forms.Padding(4);
            this.tabModules.Name = "tabModules";
            this.tabModules.SelectedIndex = 0;
            this.tabModules.Size = new System.Drawing.Size(1884, 869);
            this.tabModules.TabIndex = 1;
            // 
            // pnlAlarm
            // 
            this.pnlAlarm.Controls.Add(this.lstAlarms);
            this.pnlAlarm.Controls.Add(this.lblAlarmHeader);
            this.pnlAlarm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAlarm.Location = new System.Drawing.Point(0, 904);
            this.pnlAlarm.Margin = new System.Windows.Forms.Padding(4);
            this.pnlAlarm.Name = "pnlAlarm";
            this.pnlAlarm.Size = new System.Drawing.Size(1884, 178);
            this.pnlAlarm.TabIndex = 2;
            // 
            // lblAlarmHeader
            // 
            this.lblAlarmHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAlarmHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAlarmHeader.Location = new System.Drawing.Point(0, 0);
            this.lblAlarmHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAlarmHeader.Name = "lblAlarmHeader";
            this.lblAlarmHeader.Size = new System.Drawing.Size(1884, 30);
            this.lblAlarmHeader.TabIndex = 0;
            this.lblAlarmHeader.Text = "报警日志";
            this.lblAlarmHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lstAlarms
            // 
            this.lstAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAlarms.FormattingEnabled = true;
            this.lstAlarms.ItemHeight = 21;
            this.lstAlarms.Location = new System.Drawing.Point(0, 30);
            this.lstAlarms.Margin = new System.Windows.Forms.Padding(4);
            this.lstAlarms.Name = "lstAlarms";
            this.lstAlarms.Size = new System.Drawing.Size(1884, 148);
            this.lstAlarms.TabIndex = 1;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblPosition});
            this.statusStrip.Location = new System.Drawing.Point(0, 1082);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip.Size = new System.Drawing.Size(1884, 29);
            this.statusStrip.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(62, 24);
            this.lblStatus.Text = "就绪";
            // 
            // lblPosition
            // 
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(62, 24);
            this.lblPosition.Spring = true;
            this.lblPosition.Text = "";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1884, 1111);
            this.Controls.Add(this.tabModules);
            this.Controls.Add(this.pnlAlarm);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1200, 800);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DOMO 模拟平台 - 工艺模组测试与演示";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.pnlAlarm.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
