using System;
using System.Drawing;
using System.Windows.Forms;
using XyzController.Controls;

namespace XyzController
{
    /// <summary>
    /// 硬件调试窗体：XYZU 四轴控制 view。
    ///
    /// 当前版本：用本地字段模拟 4 个轴（开发/演示用）。
    /// 后期：把模拟字段替换为 IAxisHardware（见文件末尾的"接入真 EtherCAT SDK 步骤"）。
    ///
    /// 布局：左侧 XYView（XY 平面，鼠标可点）+ 右侧两个 ZBarView（Z / U 竖直条）
    ///       底部按钮区（回零 / 居中 / 急停）+ 状态栏。
    /// </summary>
    /// <remarks>
    /// 兼容性：本文件不使用 Lambda 表达式（工控机旧编译器要求），事件订阅用命名方法 + 委托构造。
    /// </remarks>
    public partial class HardwareForm : Form
    {
        // ============== 模拟数据（后期替换为 IAxisHardware）==============
        // 当前位置：由 _cur 们用 lerp 算法逼近 _tgt 们，模拟"硬件编码器读数"
        // 目标位置：由 UI（鼠标点 / 按钮）设置，模拟"下发给硬件的运动指令"
        private float _curX, _curY, _curZ, _curU;
        private float _tgtX, _tgtY, _tgtZ, _tgtU;

        /// <summary>每帧推进系数（越大越快）。0.25 = 25%/帧 的插值。</summary>
        private const float LerpFraction = 0.25f;

        public HardwareForm()
        {
            InitializeComponent();

            // 挂事件（不用 Lambda，工控机兼容）
            xyView.TargetSetByMouse += new EventHandler<PointF>(XyView_TargetSetByMouse);
            btnHome.Click += new EventHandler(BtnHome_Click);
            btnCenter.Click += new EventHandler(BtnCenter_Click);
            btnEStop.Click += new EventHandler(BtnEStop_Click);
            refreshTimer.Tick += new EventHandler(RefreshTimer_Tick);

            // 监听窗体大小变化，重新锁定 splitter（防止用户拖动改变 Z/U 条宽度）
            splitMain.SplitterMoved += new SplitterEventHandler(SplitMain_SplitterMoved);
        }

        /// <summary>
        /// 窗体首次加载时触发。设计器不会执行此方法。
        /// 把"启动副作用"（如定时器、串口、网络）放在这里。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 锁定 Z/U 条宽度（右侧 Panel2 始终保持设计器尺寸）
            splitMain.IsSplitterFixed = true;
            FixSplitterDistance();

            // 启动刷新定时器（30ms 一次 ≈ 33fps）
            refreshTimer.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (splitMain != null) FixSplitterDistance();
        }

        /// <summary>
        /// 让 Panel2（Z/U 条）始终保持设计器宽度。
        /// SplitterDistance 在 Designer.cs 里是字面常量（设计器要求），
        /// 运行时根据 splitMain.Width 动态调整。
        /// </summary>
        private void FixSplitterDistance()
        {
            int panel2Width = 184;
            int target = splitMain.Width - panel2Width - splitMain.SplitterWidth;
            if (target > 0 && splitMain.SplitterDistance != target)
                splitMain.SplitterDistance = target;
        }

        private void SplitMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            FixSplitterDistance();
        }

        // ============== 定时刷新（每 30ms）==============

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // 1) 当前位置向目标位置平滑过渡（模拟硬件运动）
            _curX += (_tgtX - _curX) * LerpFraction;
            _curY += (_tgtY - _curY) * LerpFraction;
            _curZ += (_tgtZ - _curZ) * LerpFraction;
            _curU += (_tgtU - _curU) * LerpFraction;

            // 2) 把目标值同步给控件
            xyView.TargetX = _tgtX;
            xyView.TargetY = _tgtY;
            zBar.TargetZ = _tgtZ;
            uBar.TargetZ = _tgtU;

            // 3) 让控件按当前值重画（XYView/ZBarView 的 CurrentX/Y/Z 是 private set，
            //    必须通过 Advance 推进，控件自己会维护 Current 值）
            xyView.Advance(LerpFraction);
            zBar.Advance(LerpFraction);
            uBar.Advance(LerpFraction);

            // 4) 更新状态栏
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            lblStatus.Text = string.Format(
                "当前  X = {0,8:F2}   Y = {1,8:F2}   Z = {2,8:F2}   U = {3,8:F2}    目标  X = {4,8:F2}   Y = {5,8:F2}   Z = {6,8:F2}   U = {7,8:F2}",
                _curX, _curY, _curZ, _curU,
                _tgtX, _tgtY, _tgtZ, _tgtU);
        }

        // ============== 鼠标交互 ==============

        /// <summary>用户在 XYView 上点击/拖动 → 把点击坐标设为 XY 目标。</summary>
        private void XyView_TargetSetByMouse(object sender, PointF e)
        {
            _tgtX = e.X;
            _tgtY = e.Y;
        }

        // ============== 按钮处理 ==============

        /// <summary>回零：所有轴目标设为 0。</summary>
        private void BtnHome_Click(object sender, EventArgs e)
        {
            _tgtX = 0f;
            _tgtY = 0f;
            _tgtZ = 0f;
            _tgtU = 0f;
        }

        /// <summary>居中：所有轴目标设为范围中点。</summary>
        private void BtnCenter_Click(object sender, EventArgs e)
        {
            _tgtX = (xyView.RangeMin + xyView.RangeMax) * 0.5f;
            _tgtY = (xyView.RangeMin + xyView.RangeMax) * 0.5f;
            _tgtZ = (zBar.RangeMin + zBar.RangeMax) * 0.5f;
            _tgtU = (uBar.RangeMin + uBar.RangeMax) * 0.5f;
        }

        /// <summary>急停：当前值立刻冻结为目标值（运动立即停止）。</summary>
        private void BtnEStop_Click(object sender, EventArgs e)
        {
            _tgtX = _curX;
            _tgtY = _curY;
            _tgtZ = _curZ;
            _tgtU = _curU;
        }

        // ====================================================================
        // ============== 后期接入真 EtherCAT SDK 的步骤（文档）==============
        // ====================================================================
        //
        // 【背景】当前 HardwareForm 用 4 对 _cur/_tgt 字段模拟轴数据。
        //         接入真硬件时，把字段替换成 IAxisHardware 接口实现即可，
        //         UI 代码（按钮、XYView 显示、状态栏）几乎不变。
        //
        // 【步骤 1】新建接口 src/XyzController/Logic/Hardware/IAxisHardware.cs
        //
        //     public interface IAxisHardware
        //     {
        //         string Name { get; }
        //         float MinLimit { get; }
        //         float MaxLimit { get; }
        //         float CurrentPosition { get; }   // 读编码器（mm 或 度）
        //         float TargetPosition { get; }    // 最近一次 MoveTo 设置的目标
        //         bool IsMoving { get; }           // 是否在运动中
        //         bool IsConnected { get; }        // 硬件是否在线
        //         void MoveTo(float position);     // 下发运动指令（非阻塞）
        //         void EmergencyStop();            // 立即急停
        //         void Home();                     // 回机械原点
        //         event EventHandler StateChanged; // 状态变化通知（UI 订阅）
        //     }
        //
        // 【步骤 2】新建假实现 src/XyzController/Logic/Hardware/MockAxisHardware.cs
        //
        //     用 Timer 模拟硬件周期推进，每 20ms 让 CurrentPosition 逼近 TargetPosition。
        //     便于在没有硬件时调试 UI。开发期就用它。
        //
        // 【步骤 3】拿到 C# SDK 后，新建真实现 src/XyzController/Logic/Hardware/EtherCatAxisHardware.cs
        //
        //     public class EtherCatAxisHardware : IAxisHardware
        //     {
        //         private readonly int _slaveId;      // EtherCAT 从站 ID
        //         private readonly int _channel;      // 通道号（一轴多通道场景）
        //         private readonly IEtherCatSdk _sdk; // 你供应商提供的 SDK 接口
        //
        //         public EtherCatAxisHardware(string name, int slaveId, int channel, IEtherCatSdk sdk) {...}
        //
        //         public float CurrentPosition
        //         {
        //             get { return _sdk.ReadEncoder(_slaveId, _channel); }
        //         }
        //
        //         public void MoveTo(float position)
        //         {
        //             _sdk.MoveAbsolute(_slaveId, _channel, position, velocity: 50f);
        //         }
        //
        //         public void EmergencyStop()
        //         {
        //             _sdk.EmergencyStop(_slaveId);
        //         }
        //         // ... 其他成员类似
        //     }
        //
        // 【步骤 4】HardwareForm 极少改动
        //
        //     // 改前（模拟）：
        //     private float _curX, _tgtX;
        //
        //     // 改后（真硬件）：
        //     private readonly IAxisHardware _axisX;
        //
        //     // 构造函数里：
        //     //   _axisX = new EtherCatAxisHardware("X", slaveId: 1001, channel: 0, sdk);
        //     //   _axisX.StateChanged += delegate { RefreshSingleAxis(); };
        //
        //     // RefreshTimer_Tick 里：
        //     //   xyView.TargetX = _axisX.TargetPosition;   // 真硬件下发的目标
        //     //   xyView.Advance(0.3f);                     // UI 平滑显示
        //     //   不再需要 _curX += (_tgtX - _curX) * lerp  // 因为真硬件自己规划速度曲线
        //
        // 【步骤 5】用工厂切换开发/生产环境
        //
        //     public static class HardwareFactory
        //     {
        //         public static IAxisHardware CreateX()
        //         {
        //             #if DEBUG
        //                 return new MockAxisHardware("X", -100, 100);
        //             #else
        //                 return new EtherCatAxisHardware("X", slaveId: 1001, channel: 0, RealSdk.Instance);
        //             #endif
        //         }
        //     }
        //
        // 【关键收益】
        //   * UI（HardwareForm.cs）几乎不变 —— 只换字段类型
        //   * 单元测试可以针对 MockAxisHardware 写，不依赖真硬件
        //   * 切换开发/生产只改工厂，不动其他代码
        //
        // ====================================================================
    }
}
