using System;
using System.Drawing;
using System.Windows.Forms;
using XyzController.Controls;
using XyzController.Logic;

namespace XyzController
{
    /// <summary>
    /// 运动轨迹查看窗体（纯 UI 层）。
    /// 显示 XY 平面运动轨迹和 Z 轴位置条，用于观察和记录运动路径。
    /// 所有运动逻辑委托给 XyzControllerHub（业务层），本类只负责：
    /// 捕获用户输入 → 转发给 hub → 监听 hub 变化 → 刷新控件。
    /// </summary>
    /// <remarks>
    /// 兼容性说明：工控机上的旧 .NET 编译器不支持 Lambda 表达式 (s,e) => ...
    /// 因此所有事件订阅都使用命名方法 + 显式委托构造（new EventHandler(...)）。
    /// </remarks>
    public partial class TrajectoryViewForm : Form
    {
        // —— 业务核心 ——
        private readonly XyzControllerHub _hub;

        // —— UI 同步锁 ——
        private bool _syncing;

        // —— 轨迹统计 ——
        private int _trailPointCount;

        public TrajectoryViewForm()
        {
            InitializeComponent();

            // 1) 创建业务层
            _hub = new XyzControllerHub(
                xyView.RangeMin, xyView.RangeMax,
                xyView.RangeMin, xyView.RangeMax,
                zBar.RangeMin, zBar.RangeMax);
            _hub.SpeedSetting = trbSpeed.Value;

            // 2) 绑定 UI 事件
            HookEvents();

            // 3) 监听 hub 变化 → 刷新 UI
            _hub.Changed += new EventHandler(Hub_Changed);

            // 4) 初始刷新
            SyncUiFromHub();
            UpdateSpeedLabel();
        }

        /// <summary>
        /// 窗体首次加载时触发动画定时器。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            animTimer.Start();

            // 锁定分隔条
            splitMain.IsSplitterFixed = true;
        }

        // ============== 事件绑定 ==============

        private void HookEvents()
        {
            btnClearTrail.Click += new EventHandler(BtnClearTrail_Click);
            cbShowTrail.CheckedChanged += new EventHandler(CbShowTrail_CheckedChanged);
            btnOrigin.Click += new EventHandler(BtnOrigin_Click);
            btnCenter.Click += new EventHandler(BtnCenter_Click);
            btnRandom.Click += new EventHandler(BtnRandom_Click);
            trbSpeed.Scroll += new EventHandler(TrbSpeed_Scroll);
            xyView.TargetSetByMouse += new EventHandler<PointF>(XyView_TargetSetByMouse);
            this.KeyDown += new KeyEventHandler(TrajectoryViewForm_KeyDown);
        }

        // ============== UI → 业务 ==============

        /// <summary>清除轨迹。</summary>
        private void BtnClearTrail_Click(object sender, EventArgs e)
        {
            xyView.ClearTrail();
            _trailPointCount = 0;
            UpdateTrailInfo();
        }

        /// <summary>切换轨迹显示。</summary>
        private void CbShowTrail_CheckedChanged(object sender, EventArgs e)
        {
            // XYView 内部通过 Invalidate 重绘，这里通过清除/保留轨迹实现
            // 如果取消勾选，清除当前轨迹显示
            if (!cbShowTrail.Checked)
            {
                xyView.ClearTrail();
            }
            xyView.Invalidate();
        }

        /// <summary>回原点。</summary>
        private void BtnOrigin_Click(object sender, EventArgs e)
        {
            _hub.ResetToOrigin();
        }

        /// <summary>居中。</summary>
        private void BtnCenter_Click(object sender, EventArgs e)
        {
            _hub.SetToCenter();
        }

        /// <summary>随机目标。</summary>
        private void BtnRandom_Click(object sender, EventArgs e)
        {
            _hub.SetRandomTarget(new Random());
        }

        /// <summary>速度滑块变化。</summary>
        private void TrbSpeed_Scroll(object sender, EventArgs e)
        {
            _hub.SpeedSetting = trbSpeed.Value;
            UpdateSpeedLabel();
        }

        /// <summary>鼠标点击 XY 视图设定目标。</summary>
        private void XyView_TargetSetByMouse(object sender, PointF e)
        {
            _hub.X.SetTarget(e.X);
            _hub.Y.SetTarget(e.Y);
        }

        /// <summary>键盘快捷键。</summary>
        private void TrajectoryViewForm_KeyDown(object sender, KeyEventArgs e)
        {
            int big = e.Shift ? 10 : 1;
            bool handled = true;

            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    _hub.X.Step(-big); break;
                case Keys.Right:
                case Keys.D:
                    _hub.X.Step(+big); break;
                case Keys.Down:
                case Keys.S:
                    _hub.Y.Step(-big); break;
                case Keys.Up:
                case Keys.W:
                    _hub.Y.Step(+big); break;
                case Keys.PageDown:
                case Keys.Q:
                    _hub.Z.Step(-big); break;
                case Keys.PageUp:
                case Keys.E:
                    _hub.Z.Step(+big); break;
                case Keys.Space:
                    _hub.ResetToOrigin(); break;
                case Keys.Escape:
                    xyView.ClearTrail();
                    _trailPointCount = 0;
                    UpdateTrailInfo();
                    break;
                default:
                    handled = false; break;
            }
            if (handled) e.Handled = true;
        }

        // ============== 业务 → UI ==============

        private void Hub_Changed(object sender, EventArgs e)
        {
            SyncUiFromHub();
        }

        /// <summary>把 hub 状态同步到 UI 控件。</summary>
        private void SyncUiFromHub()
        {
            if (_syncing) return;
            _syncing = true;
            try
            {
                // XY 视图目标
                xyView.TargetX = _hub.X.Target;
                xyView.TargetY = _hub.Y.Target;
                zBar.TargetZ = _hub.Z.Target;
            }
            finally { _syncing = false; }
        }

        // ============== 动画 Tick ==============

        private void animTimer_Tick(object sender, EventArgs e)
        {
            // 推进业务层动画
            _hub.Advance();

            // 让自定义视图按当前值重画
            if (cbShowTrail.Checked)
            {
                xyView.Advance(_hub.CurrentLerpFraction);
                _trailPointCount++;
            }
            else
            {
                // 不显示轨迹时仍更新位置但不记录轨迹
                xyView.Advance(_hub.CurrentLerpFraction);
            }
            zBar.Advance(_hub.CurrentLerpFraction);

            // 更新 DRO
            droX.SetValue(_hub.X.Current);
            droY.SetValue(_hub.Y.Current);
            droZ.SetValue(_hub.Z.Current);

            UpdateStatusLive();
            UpdateTrailInfo();
        }

        // ============== 辅助方法 ==============

        private void UpdateStatusLive()
        {
            lblStatus.Text = string.Format(
                "当前 X={0:F2}  Y={1:F2}  Z={2:F2}   →  目标 X={3:F2}  Y={4:F2}  Z={5:F2}",
                _hub.X.Current, _hub.Y.Current, _hub.Z.Current,
                _hub.X.Target, _hub.Y.Target, _hub.Z.Target);
        }

        private void UpdateTrailInfo()
        {
            lblTrailInfo.Text = "轨迹点数：" + _trailPointCount.ToString()
                + "   速度档位：" + _hub.SpeedSetting.ToString();
        }

        private void UpdateSpeedLabel()
        {
            string label;
            int v = trbSpeed.Value;
            if (v < 20) label = "速度：慢";
            else if (v < 60) label = "速度：中";
            else if (v < 90) label = "速度：快";
            else label = "速度：瞬时";
            lblSpeed.Text = label;
        }
    }
}
