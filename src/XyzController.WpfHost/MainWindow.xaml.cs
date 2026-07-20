using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace XyzController.WpfHost
{
    /// <summary>
    /// WPF 主窗口，通过 WindowsFormsHost 承载 WinForms 窗体。
    /// 支持多页面导航：顶部导航栏切换不同 WinForms 页面。
    /// 本类为 DLL 内部实现细节（受 XAML 分部类限制必须为 public）；
    /// 外部调用方请使用公共入口 WpfHostLauncher.Run(...)。
    /// </summary>
    public partial class MainWindow : Window
    {
        // ===== 多页面导航 =====
        private readonly List<WpfPage> _pages = new List<WpfPage>();
        private readonly List<WindowsFormsHost> _hosts = new List<WindowsFormsHost>();
        private readonly List<System.Windows.Controls.Button> _navButtons = new List<System.Windows.Controls.Button>();
        private int _activeIndex = -1;

        // 向后兼容：单页面模式
        private readonly Form _embeddedForm;

        /// <summary>
        /// 单页面构造（向后兼容）。
        /// </summary>
        public MainWindow(Form embeddedForm)
        {
            InitializeComponent();
            _embeddedForm = embeddedForm;
        }

        /// <summary>
        /// 多页面构造：传入页面列表，自动生成导航栏。
        /// </summary>
        public MainWindow(IList<WpfPage> pages)
        {
            InitializeComponent();
            if (pages != null)
                _pages.AddRange(pages);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_pages.Count > 0)
            {
                BuildNavigation();
            }
            else if (_embeddedForm != null)
            {
                // 向后兼容：单页面无导航栏，直接嵌入
                EmbedForm(_embeddedForm);
            }
        }

        // ============== 多页面导航 ==============

        /// <summary>
        /// 构建导航栏按钮和内容区 WindowsFormsHost。
        /// </summary>
        private void BuildNavigation()
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                WpfPage page = _pages[i];

                // 1. 创建导航按钮
                System.Windows.Controls.Button btn = CreateNavButton(page.Title, i);
                navBar.Children.Add(btn);
                _navButtons.Add(btn);

                // 2. 创建 WindowsFormsHost 并嵌入 Form
                WindowsFormsHost wfHost = new WindowsFormsHost();
                wfHost.Visibility = Visibility.Collapsed;
                contentArea.Children.Add(wfHost);
                _hosts.Add(wfHost);

                EmbedFormIntoHost(page.Content, wfHost);
            }

            // 3. 默认激活第一页
            if (_pages.Count > 0)
                SwitchPage(0);
        }

        /// <summary>
        /// 创建导航栏选项卡按钮。
        /// </summary>
        private System.Windows.Controls.Button CreateNavButton(string title, int index)
        {
            System.Windows.Controls.Button btn = new System.Windows.Controls.Button();
            btn.Content = title;
            btn.Tag = index;
            btn.Padding = new Thickness(16, 6, 16, 6);
            btn.Margin = new Thickness(0, 0, 2, 0);
            btn.Cursor = System.Windows.Input.Cursors.Hand;
            btn.FontSize = 13;
            btn.FontFamily = new FontFamily("Microsoft YaHei UI");

            // 默认样式（未选中）
            ApplyNavButtonInactiveStyle(btn);

            btn.Click += new RoutedEventHandler(NavButton_Click);
            return btn;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn == null || btn.Tag == null) return;
            int index = (int)btn.Tag;
            SwitchPage(index);
        }

        /// <summary>
        /// 切换到指定索引的页面。
        /// </summary>
        private void SwitchPage(int index)
        {
            if (index < 0 || index >= _pages.Count) return;
            if (index == _activeIndex) return;

            // 隐藏旧页面
            if (_activeIndex >= 0 && _activeIndex < _hosts.Count)
            {
                _hosts[_activeIndex].Visibility = Visibility.Collapsed;
                ApplyNavButtonInactiveStyle(_navButtons[_activeIndex]);
            }

            // 显示新页面
            _hosts[index].Visibility = Visibility.Visible;
            ApplyNavButtonActiveStyle(_navButtons[index]);
            _activeIndex = index;

            // 将焦点交给当前 WinForms 页面
            Form form = _pages[index].Content;
            if (form != null && !form.IsDisposed)
                form.Focus();
        }

        /// <summary>选中状态按钮样式。</summary>
        private void ApplyNavButtonActiveStyle(System.Windows.Controls.Button btn)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)); // 蓝色
            btn.Foreground = Brushes.White;
            btn.FontWeight = FontWeights.Bold;
        }

        /// <summary>未选中状态按钮样式。</summary>
        private void ApplyNavButtonInactiveStyle(System.Windows.Controls.Button btn)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x42)); // 深灰
            btn.Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)); // 浅灰文字
            btn.FontWeight = FontWeights.Normal;
        }

        // ============== WinForms 嵌入 ==============

        /// <summary>
        /// 把 WinForms Form 嵌入指定的 WindowsFormsHost 容器。
        /// </summary>
        private void EmbedFormIntoHost(Form form, WindowsFormsHost wfHost)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.MinimumSize = new System.Drawing.Size(0, 0);
            wfHost.Child = form;
            form.Show();
        }

        /// <summary>
        /// 向后兼容：单页面嵌入（无导航栏模式）。
        /// </summary>
        private void EmbedForm(Form form)
        {
            WindowsFormsHost wfHost = new WindowsFormsHost();
            contentArea.Children.Add(wfHost);
            _hosts.Add(wfHost);

            EmbedFormIntoHost(form, wfHost);

            form.Focus();
            wfHost.GotFocus += delegate { if (!form.IsDisposed) form.Focus(); };
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // 释放所有嵌入的 WinForms 窗体
            for (int i = 0; i < _pages.Count; i++)
            {
                Form form = _pages[i].Content;
                if (form != null && !form.IsDisposed)
                {
                    form.Close();
                    form.Dispose();
                }
            }

            // 向后兼容：单页面模式
            if (_embeddedForm != null && !_embeddedForm.IsDisposed)
            {
                _embeddedForm.Close();
                _embeddedForm.Dispose();
            }
        }
    }
}
