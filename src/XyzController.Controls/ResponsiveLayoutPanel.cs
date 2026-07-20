using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace XyzController.Controls
{
    /// <summary>
    /// 行高策略：决定响应式网格中每一行的高度如何确定。
    /// </summary>
    public enum ResponsiveRowMode
    {
        /// <summary>所有行使用统一固定高度（<see cref="ResponsiveLayoutPanel.UniformRowHeight"/>，按 DPI 缩放）。</summary>
        Uniform,

        /// <summary>每行高度取该行内子控件首选高度（GetPreferredSize）的最大值，内容自适应。</summary>
        AutoHeight,

        /// <summary>所有行等分容器可见高度（纵向的 “* 行” 铺满效果，永不出现滚动条）。</summary>
        Fill
    }

    /// <summary>
    /// 响应式布局容器。
    ///
    /// 设计目标：配合 WPF 宿主（WindowsFormsHost + Grid 的 “*” 行 + Dock.Fill + DPI 感知）使用，
    /// 使内部承载的任意 WinForms 控件随宿主窗口尺寸变化自动重新排列（resize / re-layout）。
    ///
    /// 用法：
    /// <code>
    ///   ResponsiveLayoutPanel panel = new ResponsiveLayoutPanel();
    ///   panel.Dock = DockStyle.Fill;          // 铺满窗体 → 随 WindowsFormsHost 一起缩放
    ///   panel.MinCellWidth = 240;             // 每个单元最小逻辑宽度，驱动“断点”换列
    ///   panel.CellSpacing = 10;               // 单元间距（逻辑像素，自动按 DPI 缩放）
    ///   panel.Controls.Add(anyControl);       // 放入任意控件即可
    ///   panel.SetColumnSpan(bigControl, 2);   // 可选：某控件横跨多列（类似 WPF Grid.ColumnSpan）
    ///   form.Controls.Add(panel);
    /// </code>
    ///
    /// 布局算法（每次尺寸变化 / 增删子控件时自动执行）：
    /// 1. 按可见宽度与 <see cref="MinCellWidth"/> 计算当前应显示的列数（响应式断点）。
    /// 2. 列宽在可用宽度内等分（“* 列”比例铺满），累积取整避免像素缝隙。
    /// 3. 子控件从左到右、从上到下依次排入网格，超出则换行。
    /// 4. 行高按 <see cref="RowMode"/> 决定（统一 / 内容自适应 / 等分铺满）。
    ///
    /// 兼容性：本控件通过自定义 <see cref="LayoutEngine"/> 接管布局，
    /// 子控件的 Dock / Anchor 会被忽略（由本容器统一定位），无需业务方手动处理。
    /// </summary>
    [ProvideProperty("ColumnSpan", typeof(Control))]
    [Description("响应式布局容器：内部控件随容器尺寸自动重排，配合 WPF 宿主实现界面自适应。")]
    public class ResponsiveLayoutPanel : Panel, IExtenderProvider
    {
        // ===== 布局参数（均为逻辑像素，运行时按 DPI 缩放）=====
        private int _minCellWidth = 220;
        private int _cellSpacing = 8;
        private int _maxColumns = 0;            // 0 = 不限制
        private int _uniformRowHeight = 120;
        private ResponsiveRowMode _rowMode = ResponsiveRowMode.Uniform;

        // 每个子控件的列跨度（类似 WPF Grid.ColumnSpan / “*” 比重）。默认 1，故仅存储 >1 的项。
        private readonly Dictionary<Control, int> _columnSpans = new Dictionary<Control, int>();

        // 子控件加入时记录的“自然高度”：AutoHeight 行高按它计算，避免布局把控件拉高后
        // 下一轮又以被拉高的高度重新测量而导致行高逐轮增长（漂移）。
        private readonly Dictionary<Control, int> _naturalHeights = new Dictionary<Control, int>();

        // 自定义布局引擎实例（WinForms 在每次 PerformLayout 时回调它）。
        private readonly ResponsiveLayoutEngine _engine;

        // 布局重入保护 + 当前 DPI 缩放比。
        private bool _inLayout;
        private float _dpiScale = 1f;

        public ResponsiveLayoutPanel()
        {
            _engine = new ResponsiveLayoutEngine();

            // 双缓冲，减少重排时的闪烁；容器随尺寸重绘。
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.ResizeRedraw, true);

            // 允许纵向溢出时滚动（Fill 模式因铺满而永不溢出，不会出现滚动条）。
            AutoScroll = true;
        }

        /// <summary>返回自定义布局引擎，接管本容器的所有子控件排布。</summary>
        public override LayoutEngine LayoutEngine
        {
            get { return _engine; }
        }

        // ============================================================
        //  公开布局属性
        // ============================================================

        /// <summary>
        /// 单元格最小逻辑宽度（像素）。可用宽度能容纳几个该宽度就显示几列，
        /// 从而实现“窗口变宽 → 列变多、窗口变窄 → 列变少”的响应式断点效果。
        /// </summary>
        [Category("Responsive Layout")]
        [DefaultValue(220)]
        [Description("单元格最小逻辑宽度（像素，按 DPI 缩放）。决定容器在不同宽度下显示的列数。")]
        public int MinCellWidth
        {
            get { return _minCellWidth; }
            set
            {
                if (value < 1) value = 1;
                if (_minCellWidth != value)
                {
                    _minCellWidth = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>单元格之间的水平/垂直间距（逻辑像素，按 DPI 缩放）。</summary>
        [Category("Responsive Layout")]
        [DefaultValue(8)]
        [Description("单元格之间的水平与垂直间距（逻辑像素，按 DPI 缩放）。")]
        public int CellSpacing
        {
            get { return _cellSpacing; }
            set
            {
                if (value < 0) value = 0;
                if (_cellSpacing != value)
                {
                    _cellSpacing = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>最大列数上限。0 表示不限制（仅由宽度和 MinCellWidth 决定）。</summary>
        [Category("Responsive Layout")]
        [DefaultValue(0)]
        [Description("最大列数上限。0 = 不限制。")]
        public int MaxColumns
        {
            get { return _maxColumns; }
            set
            {
                if (value < 0) value = 0;
                if (_maxColumns != value)
                {
                    _maxColumns = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>行高策略。见 <see cref="ResponsiveRowMode"/>。</summary>
        [Category("Responsive Layout")]
        [DefaultValue(ResponsiveRowMode.Uniform)]
        [Description("行高策略：Uniform=统一高度；AutoHeight=按内容自适应；Fill=等分铺满容器高度。")]
        public ResponsiveRowMode RowMode
        {
            get { return _rowMode; }
            set
            {
                if (_rowMode != value)
                {
                    _rowMode = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>Uniform 模式下每行的逻辑高度（像素，按 DPI 缩放）。</summary>
        [Category("Responsive Layout")]
        [DefaultValue(120)]
        [Description("Uniform 行高模式下每行的逻辑高度（像素，按 DPI 缩放）。")]
        public int UniformRowHeight
        {
            get { return _uniformRowHeight; }
            set
            {
                if (value < 1) value = 1;
                if (_uniformRowHeight != value)
                {
                    _uniformRowHeight = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        // ============================================================
        //  ColumnSpan 扩展属性（IExtenderProvider）
        //  既支持设计器（子控件属性网格中出现 “ColumnSpan on responsivePanel1”），
        //  也支持代码调用 SetColumnSpan / GetColumnSpan。
        // ============================================================

        bool IExtenderProvider.CanExtend(object extendee)
        {
            Control c = extendee as Control;
            return c != null && c.Parent == this;
        }

        /// <summary>获取控件在响应式网格中横跨的列数（默认 1）。</summary>
        [Category("Responsive Layout")]
        [DefaultValue(1)]
        [Description("控件在响应式网格中横跨的列数（类似 WPF Grid.ColumnSpan）。")]
        public int GetColumnSpan(Control control)
        {
            if (control == null) return 1;
            int span;
            return _columnSpans.TryGetValue(control, out span) ? span : 1;
        }

        /// <summary>设置控件横跨的列数（>=1）。修改后自动重排。</summary>
        public void SetColumnSpan(Control control, int value)
        {
            if (control == null) return;
            if (value < 1) value = 1;

            if (value == 1)
                _columnSpans.Remove(control);
            else
                _columnSpans[control] = value;

            PerformLayout();
            Invalidate();
        }

        /// <summary>手动请求重新布局（一般无需调用，尺寸变化会自动触发）。</summary>
        public void Relayout()
        {
            PerformLayout();
            Invalidate();
        }

        // ============================================================
        //  子控件增删 → 触发重排 / 清理跨列记录
        // ============================================================

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control != null)
                _naturalHeights[e.Control] = e.Control.Height;   // 记录设计高度，供 AutoHeight 使用
            PerformLayout();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (e.Control != null)
            {
                _columnSpans.Remove(e.Control);
                _naturalHeights.Remove(e.Control);
            }
            base.OnControlRemoved(e);
            PerformLayout();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // 句柄创建后 ClientSize 才可靠，补一次布局。
            PerformLayout();
        }

        // ============================================================
        //  核心布局逻辑（由 ResponsiveLayoutEngine 调用）
        // ============================================================

        /// <summary>
        /// 执行一次响应式重排。计算列数 → 分行 → 逐格定位子控件。
        /// </summary>
        internal void DoResponsiveLayout()
        {
            if (_inLayout) return;
            if (!IsHandleCreated && !DesignMode) return;

            int usableWidth = ClientSize.Width - Padding.Horizontal;
            if (usableWidth <= 0) return;

            _inLayout = true;
            SuspendLayout();
            try
            {
                _dpiScale = GetDpiScale();
                int spacing = ScaleValue(_cellSpacing);
                int minCell = ScaleValue(_minCellWidth);
                if (minCell < 1) minCell = 1;

                // 1) 收集可见子控件，并统计跨列信息。
                List<Control> items = new List<Control>();
                int totalSpan = 0;
                int maxSpan = 1;
                foreach (Control c in Controls)
                {
                    if (c == null || !c.Visible) continue;
                    int s = GetColumnSpan(c);
                    if (s < 1) s = 1;
                    items.Add(c);
                    totalSpan += s;
                    if (s > maxSpan) maxSpan = s;
                }

                if (items.Count == 0)
                {
                    AutoScrollMinSize = Size.Empty;
                    return;
                }

                // 2) 计算列数（响应式断点）。
                int cols = (int)Math.Floor((usableWidth + spacing) / (double)(minCell + spacing));
                if (cols < 1) cols = 1;
                if (_maxColumns > 0 && cols > _maxColumns) cols = _maxColumns;
                if (cols > totalSpan) cols = totalSpan;   // 内容不足时不产生多余空列
                if (cols < maxSpan) cols = maxSpan;        // 保证最宽的项能容纳
                if (cols < 1) cols = 1;

                // 3) 列的浮点步长（累积取整消除像素缝隙，实现 “* 列” 等分铺满）。
                double unit = (usableWidth + spacing) / (double)cols;

                // 4) 分行：从左到右排入，超出列数则换行。
                List<List<Cell>> rows = BuildRows(items, cols);

                int startLeft = Padding.Left + AutoScrollPosition.X;
                int startTop = Padding.Top + AutoScrollPosition.Y;

                if (_rowMode == ResponsiveRowMode.Fill)
                {
                    LayoutFillMode(rows, startLeft, startTop, unit, spacing);
                    AutoScrollMinSize = Size.Empty;
                }
                else
                {
                    LayoutStackedMode(rows, startLeft, startTop, unit, spacing);
                }
            }
            finally
            {
                ResumeLayout(false);
                _inLayout = false;
            }
        }

        /// <summary>把可见控件按列容量分配到若干行。</summary>
        private List<List<Cell>> BuildRows(List<Control> items, int cols)
        {
            List<List<Cell>> rows = new List<List<Cell>>();
            List<Cell> row = new List<Cell>();
            int col = 0;

            for (int i = 0; i < items.Count; i++)
            {
                Control c = items[i];
                int span = GetColumnSpan(c);
                if (span < 1) span = 1;
                if (span > cols) span = cols;

                if (col + span > cols)
                {
                    rows.Add(row);
                    row = new List<Cell>();
                    col = 0;
                }

                Cell cell;
                cell.Control = c;
                cell.Col = col;
                cell.Span = span;
                row.Add(cell);
                col += span;
            }

            if (row.Count > 0) rows.Add(row);
            return rows;
        }

        /// <summary>Uniform / AutoHeight：逐行堆叠，超出高度时可纵向滚动。</summary>
        private void LayoutStackedMode(List<List<Cell>> rows, int startLeft, int startTop,
                                       double unit, int spacing)
        {
            int[] rowHeights = new int[rows.Count];
            int sumH = 0;
            for (int r = 0; r < rows.Count; r++)
            {
                rowHeights[r] = ComputeRowHeight(rows[r], unit, spacing);
                sumH += rowHeights[r];
            }

            int y = startTop;
            for (int r = 0; r < rows.Count; r++)
            {
                PlaceRow(rows[r], startLeft, y, unit, spacing, rowHeights[r]);
                y += rowHeights[r] + spacing;
            }

            // 虚拟内容高度（不含滚动偏移）→ 决定是否出现纵向滚动条。
            int contentHeight = Padding.Top + sumH + Padding.Bottom;
            if (rows.Count > 1) contentHeight += spacing * (rows.Count - 1);
            AutoScrollMinSize = new Size(0, contentHeight);
        }

        /// <summary>Fill：所有行等分可见高度，纵向铺满（真正的 “* 行”）。</summary>
        private void LayoutFillMode(List<List<Cell>> rows, int startLeft, int startTop,
                                    double unit, int spacing)
        {
            int usableHeight = ClientSize.Height - Padding.Vertical;
            if (usableHeight < rows.Count) usableHeight = rows.Count;

            double rowUnit = (usableHeight + spacing) / (double)rows.Count;
            for (int r = 0; r < rows.Count; r++)
            {
                int yTop = startTop + (int)Math.Round(r * rowUnit);
                int yBot = startTop + (int)Math.Round((r + 1) * rowUnit) - spacing;
                int h = yBot - yTop;
                if (h < 1) h = 1;
                PlaceRow(rows[r], startLeft, yTop, unit, spacing, h);
            }
        }

        /// <summary>把一行内的所有单元定位到指定 y / 高度。</summary>
        private void PlaceRow(List<Cell> row, int startLeft, int top,
                              double unit, int spacing, int height)
        {
            for (int i = 0; i < row.Count; i++)
            {
                Cell cell = row[i];
                int left = startLeft + (int)Math.Round(cell.Col * unit);
                int right = startLeft + (int)Math.Round((cell.Col + cell.Span) * unit) - spacing;
                int w = right - left;
                if (w < 1) w = 1;
                cell.Control.SetBounds(left, top, w, height);
            }
        }

        /// <summary>按行高策略计算某一行的高度。</summary>
        private int ComputeRowHeight(List<Cell> row, double unit, int spacing)
        {
            if (_rowMode == ResponsiveRowMode.Uniform)
                return Math.Max(1, ScaleValue(_uniformRowHeight));

            // AutoHeight：取该行子控件首选高度的最大值。
            int max = 0;
            for (int i = 0; i < row.Count; i++)
            {
                Cell cell = row[i];
                int w = (int)Math.Round((cell.Col + cell.Span) * unit)
                        - (int)Math.Round(cell.Col * unit) - spacing;
                if (w < 1) w = 1;
                int hh = MeasureHeight(cell.Control, w);
                if (hh > max) max = hh;
            }
            if (max < 1) max = ScaleValue(_uniformRowHeight);
            return max;
        }

        /// <summary>测量控件在给定宽度下用于 AutoHeight 的期望高度。</summary>
        /// <remarks>
        /// AutoSize 控件用 GetPreferredSize 反映真实内容高度；
        /// 非 AutoSize 控件用加入时记录的“自然高度”（不受本容器后续拉伸影响），
        /// 从而保证多轮布局稳定、不漂移。
        /// </remarks>
        private int MeasureHeight(Control c, int width)
        {
            if (c.AutoSize)
            {
                try
                {
                    Size pref = c.GetPreferredSize(new Size(width, 0));
                    if (pref.Height > 0) return pref.Height;
                }
                catch
                {
                    // 个别控件测量异常时忽略，走下方回退逻辑。
                }
            }

            int natural;
            if (_naturalHeights.TryGetValue(c, out natural) && natural > 0)
                return natural;
            if (c.Height > 0) return c.Height;
            return ScaleValue(_uniformRowHeight);
        }

        // ============================================================
        //  DPI 感知
        // ============================================================

        /// <summary>
        /// 读取当前设备 DPI 缩放比（96 DPI 基准）。
        /// 进程为 DPI 感知（宿主启动时已调用 SetProcessDPIAware）时返回真实缩放，
        /// 使各断点/间距在不同 DPI 下保持一致的物理观感。
        /// </summary>
        private float GetDpiScale()
        {
            try
            {
                using (Graphics g = CreateGraphics())
                {
                    if (g.DpiX > 0) return g.DpiX / 96f;
                }
            }
            catch
            {
                // 句柄尚未就绪等情况，退回 1:1。
            }
            return 1f;
        }

        /// <summary>把逻辑像素按当前 DPI 缩放为设备像素。</summary>
        private int ScaleValue(int logical)
        {
            return (int)Math.Round(logical * _dpiScale);
        }

        /// <summary>一个网格单元的占位记录：控件 + 起始列 + 跨列数。</summary>
        private struct Cell
        {
            public Control Control;
            public int Col;
            public int Span;
        }

        /// <summary>
        /// 响应式布局引擎：WinForms 每次 PerformLayout（尺寸变化、增删子控件等）都会回调，
        /// 把实际排布委托给容器的 <see cref="ResponsiveLayoutPanel.DoResponsiveLayout"/>。
        /// </summary>
        private sealed class ResponsiveLayoutEngine : LayoutEngine
        {
            public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
            {
                ResponsiveLayoutPanel panel = container as ResponsiveLayoutPanel;
                if (panel != null)
                    panel.DoResponsiveLayout();

                // 返回 false：本容器为 Dock.Fill 使用，尺寸不依赖内容，无需父级继续布局。
                return false;
            }
        }
    }
}
