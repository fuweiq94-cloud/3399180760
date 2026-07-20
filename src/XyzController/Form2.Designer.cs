namespace XyzController
{
    partial class Form2
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
            this.joystickPad1 = new XyzController.Controls.JoystickPad();
            this.xyView1 = new XyzController.Controls.XYView();
            this.SuspendLayout();
            // 
            // joystickPad1
            // 
            this.joystickPad1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.joystickPad1.DeadZone = 0.25F;
            this.joystickPad1.Location = new System.Drawing.Point(744, 265);
            this.joystickPad1.Name = "joystickPad1";
            this.joystickPad1.Size = new System.Drawing.Size(385, 385);
            this.joystickPad1.TabIndex = 0;
            this.joystickPad1.Text = "joystickPad1";
            // 
            // xyView1
            // 
            this.xyView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.xyView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xyView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.xyView1.Location = new System.Drawing.Point(12, 12);
            this.xyView1.Name = "xyView1";
            this.xyView1.RangeMax = 100F;
            this.xyView1.RangeMin = -100F;
            this.xyView1.Size = new System.Drawing.Size(661, 638);
            this.xyView1.TabIndex = 1;
            this.xyView1.TargetX = 0F;
            this.xyView1.TargetY = 0F;
            this.xyView1.Text = "xyView1";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 890);
            this.Controls.Add(this.xyView1);
            this.Controls.Add(this.joystickPad1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.JoystickPad joystickPad1;
        private Controls.XYView xyView1;
    }
}