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

        // ===== 设计器尺寸保持（让窗口适配 Form，而非让 Form 适配窗口）=====
        // 每个页面「独立运行时」的客户区尺寸与最小尺寸（物理像素，已含 AutoScale 缩放），
        // 与 _pages 顺序一一对应；窗口尺寸始终跟随当前激活页面。
        private readonly List<System.Drawing.Size> _pageContentPx = new List<System.Drawing.Size>();
        private readonly List<System.Drawing.Size> _pageMinContentPx = new List<System.Drawing.Size>();
        private bool _hasFittedOnce;

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

            // ★ 按当前页面的设计器尺寸调整窗口，保证「VS 设计器里多大，WPF 宿主里就多大」。
            //   多页面模式由 SwitchPage 完成适配；此处只需处理单页面模式。
            if (_activeIndex < 0 && _pageContentPx.Count > 0)
                FitWindowToPage(0);
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

            // ★ 窗口尺寸跟随当前页面的设计器尺寸，
            //   避免小页面被拉伸到大页面窗口里导致 Anchor 布局走样。
            FitWindowToPage(index);

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
            // ★ 必须先于 TopLevel/Dock 修改捕获「独立运行时」的真实尺寸，
            //   否则 Dock=Fill 后 ClientSize 会被容器尺寸覆盖，设计意图丢失。
            RegisterDesiredSize(form);

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.MinimumSize = new System.Drawing.Size(0, 0);
            wfHost.Child = form;
            form.Show();
        }

        // ============== 设计器尺寸保持 ==============

        /// <summary>
        /// 捕获 Form 独立运行时的客户区尺寸与最小尺寸，并累计到全局期望值（多页面取最大）。
        /// 设计原理：WinForms 的 AutoScale（Dpi/Font）只在句柄创建时执行一次；
        /// 只有先让句柄创建，ClientSize / MinimumSize 才是「VS 设计器在当前机器上的真实呈现尺寸」。
        /// </summary>
        private void RegisterDesiredSize(Form form)
        {
            if (form == null || form.IsDisposed) return;

            System.Drawing.Size clientSize;
            System.Drawing.Size minimumSize;
            try
            {
                // 强制创建句柄以触发 AutoScale 缩放。
                // 不会显示窗口，Load/Shown 事件仍在 form.Show() 时才触发，业务行为不变。
                if (!form.IsHandleCreated)
                    form.CreateControl();

                clientSize = form.ClientSize;
                minimumSize = form.MinimumSize;
            }
            catch (Exception)
            {
                // 极端情况下句柄创建失败：退化为当前尺寸（等价于修复前的行为），不影响嵌入流程
                clientSize = form.ClientSize;
                minimumSize = form.MinimumSize;
            }

            _pageContentPx.Add(clientSize);
            _pageMinContentPx.Add(minimumSize);
        }

        /// <summary>
        /// 把 WPF 窗口适配到指定页面的「设计器尺寸」：
        /// 内容区尺寸 = 该页 Form 客户区尺寸（物理像素 ÷ 系统 DPI 缩放 = WPF 设备无关单位 DIU），
        /// 窗口尺寸   = 内容区 + 标题栏/边框/导航栏等实测非内容区。
        /// 同时把该页 Form 设计期的 MinimumSize 映射为窗口的 MinWidth/MinHeight。
        /// 切换页面时窗口随之变化，保证每一页都与 VS 设计器逐像素一致。
        /// </summary>
        private void FitWindowToPage(int index)
        {
            if (index < 0 || index >= _pageContentPx.Count) return;
            if (this.WindowState == WindowState.Maximized) return; // 用户主动最大化时尊重当前状态

            System.Drawing.Size contentPx = _pageContentPx[index];
            System.Drawing.Size minPx = _pageMinContentPx[index];
            if (contentPx.IsEmpty) return;

            double dpiScale = GetSystemDpiScale();
            if (dpiScale <= 0.0) dpiScale = 1.0;

            // 实测非内容区（标题栏 + 边框 + 导航栏）。首轮布局完成后 ActualWidth 有效。
            double chromeW = Math.Max(0.0, this.ActualWidth - contentArea.ActualWidth);
            double chromeH = Math.Max(0.0, this.ActualHeight - contentArea.ActualHeight);

            // 1) 目标尺寸：内容区恰好等于当前页 Form 客户区，不多不少
            double targetW = contentPx.Width / dpiScale + chromeW;
            double targetH = contentPx.Height / dpiScale + chromeH;

            // 2) 钳制到屏幕工作区（Form 比屏幕还大时，由 Form 内部 AutoScroll 兜底，
            //    与 Form 独立运行超出屏幕时的表现一致）
            System.Windows.Rect workArea = SystemParameters.WorkArea;
            if (targetW > workArea.Width) targetW = workArea.Width;
            if (targetH > workArea.Height) targetH = workArea.Height;

            // 3) 最小尺寸：尊重当前页设计期的 MinimumSize；先放宽约束再改尺寸，
            //    防止上一个页面的 MinWidth 把新尺寸钳住
            double minW = minPx.IsEmpty ? 0.0 : minPx.Width / dpiScale + chromeW;
            double minH = minPx.IsEmpty ? 0.0 : minPx.Height / dpiScale + chromeH;
            if (minW > targetW) minW = targetW;
            if (minH > targetH) minH = targetH;
            this.MinWidth = minW;
            this.MinHeight = minH;

            // 记录变化前的窗口中心，尺寸调整后保持中心不漂移（首次则居中到屏幕）
            double oldCenterX = this.Left + this.Width / 2;
            double oldCenterY = this.Top + this.Height / 2;

            this.Width = targetW;
            this.Height = targetH;

            double centerX, centerY;
            if (_hasFittedOnce && !double.IsNaN(oldCenterX) && !double.IsNaN(oldCenterY))
            {
                centerX = oldCenterX;
                centerY = oldCenterY;
            }
            else
            {
                centerX = workArea.Left + workArea.Width / 2;
                centerY = workArea.Top + workArea.Height / 2;
                _hasFittedOnce = true;
            }

            double left = centerX - this.Width / 2;
            double top = centerY - this.Height / 2;
            // 保证窗口完整落在工作区内
            if (left < workArea.Left) left = workArea.Left;
            if (top < workArea.Top) top = workArea.Top;
            if (left + this.Width > workArea.Right) left = workArea.Right - this.Width;
            if (top + this.Height > workArea.Bottom) top = workArea.Bottom - this.Height;
            this.Left = left;
            this.Top = top;
        }

        /// <summary>
        /// 系统 DPI 缩放系数（物理像素 → WPF DIU 的换算倍率）。
        /// 进程已在 WpfHostLauncher.Run 开头声明 SetProcessDPIAware，此处读到的是真实系统 DPI。
        /// </summary>
        private static double GetSystemDpiScale()
        {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                return g.DpiX / 96.0;
            }
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
