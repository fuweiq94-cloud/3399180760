using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XyzController.Controls
{
    /// <summary>
    /// XY 平面俯视图控件。
    /// 显示：坐标网格、原点、十字光标、目标点、运动轨迹、鼠标交互拾取。
    /// </summary>
    public class XYView : Control
    {
        // 坐标范围（机械坐标系，单位默认 mm）
        public float RangeMin { get; set; }
        public float RangeMax { get; set; }

        // 当前显示的坐标（被动画逐渐逼近 TargetX/TargetY）
        public float CurrentX { get; private set; }
        public float CurrentY { get; private set; }

        // 目标坐标（滑块/键盘设定）
        public float TargetX { get; set; }
        public float TargetY { get; set; }

        // 运动轨迹
        private readonly List<PointF> _trail = new List<PointF>();
        private const int MaxTrail = 400;

        // 鼠标拖拽相关
        private bool _dragging;
        private bool _hover;

        // 当用户通过鼠标点击/拖拽设定新目标时触发
        public event EventHandler<PointF> TargetSetByMouse;

        public XYView()
        {
            RangeMin = -100f;
            RangeMax = 100f;
            DoubleBuffered = true;          // 关键：避免重绘闪烁
            ResizeRedraw = true;
            BackColor = Color.FromArgb(245, 247, 250);
            ForeColor = Color.FromArgb(60, 70, 90);
            Font = new Font("Segoe UI", 8.25F);
            // 让控件可接收键盘焦点
            SetStyle(ControlStyles.Selectable, true);
        }

        // —— 把机械坐标转换为像素坐标 ——
        private PointF ToScreen(float mx, float my)
        {
            RectangleF area = DrawableArea;
            float span = RangeMax - RangeMin;
            if (span <= 0) span = 1;

            float sx = area.Left + (mx - RangeMin) / span * area.Width;
            // Y 轴要翻转（屏幕坐标向下为正，数学坐标向上为正）
            float sy = area.Bottom - (my - RangeMin) / span * area.Height;
            return new PointF(sx, sy);
        }

        // 把像素坐标转成机械坐标
        private PointF ToMachine(float sx, float sy)
        {
            RectangleF area = DrawableArea;
            float span = RangeMax - RangeMin;
            if (span <= 0) span = 1;

            float mx = RangeMin + (sx - area.Left) / area.Width * span;
            float my = RangeMin + (area.Bottom - sy) / area.Height * span;
            return new PointF(mx, my);
        }

        // 可绘制区域（留出边距给刻度文字）
        private RectangleF DrawableArea
        {
            get
            {
                RectangleF r = ClientRectangle;
                const float pad = 28f;
                return new RectangleF(r.Left + pad, r.Top + pad,
                                      r.Width - pad * 2, r.Height - pad * 2);
            }
        }

        // 每一帧由主窗体的 Timer 调用：current 向 target 平滑过渡
        public void Advance(float speedFraction)
        {
            float lerp = Math.Max(0.02f, Math.Min(1f, speedFraction));
            CurrentX += (TargetX - CurrentX) * lerp;
            CurrentY += (TargetY - CurrentY) * lerp;

            // 只有移动了一定距离才记录轨迹，避免点堆积
            if (_trail.Count == 0 ||
                Dist(_trail[_trail.Count - 1], new PointF(CurrentX, CurrentY)) > 0.5f)
            {
                _trail.Add(new PointF(CurrentX, CurrentY));
                if (_trail.Count > MaxTrail) _trail.RemoveAt(0);
            }
            Invalidate();
        }

        public void ClearTrail()
        {
            _trail.Clear();
            Invalidate();
        }

        private static float Dist(PointF a, PointF b)
        {
            float dx = a.X - b.X, dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            RectangleF area = DrawableArea;

            // 背景
            using (Brush bg = new SolidBrush(BackColor))
                g.FillRectangle(bg, ClientRectangle);

            DrawGrid(g, area);
            DrawAxes(g, area);
            DrawTrail(g);
            DrawCrosshair(g, area);
            DrawTarget(g);
            DrawCurrent(g);
            DrawCornerInfo(g);
        }

        private void DrawGrid(Graphics g, RectangleF area)
        {
            float span = RangeMax - RangeMin;
            int step = ChooseStep(span);
            using (Pen p = new Pen(Color.FromArgb(220, 224, 230), 1f))
            using (Pen pMajor = new Pen(Color.FromArgb(200, 206, 214), 1.4f))
            using (Brush tb = new SolidBrush(Color.FromArgb(120, 130, 145)))
            using (Font f = new Font("Segoe UI", 7.5F))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Far;
                sf.LineAlignment = StringAlignment.Center;

                for (float v = RangeMin; v <= RangeMax + 0.01f; v += step)
                {
                    bool major = Math.Abs(v % (step * 2)) < 0.01f || Math.Abs(v) < 0.01f;
                    Pen pen = (Math.Abs(v) < 0.01f) ? null : (major ? pMajor : p);

                    // 竖线（X = v）
                    PointF top = ToScreen(v, RangeMax);
                    PointF bot = ToScreen(v, RangeMin);
                    if (pen != null) g.DrawLine(pen, top, bot);

                    // 横线（Y = v）
                    PointF lft = ToScreen(RangeMin, v);
                    PointF rgt = ToScreen(RangeMax, v);
                    if (pen != null) g.DrawLine(pen, lft, rgt);

                    // 刻度文字
                    if (major)
                    {
                        g.DrawString(v.ToString("0"), f, tb, bot.X, area.Bottom + 6f, sf);
                        StringFormat sf2 = new StringFormat();
                        sf2.Alignment = StringAlignment.Far;
                        sf2.LineAlignment = StringAlignment.Center;
                        g.DrawString(v.ToString("0"), f, tb, area.Left - 4f, lft.Y, sf2);
                    }
                }
            }
        }

        private static int ChooseStep(float span)
        {
            // 让网格大约分 8~12 段
            double raw = span / 10.0;
            double[] candidates = { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000 };
            double best = 1; double bestDiff = double.MaxValue;
            foreach (double c in candidates)
            {
                double d = Math.Abs(c - raw);
                if (d < bestDiff) { bestDiff = d; best = c; }
            }
            return (int)best;
        }

        private void DrawAxes(Graphics g, RectangleF area)
        {
            // 原点十字（X=0, Y=0）
            PointF origin = ToScreen(0, 0);
            using (Pen p = new Pen(Color.FromArgb(120, 130, 150), 1.6f))
            {
                p.DashStyle = DashStyle.Dash;
                g.DrawLine(p, area.Left, origin.Y, area.Right, origin.Y);
                g.DrawLine(p, origin.X, area.Top, origin.X, area.Bottom);
            }
        }

        private void DrawTrail(Graphics g)
        {
            if (_trail.Count < 2) return;
            using (Pen p = new Pen(Color.FromArgb(80, 130, 220, 90), 2f))
            {
                p.LineJoin = LineJoin.Round;
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;
                PointF[] pts = new PointF[_trail.Count];
                for (int i = 0; i < _trail.Count; i++)
                    pts[i] = ToScreen(_trail[i].X, _trail[i].Y);
                g.DrawLines(p, pts);
            }
        }

        private void DrawCrosshair(Graphics g, RectangleF area)
        {
            // 当前点的十字虚线（贯穿整个工作区）
            PointF cur = ToScreen(CurrentX, CurrentY);
            using (Pen p = new Pen(Color.FromArgb(90, 90, 200, 120), 1f))
            {
                p.DashStyle = DashStyle.Dot;
                g.DrawLine(p, area.Left, cur.Y, area.Right, cur.Y);
                g.DrawLine(p, cur.X, area.Top, cur.X, area.Bottom);
            }
        }

        private void DrawTarget(Graphics g)
        {
            // 目标点：空心小圆
            PointF t = ToScreen(TargetX, TargetY);
            using (Pen p = new Pen(Color.FromArgb(220, 140, 40), 1.6f))
            {
                g.DrawEllipse(p, t.X - 6, t.Y - 6, 12, 12);
                g.DrawLine(p, t.X - 9, t.Y, t.X + 9, t.Y);
                g.DrawLine(p, t.X, t.Y - 9, t.X, t.Y + 9);
            }
        }

        private void DrawCurrent(Graphics g)
        {
            // 当前点：实心圆 + 光晕
            PointF c = ToScreen(CurrentX, CurrentY);
            using (Brush halo = new SolidBrush(Color.FromArgb(60, 40, 120, 220)))
                g.FillEllipse(halo, c.X - 12, c.Y - 12, 24, 24);
            using (Brush core = new SolidBrush(_dragging || _hover
                       ? Color.FromArgb(220, 60, 80)
                       : Color.FromArgb(40, 120, 220)))
            {
                g.FillEllipse(core, c.X - 6, c.Y - 6, 12, 12);
            }
            using (Pen p = new Pen(Color.White, 1.5f))
                g.DrawEllipse(p, c.X - 6, c.Y - 6, 12, 12);
        }

        private void DrawCornerInfo(Graphics g)
        {
            string text = string.Format("X = {0:F2}   Y = {1:F2}", CurrentX, CurrentY);
            using (Font f = new Font("Consolas", 9F, FontStyle.Bold))
            using (Brush b = new SolidBrush(Color.FromArgb(50, 60, 80)))
            {
                SizeF sz = g.MeasureString(text, f);
                RectangleF box = new RectangleF(8, 8, sz.Width + 14, sz.Height + 6);
                using (Brush bg = new SolidBrush(Color.FromArgb(230, 235, 245)))
                    g.FillRectangle(bg, box);
                using (Pen p = new Pen(Color.FromArgb(200, 206, 216)))
                    g.DrawRectangle(p, box.X, box.Y, box.Width, box.Height);
                g.DrawString(text, f, b, box.X + 7, box.Y + 3);
            }

            string hint = "左键拖动设定目标 · 右键设为原点";
            using (Font fh = new Font("Segoe UI", 7.5F))
            using (Brush bh = new SolidBrush(Color.FromArgb(140, 145, 155)))
                g.DrawString(hint, fh, bh, 8, ClientRectangle.Bottom - 18f);
        }

        // —— 鼠标交互 ——
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                SetTargetFromPixel(e.X, e.Y);
                Focus();
            }
            else if (e.Button == MouseButtons.Right)
            {
                TargetX = 0; TargetY = 0;
                OnTargetSetByMouse(new PointF(0, 0));
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _hover = true;
            if (_dragging) SetTargetFromPixel(e.X, e.Y);
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        private void SetTargetFromPixel(float sx, float sy)
        {
            PointF m = ToMachine(sx, sy);
            TargetX = Clamp(m.X);
            TargetY = Clamp(m.Y);
            OnTargetSetByMouse(new PointF(TargetX, TargetY));
        }

        private float Clamp(float v)
        {
            if (v < RangeMin) return RangeMin;
            if (v > RangeMax) return RangeMax;
            return v;
        }

        protected virtual void OnTargetSetByMouse(PointF p)
        {
            EventHandler<PointF> h = TargetSetByMouse;
            if (h != null) h(this, p);
        }
    }
}
