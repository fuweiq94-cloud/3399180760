namespace XyzController
{
    /// <summary>
    /// Form1 的设计器文件。
    /// 纯声明式写法：所有控件属性逐行显式设置，全部使用字面常量，
    /// 以便在 VS2017 及以上 WinForms 设计器中正常打开。
    /// </summary>
    partial class Form1
    {
        /// <summary>必需的设计器变量。</summary>
        private System.ComponentModel.IContainer components = null;

        // —— 控件字段 ——
        private XyzController.Controls.DroLabel droLabel1;
        private XyzController.Controls.DroLabel droLabel2;
        private XyzController.Controls.XYView xyView2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private XyzController.Controls.XYView xyView1;
        private XyzController.Controls.ZBarView zBarView1;
        private System.Windows.Forms.Label lbl;

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
            this.lbl = new System.Windows.Forms.Label();
            this.droLabel1 = new XyzController.Controls.DroLabel();
            this.droLabel2 = new XyzController.Controls.DroLabel();
            this.xyView2 = new XyzController.Controls.XYView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.xyView1 = new XyzController.Controls.XYView();
            this.zBarView1 = new XyzController.Controls.ZBarView();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            //
            // lbl
            //
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F);
            this.lbl.Location = new System.Drawing.Point(30, 30);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(515, 72);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "这是 Form1 测试页面\n用来验证 WpfHost 导航栏动态注册功能";
            //
            // droLabel1
            //
            this.droLabel1.AlarmHigh = null;
            this.droLabel1.AlarmLow = null;
            this.droLabel1.AxisName = "X";
            this.droLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(28)))), ((int)(((byte)(40)))));
            this.droLabel1.Decimals = 3;
            this.droLabel1.FlashDuration = 200;
            this.droLabel1.Font = new System.Drawing.Font("Consolas", 22F, System.Drawing.FontStyle.Bold);
            this.droLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.droLabel1.Location = new System.Drawing.Point(692, 135);
            this.droLabel1.Name = "droLabel1";
            this.droLabel1.Size = new System.Drawing.Size(75, 23);
            this.droLabel1.TabIndex = 5;
            this.droLabel1.Text = "droLabel1";
            this.droLabel1.Unit = "mm";
            this.droLabel1.Value = 0F;
            //
            // droLabel2
            //
            this.droLabel2.AlarmHigh = null;
            this.droLabel2.AlarmLow = null;
            this.droLabel2.AxisName = "X";
            this.droLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(28)))), ((int)(((byte)(40)))));
            this.droLabel2.Decimals = 3;
            this.droLabel2.FlashDuration = 200;
            this.droLabel2.Font = new System.Drawing.Font("Consolas", 22F, System.Drawing.FontStyle.Bold);
            this.droLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.droLabel2.Location = new System.Drawing.Point(118, 203);
            this.droLabel2.Name = "droLabel2";
            this.droLabel2.Size = new System.Drawing.Size(75, 23);
            this.droLabel2.TabIndex = 6;
            this.droLabel2.Text = "droLabel2";
            this.droLabel2.Unit = "mm";
            this.droLabel2.Value = 0F;
            //
            // xyView2
            //
            this.xyView2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.xyView2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xyView2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.xyView2.Location = new System.Drawing.Point(57, 449);
            this.xyView2.Name = "xyView2";
            this.xyView2.RangeMax = 100F;
            this.xyView2.RangeMin = -100F;
            this.xyView2.Size = new System.Drawing.Size(75, 23);
            this.xyView2.TabIndex = 8;
            this.xyView2.TargetX = 0F;
            this.xyView2.TargetY = 0F;
            this.xyView2.Text = "xyView2";
            //
            // flowLayoutPanel1
            //
            this.flowLayoutPanel1.Controls.Add(this.zBarView1);
            this.flowLayoutPanel1.Controls.Add(this.xyView1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(24, 79);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1283, 636);
            this.flowLayoutPanel1.TabIndex = 9;
            //
            // xyView1
            //
            this.xyView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.xyView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xyView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.xyView1.Location = new System.Drawing.Point(3, 188);
            this.xyView1.Name = "xyView1";
            this.xyView1.RangeMax = 100F;
            this.xyView1.RangeMin = -100F;
            this.xyView1.Size = new System.Drawing.Size(951, 510);
            this.xyView1.TabIndex = 0;
            this.xyView1.TargetX = 0F;
            this.xyView1.TargetY = 0F;
            this.xyView1.Text = "xyView1";
            //
            // zBarView1
            //
            this.zBarView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.zBarView1.Location = new System.Drawing.Point(3, 3);
            this.zBarView1.Name = "zBarView1";
            this.zBarView1.RangeMax = 100F;
            this.zBarView1.RangeMin = -50F;
            this.zBarView1.Size = new System.Drawing.Size(359, 179);
            this.zBarView1.TabIndex = 10;
            this.zBarView1.TargetZ = 0F;
            this.zBarView1.Text = "zBarView1";
            //
            // Form1
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1454, 827);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.xyView2);
            this.Controls.Add(this.droLabel2);
            this.Controls.Add(this.droLabel1);
            this.Controls.Add(this.lbl);
            this.Name = "Form1";
            this.Text = "Form1 测试页面";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
