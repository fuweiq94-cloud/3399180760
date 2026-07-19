using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XyzController.Controls
{
    /// <summary>
    /// 竖直进度条样式的 Z 轴指示控件。
    /// 显示：刻度、当前 Z 值、目标 Z 值标记。
    /// </summary>
    public class ZBarView : Control
    {
        public float RangeMin { get; set; }
        public float RangeMax { get; set; }
        public float CurrentZ { get; private set; }
        public float TargetZ { get; set; }

        public ZBarView()
        {
            RangeMin = -50f;
            RangeMax = 100f;
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.FromArgb(245, 247, 250);
            SetStyle(ControlStyles.Selectable, true);
        }

        public void Advance(float lerp)
        {
            float k = Math.Max(0.02f, Math.Min(1f, lerp));
            CurrentZ += (TargetZ - CurrentZ) * k;
            Invalidate();
        }

        private RectangleF BarArea
        {
            get
            {
                RectangleF r = ClientRectangle;
                float w = 28f;
                float cx = r.Width / 2f;
                return new RectangleF(cx - w / 2f, r.Top + 18f, w, r.Height - 44f);
            }
        }

        private float ZToScreen(float z)
        {
            RectangleF bar = BarArea;
            float span = RangeMax - RangeMin;
            if (span <= 0) span = 1;
            return bar.Bottom - (z - RangeMin) / span * bar.Height;
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

            // 中线 (Z=0)
            float zeroY = ZToScreen(0);
            if (zeroY > bar.Top && zeroY < bar.Bottom)
            {
                using (Pen p = new Pen(Color.FromArgb(180, 140, 140, 140), 1f))
                { p.DashStyle = DashStyle.Dash; g.DrawLine(p, bar.Left - 4, zeroY, bar.Right + 4, zeroY); }
            }

            // 填充：从 min 到 current
            float curY = ZToScreen(CurrentZ);
            float minY = ZToScreen(RangeMin);
            RectangleF fillRect = new RectangleF(bar.X,
                Math.Min(curY, minY), bar.Width, Math.Abs(curY - minY));
            using (LinearGradientBrush lb =
                new LinearGradientBrush(fillRect,
                    Color.FromArgb(120, 200, 160), Color.FromArgb(60, 160, 220),
                    LinearGradientMode.Vertical))
            {
                g.FillRectangle(lb, fillRect);
            }

            // 边框
            using (Pen bp = new Pen(Color.FromArgb(150, 160, 175)))
                g.DrawRectangle(bp, bar.X, bar.Y, bar.Width, bar.Height);

            // 刻度
            using (Pen tp = new Pen(Color.FromArgb(160, 170, 185)))
            using (Brush tb = new SolidBrush(Color.FromArgb(120, 130, 145)))
            using (Font f = new Font("Segoe UI", 7.5F))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                int step = ChooseStep(RangeMax - RangeMin);
                for (float z = RangeMin; z <= RangeMax + 0.01f; z += step)
                {
                    float y = ZToScreen(z);
                    g.DrawLine(tp, bar.Right + 1, y, bar.Right + 5, y);
                    g.DrawString(z.ToString("0"), f, tb, bar.Right + 8, y, sf);
                }
            }

            // 目标 Z 标记（箭头）
            float tY = ZToScreen(TargetZ);
            using (Pen tp = new Pen(Color.FromArgb(220, 140, 40), 1.6f))
            {
                g.DrawLine(tp, bar.Left - 10, tY, bar.Left - 2, tY);
                g.DrawLine(tp, bar.Left - 2, tY, bar.Left - 6, tY - 4);
                g.DrawLine(tp, bar.Left - 2, tY, bar.Left - 6, tY + 4);
            }

            // 当前值指示条
            using (Pen cp = new Pen(Color.FromArgb(40, 80, 180), 2f))
                g.DrawLine(cp, bar.Left - 2, curY, bar.Right + 2, curY);

            // 顶部标题
            using (Font tf = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (Brush tbh = new SolidBrush(Color.FromArgb(50, 60, 80)))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString("Z", tf, tbh, ClientRectangle.Width / 2f, 2f, sf);
            }

            // 底部数值
            using (Font vf = new Font("Consolas", 9F, FontStyle.Bold))
            using (Brush vb = new SolidBrush(Color.FromArgb(40, 80, 180)))
            {
                string s = string.Format("{0:F2}", CurrentZ);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString(s, vf, vb, ClientRectangle.Width / 2f,
                    ClientRectangle.Bottom - 16f, sf);
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
