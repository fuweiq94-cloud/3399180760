using System.Drawing;
using System.Windows.Forms;

namespace XyzController
{
    /// <summary>
    /// 测试用窗体（验证 WpfHost 导航栏动态加页面的能力）。
    /// 实际项目中可替换为任何业务 Form。
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Form1 测试页面";
            this.BackColor = Color.FromArgb(245, 250, 255);

            Label lbl = new Label();
            lbl.Text = "这是 Form1 测试页面\n用来验证 WpfHost 导航栏动态注册功能";
            lbl.Font = new Font("Microsoft YaHei UI", 14F);
            lbl.AutoSize = true;
            lbl.Location = new Point(30, 30);

            this.Controls.Add(lbl);
        }
    }
}
