using System;
using System.Windows.Forms;
using XyzController.WpfHost;

namespace XyzController
{
    /// <summary>
    /// 程序入口。通过 WPF 宿主接口 DLL（XyzController.WpfHost）启动界面，
    /// 调用方完全不需要接触 WPF 源码与 WPF 类型。WinForms/WPF 都要求 [STAThread]。
    /// 
    /// 命令行参数：
    ///   （无参数）或 main  → 主控制器界面 MainForm
    ///   jump              → 点位跳转界面 PointJumpForm
    ///   trajectory        → 运动轨迹查看界面 TrajectoryViewForm
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Form form = CreateForm(args);
            WpfHostLauncher.Run(form);
        }

        /// <summary>
        /// 根据命令行参数创建对应的窗体实例。
        /// </summary>
        private static Form CreateForm(string[] args)
        {
            string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "main";

            switch (mode)
            {
                case "jump":
                    return new PointJumpForm();
                case "trajectory":
                    return new TrajectoryViewForm();
                case "main":
                default:
                    return new MainForm();
            }
        }
    }
}
