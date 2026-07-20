namespace XyzController
{
    /// <summary>
    /// HardwareForm 的设计器文件。
    /// 此文件采用 VS 设计器兼容的纯声明式写法：每个控件属性逐行显式设置，
    /// 全部使用字面常量（不使用运算表达式 / 方法调用 / 循环 / 条件），
    /// 以便在 VS2017 及以上 WinForms 设计器中正常打开。
    ///
    /// 动态布局（如 SplitterDistance 跟随窗体大小）放在 HardwareForm.cs 的
    /// OnLoad / OnResize 里，不写在这里（设计器无法解析运算表达式）。
    /// </summary>
    partial class HardwareForm
    {
        /// <summary>必需的设计器变量。</summary>
        private System.ComponentModel.IContainer components = null;

        // —— 主布局容器 ——
        private System.Windows.Forms.SplitContainer splitMain;

        // —— 左侧 XY 平面视图 ——
        private XyzController.Controls.XYView xyView;

        // —— 右侧 Z / U 轴竖直条 ——
        private System.Windows.Forms.Panel pnlRight;
        private XyzController.Controls.ZBarView zBar;
        private XyzController.Controls.ZBarView uBar;

        // —— 顶部按钮区 ——
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnCenter;
        private System.Windows.Forms.Button btnEStop;

        // —— 底部状态栏 ——
        private System.Windows.Forms.Label lblStatus;

        // —— 刷新定时器（30ms 一次，~33fps）——
        private System.Windows.Forms.Timer refreshTimer;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">true 表示释放托管资源。</param>
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
        /// 设计器支持所必需的方法，不要使用代码编辑器修改它。
        /// 此方法的内容必须保持纯声明式：所有属性赋值使用字面常量。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.xyView = new XyzController.Controls.XYView();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.zBar = new XyzController.Controls.ZBarView();
            this.uBar = new XyzController.Controls.ZBarView();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnCenter = new System.Windows.Forms.Button();
            this.btnEStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.IsSplitterFixed = true;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.xyView);
            this.splitMain.Panel1MinSize = 200;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pnlRight);
            this.splitMain.Panel2MinSize = 184;
            this.splitMain.Size = new System.Drawing.Size(1488, 744);
            this.splitMain.SplitterDistance = 712;
            this.splitMain.TabIndex = 0;
            // 
            // xyView
            // 
            this.xyView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xyView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.xyView.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xyView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.xyView.Location = new System.Drawing.Point(0, 0);
            this.xyView.Name = "xyView";
            this.xyView.RangeMax = 100F;
            this.xyView.RangeMin = -100F;
            this.xyView.Size = new System.Drawing.Size(712, 744);
            this.xyView.TabIndex = 0;
            this.xyView.TargetX = 0F;
            this.xyView.TargetY = 0F;
            this.xyView.Text = "xyView";
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            this.pnlRight.Controls.Add(this.zBar);
            this.pnlRight.Controls.Add(this.uBar);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(184, 540);
            this.pnlRight.TabIndex = 0;
            // 
            // zBar
            // 
            this.zBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.zBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.zBar.Location = new System.Drawing.Point(8, 8);
            this.zBar.Name = "zBar";
            this.zBar.RangeMax = 100F;
            this.zBar.RangeMin = -50F;
            this.zBar.Size = new System.Drawing.Size(80, 524);
            this.zBar.TabIndex = 0;
            this.zBar.TargetZ = 0F;
            this.zBar.Text = "zBar";
            // 
            // uBar
            // 
            this.uBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.uBar.Location = new System.Drawing.Point(96, 8);
            this.uBar.Name = "uBar";
            this.uBar.RangeMax = 360F;
            this.uBar.RangeMin = -360F;
            this.uBar.Size = new System.Drawing.Size(80, 524);
            this.uBar.TabIndex = 1;
            this.uBar.TargetZ = 0F;
            this.uBar.Text = "uBar";
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(248)))));
            this.pnlButtons.Controls.Add(this.btnHome);
            this.pnlButtons.Controls.Add(this.btnCenter);
            this.pnlButtons.Controls.Add(this.btnEStop);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 744);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(1488, 52);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnHome
            // 
            this.btnHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(248)))));
            this.btnHome.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnHome.Location = new System.Drawing.Point(16, 8);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(96, 36);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "回零";
            this.btnHome.UseVisualStyleBackColor = false;
            // 
            // btnCenter
            // 
            this.btnCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(248)))));
            this.btnCenter.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnCenter.Location = new System.Drawing.Point(120, 8);
            this.btnCenter.Name = "btnCenter";
            this.btnCenter.Size = new System.Drawing.Size(96, 36);
            this.btnCenter.TabIndex = 1;
            this.btnCenter.Text = "居中";
            this.btnCenter.UseVisualStyleBackColor = false;
            // 
            // btnEStop
            // 
            this.btnEStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnEStop.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEStop.ForeColor = System.Drawing.Color.White;
            this.btnEStop.Location = new System.Drawing.Point(1384, 8);
            this.btnEStop.Name = "btnEStop";
            this.btnEStop.Size = new System.Drawing.Size(96, 36);
            this.btnEStop.TabIndex = 2;
            this.btnEStop.Text = "急停";
            this.btnEStop.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(36)))), ((int)(((byte)(48)))));
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Consolas", 10F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.lblStatus.Location = new System.Drawing.Point(0, 796);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(1488, 28);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "等待硬件连接...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 30;
            // 
            // HardwareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1488, 824);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(800, 480);
            this.Name = "HardwareForm";
            this.Text = "HardwareForm - 硬件调试 (XYZU 四轴)";
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
