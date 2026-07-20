using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace XyzController.WpfHost
{
    /// <summary>
    /// WPF 宿主启动器：本 DLL 对外的唯一入口。
    /// 调用方只需传入任意 WinForms Form 实例，即可在 WPF 窗口中嵌入运行，
    /// 无需接触 WPF 宿主项目的内部源码（MainWindow / WindowsFormsHost 配置等）。
    /// 本程序集仅依赖 .NET 框架程序集，不引用任何业务项目。
    /// </summary>
    public static class WpfHostLauncher
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// 启动 WPF 宿主并嵌入指定的 WinForms 窗体（阻塞式，窗口关闭后返回）。
        /// </summary>
        /// <param name="form">要嵌入的 WinForms 窗体实例，例如 new MainForm()</param>
        /// <returns>WPF 应用程序退出码</returns>
        /// <remarks>必须在标记了 [STAThread] 的线程中调用（通常为 Program.Main）。</remarks>
        public static int Run(Form form)
        {
            if (form == null) throw new ArgumentNullException("form");

            // WinForms 与 WPF 都要求 STA 线程模型
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new InvalidOperationException(
                    "WpfHostLauncher.Run 必须在 [STAThread] 线程中调用（请为 Main 方法标注 [STAThread]）。");

            // ★ DPI 感知：必须在创建任何窗口之前调用。
            //    让 WPF 和 WinForms 都按实际 DPI 渲染，避免 Windows 位图缩放导致模糊/尺寸不一致。
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            // ★ 必须在创建任何 WinForms 控件之前调用，否则嵌入的控件显示经典样式。
            //    若调用方已提前调用（如 Bootstrapper），SetCompatibleTextRenderingDefault
            //    会因窗口已创建而抛出 InvalidOperationException，安全忽略即可。
            System.Windows.Forms.Application.EnableVisualStyles();
            try { System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false); }
            catch (InvalidOperationException) { /* 调用方已提前设置，忽略 */ }

            // 类库没有 App.xaml，按需创建 WPF Application（提供消息循环）。
            // 若宿主进程已有 Application（如被另一个 WPF 应用调用），则复用它。
            System.Windows.Application app = System.Windows.Application.Current;
            if (app == null)
                app = new System.Windows.Application();

            MainWindow window = new MainWindow(form);

            // ★ 类库中编程创建的 Application 显式 Show 窗口
            window.Show();
            return app.Run(window);
        }

        /// <summary>
        /// 泛型便捷重载：内部自行 new 一个 TForm 实例。
        /// 等同于 WpfHostLauncher.Run(new TForm())。
        /// </summary>
        /// <typeparam name="TForm">要嵌入的 WinForms 窗体类型，需有无参构造函数</typeparam>
        /// <returns>WPF 应用程序退出码</returns>
        public static int Run<TForm>() where TForm : Form, new()
        {
            return Run(new TForm());
        }

        /// <summary>
        /// 启动 WPF 宿主并显示多页面导航界面（阻塞式，窗口关闭后返回）。
        /// 顶部导航栏包含所有页面的选项卡，用户可自由切换。
        /// </summary>
        /// <param name="pages">页面列表，每个页面包含标题和 WinForms 窗体</param>
        /// <returns>WPF 应用程序退出码</returns>
        /// <remarks>必须在标记了 [STAThread] 的线程中调用。</remarks>
        public static int Run(IList<WpfPage> pages)
        {
            if (pages == null) throw new ArgumentNullException("pages");
            if (pages.Count == 0) throw new ArgumentException("页面列表不能为空。", "pages");

            // WinForms 与 WPF 都要求 STA 线程模型
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new InvalidOperationException(
                    "WpfHostLauncher.Run 必须在 [STAThread] 线程中调用（请为 Main 方法标注 [STAThread]）。");

            // ★ DPI 感知
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            // ★ WinForms 视觉样式
            System.Windows.Forms.Application.EnableVisualStyles();
            try { System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false); }
            catch (InvalidOperationException) { /* 调用方已提前设置，忽略 */ }

            // 创建 WPF Application
            System.Windows.Application app = System.Windows.Application.Current;
            if (app == null)
                app = new System.Windows.Application();

            MainWindow window = new MainWindow(pages);
            window.Show();
            return app.Run(window);
        }
    }
}
