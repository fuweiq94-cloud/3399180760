using System.Windows.Forms;

namespace XyzController
{
    /// <summary>
    /// 测试用窗体（验证 WpfHost 导航栏动态加页面的能力）。
    /// 实际项目中可替换为任何业务 Form。
    ///
    /// 结构说明：
    /// - 主文件（本文件）：业务逻辑 + 事件处理
    /// - Form1.Designer.cs：控件声明 + InitializeComponent（设计器维护）
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
}
