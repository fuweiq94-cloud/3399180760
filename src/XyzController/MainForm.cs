using System;
using System.Drawing;
using System.Windows.Forms;
using XyzController.Controls;
using XyzController.Logic;

namespace XyzController
{
    /// <summary>
    /// 主窗体（纯 UI 层）。
    /// 不再包含任何"算坐标/算动画"的业务逻辑，
    /// 所有状态都委托给 XyzControllerHub（业务层）。
    /// 本类只负责：捕获用户输入 → 转发给 hub → 监听 hub 变化 → 刷新控件。
    /// </summary>
    /// <remarks>
    /// 兼容性说明：工控机上的旧 .NET 编译器不支持 Lambda 表达式 (s,e) => ...
    /// 因此所有事件订阅都使用命名方法 + 显式委托构造（new EventHandler(...)）。
    /// </remarks>
    public partial class MainForm : Form
    {
        // —— 业务核心 ——
        private readonly XyzControllerHub _hub;

        // —— JOG 服务：每个轴一个 ——
        private AxisJogService _jogX;
        private AxisJogService _jogY;
        private AxisJogService _jogZ;

        // —— UI 同步锁：防止 hub.Changed 回调里又改 UI 触发新事件 ——
        private bool _syncing;

        public MainForm()
        {
            InitializeComponent();

            // 1) 创建业务层对象，把 UI 控件的范围同步给它
            _hub = new XyzControllerHub(
                trbX.Minimum, trbX.Maximum,
                trbY.Minimum, trbY.Maximum,
                trbZ.Minimum, trbZ.Maximum);
            _hub.SpeedSetting = trbSpeed.Value;

            // 2) 同步自定义视图的范围（XYView/ZBarView 自己维护坐标范围）
            xyView.RangeMin = trbX.Minimum;
            xyView.RangeMax = trbX.Maximum;
            zBar.RangeMin = trbZ.Minimum;
            zBar.RangeMax = trbZ.Maximum;

            // 3) 绑定 UI 事件 → 转发到 hub
            HookEvents();

            // 4) 监听 hub 变化 → 刷新 UI（不用 Lambda）
            _hub.Changed += new EventHandler(Hub_Changed);

            // 5) 初始刷新一次，让 UI 反映 hub 默认状态
            SyncUiFromHub();
            UpdateSpeedLabel();
        }

        /// <summary>
        /// 窗体首次加载时触发。设计器不会执行此方法。
        /// 把"启动副作用"（如定时器、串口、网络）放在这里，比放在构造函数更规范，
        /// 因为设计器解析构造函数时会跳过 OnLoad，避免在设计器里触发动画/IO。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            animTimer.Start();

            // 锁定 Z 条宽度：禁止用户拖动分隔条改变 Z 条宽度。
            splitMain.IsSplitterFixed = true;
            FixSplitterDistance();
            splitMain.SplitterMoved += new SplitterEventHandler(SplitMain_SplitterMoved);
        }

        /// <summary>
        /// 让 Panel2（Z 条）始终保持在右侧 80px 宽。
        /// </summary>
        private void FixSplitterDistance()
        {
            int panel2Width = 80;
            int target = splitMain.Width - panel2Width - splitMain.SplitterWidth;
            if (splitMain.SplitterDistance != target && target > 0)
                splitMain.SplitterDistance = target;
        }

        private void SplitMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            FixSplitterDistance();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (splitMain != null) FixSplitterDistance();
        }

        // ============== UI → 业务 ==============

        private void HookEvents()
        {
            // —— 滑块、数字框、按钮（全部用命名方法 + 委托构造）——
            trbX.Scroll += new EventHandler(TrbX_Scroll);
            trbY.Scroll += new EventHandler(TrbY_Scroll);
            trbZ.Scroll += new EventHandler(TrbZ_Scroll);

            nudX.ValueChanged += new EventHandler(NudX_ValueChanged);
            nudY.ValueChanged += new EventHandler(NudY_ValueChanged);
            nudZ.ValueChanged += new EventHandler(NudZ_ValueChanged);

            btnXMinus.Click += new EventHandler(BtnXMinus_Click);
            btnXPlus.Click  += new EventHandler(BtnXPlus_Click);
            btnYMinus.Click += new EventHandler(BtnYMinus_Click);
            btnYPlus.Click  += new EventHandler(BtnYPlus_Click);
            btnZMinus.Click += new EventHandler(BtnZMinus_Click);
            btnZPlus.Click  += new EventHandler(BtnZPlus_Click);

            // —— 通用按钮 ——
            btnZero.Click      += new EventHandler(BtnZero_Click);
            btnCenter.Click    += new EventHandler(BtnCenter_Click);
            btnRandom.Click    += new EventHandler(BtnRandom_Click);
            btnClearTrail.Click += new EventHandler(BtnClearTrail_Click);

            trbSpeed.Scroll += new EventHandler(TrbSpeed_Scroll);

            // —— JOG 服务：三轴各创建一个 ——
            _jogX = new AxisJogService(_hub.X);
            _jogY = new AxisJogService(_hub.Y);
            _jogZ = new AxisJogService(_hub.Z);

            // 寸动 / 连续模式切换
            rbIncremental.CheckedChanged += new EventHandler(RbMode_CheckedChanged);

            // 步长变化（寸动模式生效）
            nudJogStep.ValueChanged += new EventHandler(NudJogStep_ValueChanged);

            // JogButton 控件：Jog 事件 = 按下/持续触发；Stop 事件 = 松开
            // 注意：寸动模式下 Jog 事件会重复触发，但每次走一步；
            //       连续模式下 Jog 只在按下时触发一次（设目标到限位），Stop 才停。
            BindJogButton(jogXPlus, _jogX);
            BindJogButton(jogXMinus, _jogX);
            BindJogButton(jogYPlus, _jogY);
            BindJogButton(jogYMinus, _jogY);
            BindJogButton(jogZPlus, _jogZ);
            BindJogButton(jogZMinus, _jogZ);

            // 急停按钮
            btnEStop.Click += new EventHandler(BtnEStop_Click);

            // —— 鼠标拖动 XY 视图设定目标 ——
            xyView.TargetSetByMouse += new EventHandler<PointF>(XyView_TargetSetByMouse);

            // —— 键盘 ——
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
        }

        // —— hub 变化 → 刷新 UI ——
        private void Hub_Changed(object sender, EventArgs e)
        {
            SyncUiFromHub();
        }

        // —— 滑块事件处理器 ——
        private void TrbX_Scroll(object sender, EventArgs e) { _hub.X.SetTarget(trbX.Value); }
        private void TrbY_Scroll(object sender, EventArgs e) { _hub.Y.SetTarget(trbY.Value); }
        private void TrbZ_Scroll(object sender, EventArgs e) { _hub.Z.SetTarget(trbZ.Value); }

        // —— 数字框事件处理器 ——
        private void NudX_ValueChanged(object sender, EventArgs e) { _hub.X.SetTarget((float)nudX.Value); }
        private void NudY_ValueChanged(object sender, EventArgs e) { _hub.Y.SetTarget((float)nudY.Value); }
        private void NudZ_ValueChanged(object sender, EventArgs e) { _hub.Z.SetTarget((float)nudZ.Value); }

        // —— 单步按钮（X 轴）——
        private void BtnXMinus_Click(object sender, EventArgs e) { _hub.X.Step(-1); }
        private void BtnXPlus_Click(object sender, EventArgs e)  { _hub.X.Step(+1); }
        private void BtnYMinus_Click(object sender, EventArgs e) { _hub.Y.Step(-1); }
        private void BtnYPlus_Click(object sender, EventArgs e)  { _hub.Y.Step(+1); }
        private void BtnZMinus_Click(object sender, EventArgs e) { _hub.Z.Step(-1); }
        private void BtnZPlus_Click(object sender, EventArgs e)  { _hub.Z.Step(+1); }

        // —— 通用按钮 ——
        private void BtnZero_Click(object sender, EventArgs e)      { _hub.ResetToOrigin(); }
        private void BtnCenter_Click(object sender, EventArgs e)    { _hub.SetToCenter(); }
        private void BtnRandom_Click(object sender, EventArgs e)    { _hub.SetRandomTarget(new Random()); }
        private void BtnClearTrail_Click(object sender, EventArgs e) { xyView.ClearTrail(); }

        // —— 速度滑块 ——
        private void TrbSpeed_Scroll(object sender, EventArgs e)
        {
            _hub.SpeedSetting = trbSpeed.Value;
            UpdateSpeedLabel();
        }

        // —— JOG 模式切换 ——
        private void RbMode_CheckedChanged(object sender, EventArgs e)
        {
            JogMode m = rbIncremental.Checked ? JogMode.Incremental : JogMode.Continuous;
            _jogX.SetMode(m);
            _jogY.SetMode(m);
            _jogZ.SetMode(m);
        }

        // —— JOG 步长 ——
        private void NudJogStep_ValueChanged(object sender, EventArgs e)
        {
            float step = (float)nudJogStep.Value;
            _jogX.SetStepDistance(step);
            _jogY.SetStepDistance(step);
            _jogZ.SetStepDistance(step);
        }

        // —— 急停 ——
        private void BtnEStop_Click(object sender, EventArgs e)
        {
            _jogX.EmergencyStop();
            _jogY.EmergencyStop();
            _jogZ.EmergencyStop();
        }

        // —— 鼠标点击 XY 视图设定目标 ——
        private void XyView_TargetSetByMouse(object sender, PointF e)
        {
            _hub.X.SetTarget(e.X);
            _hub.Y.SetTarget(e.Y);
        }

        /// <summary>
        /// 把 JogButton 控件的事件接到 AxisJogService。
        /// JogButton.Jog 事件会在按下/重复触发时调用；Stop 在松开时调用。
        /// </summary>
        private void BindJogButton(JogButton btn, AxisJogService service)
        {
            // 把 service 通过 closure 替换为 JogButton.Tag 携带，便于在命名方法里取回。
            btn.Tag = service;
            btn.Jog += new EventHandler<int>(JogButton_Jog);
            btn.Stop += new EventHandler<int>(JogButton_Stop);
        }

        private void JogButton_Jog(object sender, int direction)
        {
            JogButton btn = (JogButton)sender;
            AxisJogService service = (AxisJogService)btn.Tag;
            service.OnJogStart(direction);
        }

        private void JogButton_Stop(object sender, int direction)
        {
            JogButton btn = (JogButton)sender;
            AxisJogService service = (AxisJogService)btn.Tag;
            service.OnJogStop();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
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
                    xyView.ClearTrail(); break;
                default:
                    handled = false; break;
            }
            if (handled) e.Handled = true;
        }

        // ============== 业务 → UI ==============

        /// <summary>
        /// 把 hub 的状态（目标值 + 当前值）反向同步到所有 UI 控件。
        /// </summary>
        private void SyncUiFromHub()
        {
            if (_syncing) return;
            _syncing = true;
            try
            {
                // 滑块（必须整数）
                trbX.Value = ClampInt((int)Math.Round(_hub.X.Target), trbX.Minimum, trbX.Maximum);
                trbY.Value = ClampInt((int)Math.Round(_hub.Y.Target), trbY.Minimum, trbY.Maximum);
                trbZ.Value = ClampInt((int)Math.Round(_hub.Z.Target), trbZ.Minimum, trbZ.Maximum);

                // 数字框（支持小数）
                if (nudX.Value != (decimal)_hub.X.Target)
                    nudX.Value = ClampDecimal((decimal)_hub.X.Target, nudX.Minimum, nudX.Maximum);
                if (nudY.Value != (decimal)_hub.Y.Target)
                    nudY.Value = ClampDecimal((decimal)_hub.Y.Target, nudY.Minimum, nudY.Maximum);
                if (nudZ.Value != (decimal)_hub.Z.Target)
                    nudZ.Value = ClampDecimal((decimal)_hub.Z.Target, nudZ.Minimum, nudZ.Maximum);

                // 自定义视图：目标和当前都同步
                xyView.TargetX = _hub.X.Target;
                xyView.TargetY = _hub.Y.Target;
                zBar.TargetZ = _hub.Z.Target;
                // 当前值不能直接赋（CurrentX/Y/Z 是只读），靠 animTimer 推进后视图自己刷新
            }
            finally { _syncing = false; }
        }

        // ============== 动画 Tick ==============
        private void animTimer_Tick(object sender, EventArgs e)
        {
            // 推进业务层的动画
            _hub.Advance();

            // 让自定义视图按当前值重画
            xyView.Advance(_hub.CurrentLerpFraction);
            zBar.Advance(_hub.CurrentLerpFraction);

            UpdateStatusLive();
        }

        // ============== 状态栏 ==============
        private void UpdateStatusLive()
        {
            lblStatus.Text = string.Format(
                "当前 X={0:F2}  Y={1:F2}  Z={2:F2}   →  目标 X={3:F2}  Y={4:F2}  Z={5:F2}",
                _hub.X.Current, _hub.Y.Current, _hub.Z.Current,
                _hub.X.Target, _hub.Y.Target, _hub.Z.Target);
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

        // ============== 工具方法 ==============
        private static int ClampInt(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private static decimal ClampDecimal(decimal v, decimal min, decimal max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
