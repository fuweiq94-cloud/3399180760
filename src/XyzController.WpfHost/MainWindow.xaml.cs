using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using XyzController;

namespace XyzController.WpfHost
{
    /// <summary>
    /// WPF 主窗口，通过 WindowsFormsHost 承载 WinForms MainForm。
    /// 所有宿主配置集中在此文件，MainForm 源码零改动。
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainForm _form;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. 创建 MainForm 实例
            _form = new MainForm();

            // 2. ★ 关键：从顶层窗体降级为子控件
            _form.TopLevel = false;

            // 3. 去除窗口边框（子控件不需要标题栏和边框）
            _form.FormBorderStyle = FormBorderStyle.None;

            // 4. 填满 WindowsFormsHost 容器
            _form.Dock = DockStyle.Fill;

            // 5. 解除原 MinimumSize 约束（Designer 中设为 1279x662）
            //    避免嵌入后若容器较小导致裁剪或强制撑大
            _form.MinimumSize = new System.Drawing.Size(0, 0);

            // 6. 赋值给 WindowsFormsHost 的 Child 属性
            host.Child = _form;

            // 7. ★ 关键：显式设 Visible=true 触发 MainForm.OnLoad
            //    OnLoad 中 animTimer.Start() 才会执行，否则动画不启动
            _form.Show();

            // 8. 将焦点交给 WinForms 子控件，确保键盘交互生效
            //    MainForm 依赖 KeyPreview=true 处理 WASD/方向键
            _form.Focus();

            // 9. 键盘焦点增强：宿主获得焦点时转发给 WinForms 子控件
            host.GotFocus += (s, ev) => { if (_form != null && !_form.IsDisposed) _form.Focus(); };
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // ★ 关键：显式释放资源
            // MainForm.Dispose 会释放 animTimer、components、6 个 JogButton 的 Timer
            if (_form != null && !_form.IsDisposed)
            {
                _form.Close();
                _form.Dispose();
            }
        }
    }
}
