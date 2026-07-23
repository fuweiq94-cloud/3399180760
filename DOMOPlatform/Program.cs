using System;
using System.Windows.Forms;

namespace DOMOPlatform
{
    /// <summary>
    /// DOMO 模拟平台程序入口。
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
