using System.Windows.Forms;

namespace XyzController.WpfHost
{
    /// <summary>
    /// 导航页面模型：封装一个要嵌入 WPF 导航框架的 WinForms 窗体及其选项卡标题。
    /// 调用方创建若干 WpfPage 实例后传入 WpfHostLauncher.Run(IList&lt;WpfPage&gt;)，
    /// 即可在 WPF 顶部导航栏中切换多个 WinForms 页面。
    /// </summary>
    public class WpfPage
    {
        /// <summary>导航栏上显示的选项卡标题。</summary>
        public string Title { get; private set; }

        /// <summary>要嵌入的 WinForms 窗体实例。</summary>
        public Form Content { get; private set; }

        /// <summary>
        /// 创建一个导航页面。
        /// </summary>
        /// <param name="title">选项卡标题（如"主控制器"）</param>
        /// <param name="content">WinForms 窗体实例（如 new MainForm()）</param>
        public WpfPage(string title, Form content)
        {
            Title = title;
            Content = content;
        }
    }
}
