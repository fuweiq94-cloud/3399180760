using System.Windows;

namespace XyzController.WpfHost
{
    /// <summary>
    /// WPF 宿主应用入口。
    /// 关键：在 WPF 启动时初始化 WinForms 环境，
    /// 替代原 Program.cs 中 Application.EnableVisualStyles() 的职责。
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // ★ 关键：必须在创建任何 WinForms 控件之前调用
            // 否则嵌入的 WinForms 控件（TrackBar、Button 等）将显示经典样式
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            base.OnStartup(e);
        }
    }
}
