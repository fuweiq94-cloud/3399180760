using System;
using System.Windows;
using System.Windows.Forms;

namespace XyzController.WpfHost
{
    /// <summary>
    /// WPF 主窗口，通过 WindowsFormsHost 承载 WinForms 窗体。
    /// 本类为 DLL 内部实现细节（受 XAML 分部类限制必须为 public）；
    /// 外部调用方请使用公共入口 WpfHostLauncher.Run(...)。
    /// 所有宿主配置集中在 EmbedForm，被嵌入的 Form 源码零改动。
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Form _embeddedForm;
        private Form _form;

        public MainWindow(Form embeddedForm)
        {
            InitializeComponent();
            _embeddedForm = embeddedForm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EmbedForm(_embeddedForm);
        }

        /// <summary>
        /// 把任意 WinForms Form 嵌入 WindowsFormsHost 容器。
        /// 封装了“顶层窗体 → 子控件”的全部通用步骤：
        /// TopLevel=false、去边框、Dock=Fill、解除 MinimumSize、
        /// 挂到 host.Child、Show 触发 OnLoad、焦点转发保证键盘交互。
        /// </summary>
        private void EmbedForm(Form form)
        {
            _form = form;

            // 1. ★ 关键：从顶层窗体降级为子控件
            _form.TopLevel = false;

            // 2. 去除窗口边框（子控件不需要标题栏和边框）
            _form.FormBorderStyle = FormBorderStyle.None;

            // 3. 填满 WindowsFormsHost 容器
            _form.Dock = DockStyle.Fill;

            // 4. 解除原 MinimumSize 约束（如 MainForm Designer 中设为 1279x662）
            //    避免嵌入后若容器较小导致裁剪或强制撑大
            _form.MinimumSize = new System.Drawing.Size(0, 0);

            // 5. 赋值给 WindowsFormsHost 的 Child 属性
            host.Child = _form;

            // 6. ★ 关键：显式 Show 触发 Form.OnLoad
            //    （如 MainForm.OnLoad 中 animTimer.Start()，否则动画不启动）
            _form.Show();

            // 7. 将焦点交给 WinForms 子控件，确保键盘交互生效
            //    （MainForm 依赖 KeyPreview=true 处理 WASD/方向键）
            _form.Focus();

            // 8. 键盘焦点增强：宿主获得焦点时转发给 WinForms 子控件
            host.GotFocus += (s, ev) => { if (_form != null && !_form.IsDisposed) _form.Focus(); };
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // ★ 关键：显式释放资源
            // Form.Dispose 会连带释放其 components（如 animTimer、JogButton 的 Timer）
            if (_form != null && !_form.IsDisposed)
            {
                _form.Close();
                _form.Dispose();
            }
        }
    }
}
