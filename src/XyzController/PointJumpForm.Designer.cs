namespace XyzController
{
    /// <summary>
    /// 点位跳转窗体的设计器文件。
    /// 兼容 VS2017 WinForms 设计器：纯声明式逐行属性设置，无 Lambda / 集合初始化器。
    /// </summary>
    partial class PointJumpForm
    {
        private System.ComponentModel.IContainer components = null;

        // —— 布局容器 ——
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel pnlRight;

        // —— XY 视图 ——
        private XyzController.Controls.XYView xyView;

        // —— 实时坐标 DRO ——
        private System.Windows.Forms.GroupBox grpDro;
        private System.Windows.Forms.TableLayoutPanel tlpDro;
        private XyzController.Controls.DroLabel droX;
        private XyzController.Controls.DroLabel droY;
        private XyzController.Controls.DroLabel droZ;

        // —— 目标坐标输入 ——
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.TableLayoutPanel tlpTarget;
        private System.Windows.Forms.Label lblTargetX;
        private System.Windows.Forms.NumericUpDown nudTargetX;
        private System.Windows.Forms.Label lblTargetY;
        private System.Windows.Forms.NumericUpDown nudTargetY;
        private System.Windows.Forms.Label lblTargetZ;
        private System.Windows.Forms.NumericUpDown nudTargetZ;
        private System.Windows.Forms.Button btnJump;

        // —— 预设点位 ——
        private System.Windows.Forms.GroupBox grpPresets;
        private System.Windows.Forms.TableLayoutPanel tlpPresets;
        private System.Windows.Forms.ListView lvPresets;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colX;
        private System.Windows.Forms.ColumnHeader colY;
        private System.Windows.Forms.ColumnHeader colZ;
        private System.Windows.Forms.Button btnSavePos;
        private System.Windows.Forms.Button btnDeletePos;
        private System.Windows.Forms.Button btnGotoSelected;

        // —— 速度控制 ——
        private System.Windows.Forms.GroupBox grpSpeed;
        private System.Windows.Forms.TableLayoutPanel tlpSpeed;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TrackBar trbSpeed;

        // —— 状态栏 ——
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        // —— 动画定时器 ——
        private System.Windows.Forms.Timer animTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.xyView = new XyzController.Controls.XYView();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.grpDro = new System.Windows.Forms.GroupBox();
            this.tlpDro = new System.Windows.Forms.TableLayoutPanel();
            this.droX = new XyzController.Controls.DroLabel();
            this.droY = new XyzController.Controls.DroLabel();
            this.droZ = new XyzController.Controls.DroLabel();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.tlpTarget = new System.Windows.Forms.TableLayoutPanel();
            this.lblTargetX = new System.Windows.Forms.Label();
            this.nudTargetX = new System.Windows.Forms.NumericUpDown();
            this.lblTargetY = new System.Windows.Forms.Label();
            this.nudTargetY = new System.Windows.Forms.NumericUpDown();
            this.lblTargetZ = new System.Windows.Forms.Label();
            this.nudTargetZ = new System.Windows.Forms.NumericUpDown();
            this.btnJump = new System.Windows.Forms.Button();
            this.grpPresets = new System.Windows.Forms.GroupBox();
            this.tlpPresets = new System.Windows.Forms.TableLayoutPanel();
            this.lvPresets = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colX = new System.Windows.Forms.ColumnHeader();
            this.colY = new System.Windows.Forms.ColumnHeader();
            this.colZ = new System.Windows.Forms.ColumnHeader();
            this.btnSavePos = new System.Windows.Forms.Button();
            this.btnDeletePos = new System.Windows.Forms.Button();
            this.btnGotoSelected = new System.Windows.Forms.Button();
            this.grpSpeed = new System.Windows.Forms.GroupBox();
            this.tlpSpeed = new System.Windows.Forms.TableLayoutPanel();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.trbSpeed = new System.Windows.Forms.TrackBar();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.animTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.grpDro.SuspendLayout();
            this.tlpDro.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.tlpTarget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetZ)).BeginInit();
            this.grpPresets.SuspendLayout();
            this.tlpPresets.SuspendLayout();
            this.grpSpeed.SuspendLayout();
            this.tlpSpeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbSpeed)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.xyView);
            this.splitMain.Panel1.Padding = new System.Windows.Forms.Padding(8);
            this.splitMain.Panel1MinSize = 200;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pnlRight);
            this.splitMain.Panel2MinSize = 380;
            this.splitMain.Size = new System.Drawing.Size(1284, 724);
            this.splitMain.SplitterDistance = 900;
            this.splitMain.TabIndex = 0;
            // 
            // xyView
            // 
            this.xyView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.xyView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xyView.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xyView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.xyView.Location = new System.Drawing.Point(8, 8);
            this.xyView.Margin = new System.Windows.Forms.Padding(4);
            this.xyView.Name = "xyView";
            this.xyView.RangeMax = 100F;
            this.xyView.RangeMin = -100F;
            this.xyView.Size = new System.Drawing.Size(884, 708);
            this.xyView.TabIndex = 0;
            this.xyView.TargetX = 0F;
            this.xyView.TargetY = 0F;
            // 
            // pnlRight
            // 
            this.pnlRight.AutoScroll = true;
            this.pnlRight.Controls.Add(this.grpSpeed);
            this.pnlRight.Controls.Add(this.grpPresets);
            this.pnlRight.Controls.Add(this.grpTarget);
            this.pnlRight.Controls.Add(this.grpDro);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(8);
            this.pnlRight.Size = new System.Drawing.Size(380, 724);
            this.pnlRight.TabIndex = 0;
            // 
            // grpDro
            // 
            this.grpDro.Controls.Add(this.tlpDro);
            this.grpDro.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDro.Location = new System.Drawing.Point(8, 8);
            this.grpDro.Name = "grpDro";
            this.grpDro.Padding = new System.Windows.Forms.Padding(4);
            this.grpDro.Size = new System.Drawing.Size(364, 160);
            this.grpDro.TabIndex = 0;
            this.grpDro.TabStop = false;
            this.grpDro.Text = "实时坐标";
            // 
            // tlpDro
            // 
            this.tlpDro.ColumnCount = 1;
            this.tlpDro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDro.Controls.Add(this.droX, 0, 0);
            this.tlpDro.Controls.Add(this.droY, 0, 1);
            this.tlpDro.Controls.Add(this.droZ, 0, 2);
            this.tlpDro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDro.Location = new System.Drawing.Point(4, 28);
            this.tlpDro.Name = "tlpDro";
            this.tlpDro.RowCount = 3;
            this.tlpDro.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpDro.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpDro.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tlpDro.Size = new System.Drawing.Size(356, 128);
            this.tlpDro.TabIndex = 0;
            // 
            // droX
            // 
            this.droX.AxisName = "X";
            this.droX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(28)))), ((int)(((byte)(40)))));
            this.droX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.droX.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.droX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.droX.Location = new System.Drawing.Point(3, 3);
            this.droX.Margin = new System.Windows.Forms.Padding(3);
            this.droX.Name = "droX";
            this.droX.Size = new System.Drawing.Size(350, 36);
            this.droX.TabIndex = 0;
            this.droX.Unit = "mm";
            this.droX.Value = 0F;
            // 
            // droY
            // 
            this.droY.AxisName = "Y";
            this.droY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(28)))), ((int)(((byte)(40)))));
            this.droY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.droY.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.droY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.droY.Location = new System.Drawing.Point(3, 45);
            this.droY.Margin = new System.Windows.Forms.Padding(3);
            this.droY.Name = "droY";
            this.droY.Size = new System.Drawing.Size(350, 36);
            this.droY.TabIndex = 1;
            this.droY.Unit = "mm";
            this.droY.Value = 0F;
            // 
            // droZ
            // 
            this.droZ.AxisName = "Z";
            this.droZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(28)))), ((int)(((byte)(40)))));
            this.droZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.droZ.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.droZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.droZ.Location = new System.Drawing.Point(3, 87);
            this.droZ.Margin = new System.Windows.Forms.Padding(3);
            this.droZ.Name = "droZ";
            this.droZ.Size = new System.Drawing.Size(350, 38);
            this.droZ.TabIndex = 2;
            this.droZ.Unit = "mm";
            this.droZ.Value = 0F;
            // 
            // grpTarget
            // 
            this.grpTarget.Controls.Add(this.tlpTarget);
            this.grpTarget.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTarget.Location = new System.Drawing.Point(8, 168);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Padding = new System.Windows.Forms.Padding(4);
            this.grpTarget.Size = new System.Drawing.Size(364, 180);
            this.grpTarget.TabIndex = 1;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "目标坐标输入";
            // 
            // tlpTarget
            // 
            this.tlpTarget.ColumnCount = 2;
            this.tlpTarget.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpTarget.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTarget.Controls.Add(this.lblTargetX, 0, 0);
            this.tlpTarget.Controls.Add(this.nudTargetX, 1, 0);
            this.tlpTarget.Controls.Add(this.lblTargetY, 0, 1);
            this.tlpTarget.Controls.Add(this.nudTargetY, 1, 1);
            this.tlpTarget.Controls.Add(this.lblTargetZ, 0, 2);
            this.tlpTarget.Controls.Add(this.nudTargetZ, 1, 2);
            this.tlpTarget.Controls.Add(this.btnJump, 0, 3);
            this.tlpTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTarget.Location = new System.Drawing.Point(4, 28);
            this.tlpTarget.Name = "tlpTarget";
            this.tlpTarget.RowCount = 4;
            this.tlpTarget.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpTarget.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpTarget.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpTarget.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTarget.Size = new System.Drawing.Size(356, 148);
            this.tlpTarget.TabIndex = 0;
            // 
            // lblTargetX
            // 
            this.lblTargetX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetX.Location = new System.Drawing.Point(3, 0);
            this.lblTargetX.Name = "lblTargetX";
            this.lblTargetX.Size = new System.Drawing.Size(54, 36);
            this.lblTargetX.TabIndex = 0;
            this.lblTargetX.Text = "X:";
            this.lblTargetX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudTargetX
            // 
            this.nudTargetX.DecimalPlaces = 2;
            this.nudTargetX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudTargetX.Location = new System.Drawing.Point(63, 3);
            this.nudTargetX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTargetX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudTargetX.Name = "nudTargetX";
            this.nudTargetX.Size = new System.Drawing.Size(290, 31);
            this.nudTargetX.TabIndex = 1;
            this.nudTargetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTargetY
            // 
            this.lblTargetY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetY.Location = new System.Drawing.Point(3, 36);
            this.lblTargetY.Name = "lblTargetY";
            this.lblTargetY.Size = new System.Drawing.Size(54, 36);
            this.lblTargetY.TabIndex = 2;
            this.lblTargetY.Text = "Y:";
            this.lblTargetY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudTargetY
            // 
            this.nudTargetY.DecimalPlaces = 2;
            this.nudTargetY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudTargetY.Location = new System.Drawing.Point(63, 39);
            this.nudTargetY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTargetY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudTargetY.Name = "nudTargetY";
            this.nudTargetY.Size = new System.Drawing.Size(290, 31);
            this.nudTargetY.TabIndex = 3;
            this.nudTargetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTargetZ
            // 
            this.lblTargetZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetZ.Location = new System.Drawing.Point(3, 72);
            this.lblTargetZ.Name = "lblTargetZ";
            this.lblTargetZ.Size = new System.Drawing.Size(54, 36);
            this.lblTargetZ.TabIndex = 4;
            this.lblTargetZ.Text = "Z:";
            this.lblTargetZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudTargetZ
            // 
            this.nudTargetZ.DecimalPlaces = 2;
            this.nudTargetZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudTargetZ.Location = new System.Drawing.Point(63, 75);
            this.nudTargetZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTargetZ.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudTargetZ.Name = "nudTargetZ";
            this.nudTargetZ.Size = new System.Drawing.Size(290, 31);
            this.nudTargetZ.TabIndex = 5;
            this.nudTargetZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnJump
            // 
            this.btnJump.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(120)))), ((int)(((byte)(220)))));
            this.tlpTarget.SetColumnSpan(this.btnJump, 2);
            this.btnJump.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnJump.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJump.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnJump.ForeColor = System.Drawing.Color.White;
            this.btnJump.Location = new System.Drawing.Point(3, 111);
            this.btnJump.Name = "btnJump";
            this.btnJump.Size = new System.Drawing.Size(350, 34);
            this.btnJump.TabIndex = 6;
            this.btnJump.Text = "▶ 跳转到目标点位";
            this.btnJump.UseVisualStyleBackColor = false;
            // 
            // grpPresets
            // 
            this.grpPresets.Controls.Add(this.tlpPresets);
            this.grpPresets.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPresets.Location = new System.Drawing.Point(8, 348);
            this.grpPresets.Name = "grpPresets";
            this.grpPresets.Padding = new System.Windows.Forms.Padding(4);
            this.grpPresets.Size = new System.Drawing.Size(364, 240);
            this.grpPresets.TabIndex = 2;
            this.grpPresets.TabStop = false;
            this.grpPresets.Text = "预设点位";
            // 
            // tlpPresets
            // 
            this.tlpPresets.ColumnCount = 3;
            this.tlpPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tlpPresets.Controls.Add(this.lvPresets, 0, 0);
            this.tlpPresets.Controls.Add(this.btnSavePos, 0, 1);
            this.tlpPresets.Controls.Add(this.btnDeletePos, 1, 1);
            this.tlpPresets.Controls.Add(this.btnGotoSelected, 2, 1);
            this.tlpPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPresets.Location = new System.Drawing.Point(4, 28);
            this.tlpPresets.Name = "tlpPresets";
            this.tlpPresets.RowCount = 2;
            this.tlpPresets.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPresets.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpPresets.Size = new System.Drawing.Size(356, 208);
            this.tlpPresets.TabIndex = 0;
            // 
            // lvPresets
            // 
            this.lvPresets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colX,
            this.colY,
            this.colZ});
            this.tlpPresets.SetColumnSpan(this.lvPresets, 3);
            this.lvPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPresets.FullRowSelect = true;
            this.lvPresets.GridLines = true;
            this.lvPresets.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPresets.Location = new System.Drawing.Point(3, 3);
            this.lvPresets.MultiSelect = false;
            this.lvPresets.Name = "lvPresets";
            this.lvPresets.Size = new System.Drawing.Size(350, 166);
            this.lvPresets.TabIndex = 0;
            this.lvPresets.UseCompatibleStateImageBehavior = false;
            this.lvPresets.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "名称";
            this.colName.Width = 80;
            // 
            // colX
            // 
            this.colX.Text = "X";
            this.colX.Width = 80;
            // 
            // colY
            // 
            this.colY.Text = "Y";
            this.colY.Width = 80;
            // 
            // colZ
            // 
            this.colZ.Text = "Z";
            this.colZ.Width = 80;
            // 
            // btnSavePos
            // 
            this.btnSavePos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSavePos.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSavePos.Location = new System.Drawing.Point(3, 175);
            this.btnSavePos.Name = "btnSavePos";
            this.btnSavePos.Size = new System.Drawing.Size(112, 30);
            this.btnSavePos.TabIndex = 1;
            this.btnSavePos.Text = "保存当前";
            this.btnSavePos.UseVisualStyleBackColor = true;
            // 
            // btnDeletePos
            // 
            this.btnDeletePos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDeletePos.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDeletePos.Location = new System.Drawing.Point(121, 175);
            this.btnDeletePos.Name = "btnDeletePos";
            this.btnDeletePos.Size = new System.Drawing.Size(112, 30);
            this.btnDeletePos.TabIndex = 2;
            this.btnDeletePos.Text = "删除选中";
            this.btnDeletePos.UseVisualStyleBackColor = true;
            // 
            // btnGotoSelected
            // 
            this.btnGotoSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGotoSelected.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnGotoSelected.Location = new System.Drawing.Point(239, 175);
            this.btnGotoSelected.Name = "btnGotoSelected";
            this.btnGotoSelected.Size = new System.Drawing.Size(114, 30);
            this.btnGotoSelected.TabIndex = 3;
            this.btnGotoSelected.Text = "跳转选中";
            this.btnGotoSelected.UseVisualStyleBackColor = true;
            // 
            // grpSpeed
            // 
            this.grpSpeed.Controls.Add(this.tlpSpeed);
            this.grpSpeed.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSpeed.Location = new System.Drawing.Point(8, 588);
            this.grpSpeed.Name = "grpSpeed";
            this.grpSpeed.Padding = new System.Windows.Forms.Padding(4);
            this.grpSpeed.Size = new System.Drawing.Size(364, 80);
            this.grpSpeed.TabIndex = 3;
            this.grpSpeed.TabStop = false;
            this.grpSpeed.Text = "运动速度";
            // 
            // tlpSpeed
            // 
            this.tlpSpeed.ColumnCount = 2;
            this.tlpSpeed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlpSpeed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSpeed.Controls.Add(this.lblSpeed, 0, 0);
            this.tlpSpeed.Controls.Add(this.trbSpeed, 1, 0);
            this.tlpSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSpeed.Location = new System.Drawing.Point(4, 28);
            this.tlpSpeed.Name = "tlpSpeed";
            this.tlpSpeed.RowCount = 1;
            this.tlpSpeed.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSpeed.Size = new System.Drawing.Size(356, 48);
            this.tlpSpeed.TabIndex = 0;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpeed.Location = new System.Drawing.Point(3, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(64, 48);
            this.lblSpeed.TabIndex = 0;
            this.lblSpeed.Text = "速度：中";
            this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trbSpeed
            // 
            this.trbSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trbSpeed.Location = new System.Drawing.Point(73, 3);
            this.trbSpeed.Name = "trbSpeed";
            this.trbSpeed.Size = new System.Drawing.Size(280, 42);
            this.trbSpeed.TabIndex = 1;
            this.trbSpeed.TickFrequency = 10;
            this.trbSpeed.Value = 20;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 724);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1284, 29);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1269, 24);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "就绪";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // animTimer
            // 
            this.animTimer.Interval = 16;
            this.animTimer.Tick += new System.EventHandler(this.animTimer_Tick);
            // 
            // PointJumpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1284, 753);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "PointJumpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "点位跳转  ·  XYZ 工控机";
            this.splitMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.grpDro.ResumeLayout(false);
            this.tlpDro.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.tlpTarget.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetZ)).EndInit();
            this.grpPresets.ResumeLayout(false);
            this.tlpPresets.ResumeLayout(false);
            this.grpSpeed.ResumeLayout(false);
            this.tlpSpeed.ResumeLayout(false);
            this.tlpSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbSpeed)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
