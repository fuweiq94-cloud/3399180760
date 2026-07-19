using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XyzController.Controls
{
    /// <summary>
    /// 双向轴位置条（带限位报警）。
    /// 比 ZBarView 更工业化：
    /// - 水平/竖直方向可选
    /// - 正值蓝色填充，负值橙色填充（区分方向）
    /// - 可设置软限位（SoftLimit），超限变红报警
    /// - 当前值数字直接画在指针旁
    /// </summary>
    public class AxisBar : Control
    {
        public enum Orientations { Horizontal, Vertical }

        public Orientations Orientation { get; set; }

        public float RangeMin { get; set; }
        public float RangeMax { get; set; }
        public float CurrentValue { get; set; }
        public float TargetValue { get; set; }

        /// <summary>软限位（正方向），超过变红。null = 不检查。</summary>
        public float? SoftLimitPositive { get; set; }

        /// <summary>软限位（负方向），低于变红。</summary>
        public float? SoftLimitNegative { get; set; }

        public string AxisLabel { get; set; }

        public AxisBar()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Orientation = Orientations.Vertical;
            RangeMin = -100f;
            RangeMax = 100f;
            AxisLabel = "X";
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 8F);
            SetStyle(ControlStyles.Selectable, false);
        }

        private bool IsOverLimit
        {
            get
            {
                return (SoftLimitPositive.HasValue && CurrentValue > SoftLimitPositive.Value)
                    || (SoftLimitNegative.HasValue && CurrentValue < SoftLimitNegative.Value);
            }
        }

        public void SetValue(float v)
        {
            CurrentValue = v;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (Brush bg = new SolidBrush(BackColor))
                g.FillRectangle(bg, ClientRectangle);

            RectangleF bar = BarArea;

            // 背景
            using (Brush bb = new SolidBrush(Color.FromArgb(225, 230, 238)))
                g.FillRectangle(bb, bar);

            // 原点（0）位置标线
            float zeroPos = ValueToPos(0);
            DrawAxisLine(g, zeroPos, Color.FromArgb(160, 140, 140, 140), 1.4f, true);

            // 软限位标线
            if (SoftLimitPositive.HasValue)
                DrawAxisLine(g, ValueToPos(SoftLimitPositive.Value), Color.FromArgb(220, 140, 40), 1.2f, false);
            if (SoftLimitNegative.HasValue)
                DrawAxisLine(g, ValueToPos(SoftLimitNegative.Value), Color.FromArgb(220, 140, 40), 1.2f, false);

            // 填充：从 0 到当前值（正值蓝色 / 负值橙色，超限红色）
            float curPos = ValueToPos(CurrentValue);
            RectangleF fillRect = MakeFillRect(zeroPos, curPos);
            Color fillColor = ChooseFillColor();
            using (Brush fb = new SolidBrush(fillColor))
                g.FillRectangle(fb, fillRect);

            // 边框
            using (Pen bp = new Pen(Color.FromArgb(150, 160, 175)))
                g.DrawRectangle(bp, bar.X, bar.Y, bar.Width, bar.Height);

            // 刻度
            DrawTicks(g);

            // 当前值标牌
            DrawValueBadge(g, curPos, fillColor);

            // 轴标签
            DrawAxisLabel(g);
        }

        private RectangleF BarArea
        {
            get
            {
                RectangleF r = ClientRectangle;
                if (Orientation == Orientations.Vertical)
                {
                    float w = 30f;
                    float cx = r.Width / 2f;
                    return new RectangleF(cx - w / 2f, r.Top + 18f, w, r.Height - 44f);
                }
                else
                {
                    float h = 26f;
                    float cy = r.Height / 2f;
                    return new RectangleF(r.Left + 18f, cy - h / 2f, r.Width - 36f, h);
                }
            }
        }

        // 把数值映射到条上的像素位置（竖直: y坐标；水平: x坐标）
        private float ValueToPos(float v)
        {
            RectangleF bar = BarArea;
            float span = RangeMax - RangeMin;
            if (span <= 0) span = 1;
            float ratio = (v - RangeMin) / span;
            if (Orientation == Orientations.Vertical)
                return bar.Bottom - ratio * bar.Height;
            else
                return bar.Left + ratio * bar.Width;
        }

        private RectangleF MakeFillRect(float zeroPos, float curPos)
        {
            RectangleF bar = BarArea;
            if (Orientation == Orientations.Vertical)
            {
                float y = Math.Min(zeroPos, curPos);
                float h = Math.Abs(curPos - zeroPos);
                return new RectangleF(bar.X, y, bar.Width, h);
            }
            else
            {
                float x = Math.Min(zeroPos, curPos);
                float w = Math.Abs(curPos - zeroPos);
                return new RectangleF(x, bar.Y, w, bar.Height);
            }
        }

        private Color ChooseFillColor()
        {
            if (IsOverLimit) return Color.FromArgb(220, 70, 70);
            return CurrentValue >= 0 ? Color.FromArgb(60, 140, 220) : Color.FromArgb(230, 150, 50);
        }

        private void DrawAxisLine(Graphics g, float pos, Color color, float width, bool dashed)
        {
            RectangleF bar = BarArea;
            using (Pen p = new Pen(color, width))
            {
                if (dashed) p.DashStyle = DashStyle.Dash;
                if (Orientation == Orientations.Vertical)
                    g.DrawLine(p, bar.Left - 4, pos, bar.Right + 4, pos);
                else
                    g.DrawLine(p, pos, bar.Top - 4, pos, bar.Bottom + 4);
            }
        }

        private void DrawTicks(Graphics g)
        {
            RectangleF bar = BarArea;
            int step = ChooseStep(RangeMax - RangeMin);
            using (Pen tp = new Pen(Color.FromArgb(160, 170, 185)))
            using (Brush tb = new SolidBrush(Color.FromArgb(120, 130, 145)))
            using (Font f = new Font("Segoe UI", 7.5F))
            {
                StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center };
                for (float v = RangeMin; v <= RangeMax + 0.01f; v += step)
                {
                    float pos = ValueToPos(v);
                    if (Orientation == Orientations.Vertical)
                    {
                        g.DrawLine(tp, bar.Right + 1, pos, bar.Right + 5, pos);
                        sf.Alignment = StringAlignment.Near;
                        g.DrawString(v.ToString("0"), f, tb, bar.Right + 8, pos, sf);
                    }
                    else
                    {
                        g.DrawLine(tp, pos, bar.Bottom + 1, pos, bar.Bottom + 5);
                        sf.Alignment = StringAlignment.Center;
                        g.DrawString(v.ToString("0"), f, tb, pos, bar.Bottom + 8, sf);
                    }
                }
            }
        }

        private void DrawValueBadge(Graphics g, float pos, Color color)
        {
            RectangleF bar = BarArea;
            string text = CurrentValue.ToString("F2");
            using (Font f = new Font("Consolas", 8F, FontStyle.Bold))
            {
                SizeF ts = g.MeasureString(text, f);
                float bw = ts.Width + 8;
                float bh = ts.Height + 2;
                RectangleF badge;
                if (Orientation == Orientations.Vertical)
                {
                    badge = new RectangleF(bar.Left - bw - 4, pos - bh / 2f, bw, bh);
                }
                else
                {
                    badge = new RectangleF(pos - bw / 2f, bar.Top - bh - 4, bw, bh);
                }
                using (Brush bb = new SolidBrush(color))
                    g.FillRectangle(bb, badge);
                using (Pen p = new Pen(Color.White, 0.8f))
                    g.DrawRectangle(p, badge.X, badge.Y, badge.Width, badge.Height);
                using (Brush tb = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, f, tb, badge, sf);
                }
            }
        }

        private void DrawAxisLabel(Graphics g)
        {
            using (Font f = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Brush b = new SolidBrush(Color.FromArgb(60, 70, 90)))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                if (Orientation == Orientations.Vertical)
                    g.DrawString(AxisLabel ?? "", f, b, Width / 2f, 2f, sf);
                else
                {
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString(AxisLabel ?? "", f, b, 8f, Height / 2f, sf);
                }
            }
        }

        private static int ChooseStep(float span)
        {
            double raw = span / 6.0;
            double[] candidates = { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000 };
            double best = 1; double bestDiff = double.MaxValue;
            foreach (double c in candidates)
            {
                double d = Math.Abs(c - raw);
                if (d < bestDiff) { bestDiff = d; best = c; }
            }
            return (int)best;
        }
    }
}
