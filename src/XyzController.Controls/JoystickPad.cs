using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XyzController.Controls
{
    /// <summary>
    /// 8 方向虚拟摇杆。
    /// 按住中心圆拖动（或鼠标按住任意位置）会输出方向矢量 (dx, dy)，每个分量 ∈ {-1, 0, +1}。
    /// 松开自动回中。可同时按 2 个方向（如右上 = (+1, +1)）。
    /// </summary>
    [ToolboxBitmap(typeof(JoystickPad), "Resources.JoystickPad.bmp")]
    [DefaultEvent("DirectionChanged")]
    [DefaultProperty("DeadZone")]
    [Description("8 方向虚拟摇杆：拖动输出方向矢量 (dx, dy)，松开自动回中。可同时按 2 个方向。")]
    public class JoystickPad : Control
    {
        // 方向死区：拖动距离超过这个值才算"有方向"
        [Category("Behavior")]
        [DefaultValue(0.25f)]
        [Description("方向死区（归一化位移 0~1），拖动距离超过此值才算有方向输出。")]
        public float DeadZone { get; set; }

        private PointF _knob = PointF.Empty;   // 摇杆当前位置（相对中心的偏移）
        private bool _dragging;
        private int _dx, _dy;                   // 当前输出方向 (-1/0/+1)

        /// <summary>方向或强度变化时触发。参数为 (dx, dy)。</summary>
        [Category("Action")]
        [Description("方向变化时引发。参数 Point(dx, dy)，每个分量 ∈ {-1, 0, +1}，Y 轴已翻转（向上=+1）。")]
        public event EventHandler<Point> DirectionChanged;

        public JoystickPad()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            DeadZone = 0.25f;
            BackColor = Color.FromArgb(245, 247, 250);
            SetStyle(ControlStyles.Selectable, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // 强制保持正方形（取较短边）
            int s = Math.Min(Width, Height);
            if (Width != s || Height != s)
            {
                SetBounds(Left, Top, s, s, BoundsSpecified.Size);
            }
        }

        // 计算可操作半径
        private float Radius
        {
            get { return Math.Min(Width, Height) / 2f - 8f; }
        }

        private PointF Center
        {
            get { return new PointF(Width / 2f, Height / 2f); }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                UpdateKnob(e.X, e.Y);
                Focus();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging) UpdateKnob(e.X, e.Y);
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_dragging)
            {
                _dragging = false;
                // 松开回中
                _knob = PointF.Empty;
                SetDirection(0, 0);
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        private void UpdateKnob(float mx, float my)
        {
            PointF c = Center;
            float dx = mx - c.X;
            float dy = my - c.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            float r = Radius;

            // 限制在圆内
            if (dist > r)
            {
                dx = dx / dist * r;
                dy = dy / dist * r;
            }
            _knob = new PointF(dx, dy);

            // 计算方向（基于归一化位移）
            float nx = r > 0 ? dx / r : 0;
            float ny = r > 0 ? dy / r : 0;
            int newDx = Math.Abs(nx) < DeadZone ? 0 : (nx > 0 ? +1 : -1);
            // Y 轴翻转：屏幕坐标向下为正，输出"向上"为 +1
            int newDy = Math.Abs(ny) < DeadZone ? 0 : (ny > 0 ? -1 : +1);

            SetDirection(newDx, newDy);
            Invalidate();
        }

        private void SetDirection(int dx, int dy)
        {
            if (dx == _dx && dy == _dy) return;
            _dx = dx;
            _dy = dy;
            EventHandler<Point> h = DirectionChanged;
            if (h != null) h(this, new Point(_dx, _dy));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            PointF c = Center;
            float r = Radius;

            // 外圆（底盘）
            using (LinearGradientBrush bg = new LinearGradientBrush(
                new RectangleF(0, 0, Width, Height),
                Color.FromArgb(230, 235, 245), Color.FromArgb(210, 218, 230), 90F))
            {
                g.FillEllipse(bg, c.X - r, c.Y - r, r * 2, r * 2);
            }
            using (Pen p = new Pen(Color.FromArgb(180, 190, 205), 1.5f))
                g.DrawEllipse(p, c.X - r, c.Y - r, r * 2, r * 2);

            // 十字辅助线
            using (Pen p = new Pen(Color.FromArgb(200, 206, 216), 1f))
            {
                p.DashStyle = DashStyle.Dot;
                g.DrawLine(p, c.X - r, c.Y, c.X + r, c.Y);
                g.DrawLine(p, c.X, c.Y - r, c.X, c.Y + r);
            }

            // 死区圆（视觉提示）
            float dr = r * DeadZone;
            using (Pen p = new Pen(Color.FromArgb(220, 200, 200, 210), 1f))
                g.DrawEllipse(p, c.X - dr, c.Y - dr, dr * 2, dr * 2);

            // 摇杆手柄
            float knobR = r * 0.28f;
            PointF knobPos = new PointF(c.X + _knob.X, c.Y + _knob.Y);
            using (LinearGradientBrush kb = new LinearGradientBrush(
                new RectangleF(knobPos.X - knobR, knobPos.Y - knobR, knobR * 2, knobR * 2),
                Color.FromArgb(120, 170, 240), Color.FromArgb(40, 100, 210), 90F))
            {
                g.FillEllipse(kb, knobPos.X - knobR, knobPos.Y - knobR, knobR * 2, knobR * 2);
            }
            using (Pen p = new Pen(Color.FromArgb(255, 255, 255, 255), 2f))
                g.DrawEllipse(p, knobPos.X - knobR, knobPos.Y - knobR, knobR * 2, knobR * 2);

            // 方向指示箭头（中心十字小标）
            DrawDirectionHint(g, c, r);
        }

        private void DrawDirectionHint(Graphics g, PointF c, float r)
        {
            using (Font f = new Font("Segoe UI", 8F))
            using (Brush b = new SolidBrush(Color.FromArgb(150, 160, 175)))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("↑", f, b, c.X, c.Y - r * 0.78f, sf);
                g.DrawString("↓", f, b, c.X, c.Y + r * 0.78f, sf);
                g.DrawString("←", f, b, c.X - r * 0.78f, c.Y, sf);
                g.DrawString("→", f, b, c.X + r * 0.78f, c.Y, sf);
            }
        }
    }
}
