using System;
using System.Windows.Forms;
using WpfHost;
namespace XyzController
{
    /// <summary>
    /// 程序入口。WinForms 应用必须使用 [STAThread]。
    /// </summary>
    internal static class Program
    {

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ★ 换窗体只改这一行：EmbedForm(new 你的Form());
            WpfHost.Window_Loaded(new MainForm());
        }

    }
}
