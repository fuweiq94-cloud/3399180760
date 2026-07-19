using System;
using System.Windows.Forms;

namespace XyzController
{
    /// <summary>
    /// 程序入口。WinForms 应用必须使用 [STAThread]。
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
