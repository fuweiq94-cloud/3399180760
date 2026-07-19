using System;
using XyzController.WpfHost;

namespace XyzController
{
    /// <summary>
    /// 程序入口。通过 WPF 宿主接口 DLL（XyzController.WpfHost）启动界面，
    /// 调用方完全不需要接触 WPF 源码与 WPF 类型。WinForms/WPF 都要求 [STAThread]。
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // ★ 换窗体只改这一行：WpfHostLauncher.Run(new 你的Form());
            WpfHostLauncher.Run(new MainForm());
        }
    }
}
