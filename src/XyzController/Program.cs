using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XyzController.WpfHost;

namespace XyzController
{
    /// <summary>
    /// 程序入口。通过 WPF 宿主接口 DLL（XyzController.WpfHost）启动界面，
    /// 调用方完全不需要接触 WPF 源码与 WPF 类型。WinForms/WPF 都要求 [STAThread]。
    /// 
    /// 命令行参数：
    ///   （无参数）或 nav   → 多页面导航模式（包含所有页面）
    ///   main              → 仅主控制器界面 MainForm
    ///   jump              → 仅点位跳转界面 PointJumpForm
    ///   trajectory        → 仅运动轨迹查看界面 TrajectoryViewForm
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "nav";

            switch (mode)
            {
                case "main":
                    WpfHostLauncher.Run(new MainForm());
                    break;
                case "jump":
                    WpfHostLauncher.Run(new PointJumpForm());
                    break;
                case "trajectory":
                    WpfHostLauncher.Run(new TrajectoryViewForm());
                    break;
                case "nav":
                default:
                    RunMultiPage();
                    break;
            }
        }

        /// <summary>
        /// 多页面导航模式：将所有功能页面注册到 WPF 导航框架中。
        /// </summary>
        private static void RunMultiPage()
        {
            List<WpfPage> pages = new List<WpfPage>();
            pages.Add(new WpfPage("主控制器", new MainForm()));
            pages.Add(new WpfPage("点位跳转", new PointJumpForm()));
            pages.Add(new WpfPage("运动轨迹", new TrajectoryViewForm()));
            pages.Add(new WpfPage("sadf", new Form1()));
            WpfHostLauncher.Run(pages);
        }
    }
}
