using System;
using System.Windows.Forms;
using ProcessModules;

namespace DOMOPlatform
{
    /// <summary>
    /// 模拟运动控制服务（实现 IMotionService）。
    /// 用于测试和演示工艺模组，模拟 4 轴（X/Y/Z/U）运动：
    /// 收到移动指令后，内部 Timer 以插值方式让 Actual 逼近 Target，
    /// 并通过 PositionUpdated 事件将位置推送给前端。
    /// 
    /// 注意：本类仅用于开发/测试环境。正式对接真实硬件时，
    /// 应替换为实现了 IMotionService 的真实驱动服务。
    /// </summary>
    public class SimulatedMotionService : IMotionService
    {
        private const int AxisCountValue = 4;

        private bool _isConnected;
        private float _speed;
        private readonly float[] _target;
        private readonly float[] _actual;
        private readonly bool[] _isMoving;
        private readonly bool[] _isHomed;
        private Timer _timer;

        /// <summary>实际位置实时更新事件（前端订阅以刷新 DRO / 视图）。</summary>
        public event EventHandler<AxisPositionEventArgs> PositionUpdated;

        public SimulatedMotionService()
        {
            _target = new float[AxisCountValue];
            _actual = new float[AxisCountValue];
            _isMoving = new bool[AxisCountValue];
            _isHomed = new bool[AxisCountValue];
            _speed = 20f;
        }

        // ============== 指令下发 ==============

        /// <summary>
        /// 执行运动控制指令（统一入口）。
        /// 模拟服务按指令类型更新内部目标位置。
        /// </summary>
        public bool ExecuteCommand(MotionCommand command)
        {
            if (!_isConnected || command == null)
                return false;

            switch (command.CommandType)
            {
                case MotionCommandType.MoveAbsolute:
                    if (command.Positions != null)
                    {
                        for (int i = 0; i < AxisCountValue && i < command.Positions.Length; i++)
                            _target[i] = command.Positions[i];
                    }
                    if (command.Speed > 0)
                        _speed = command.Speed;
                    break;

                case MotionCommandType.MoveRelative:
                    if (command.Positions != null)
                    {
                        for (int i = 0; i < AxisCountValue && i < command.Positions.Length; i++)
                            _target[i] += command.Positions[i];
                    }
                    break;

                case MotionCommandType.Home:
                    for (int i = 0; i < AxisCountValue; i++)
                    {
                        _target[i] = 0f;
                        _isHomed[i] = true;
                    }
                    break;

                case MotionCommandType.Stop:
                case MotionCommandType.EmergencyStop:
                    // 冻结：目标 = 当前位置
                    for (int i = 0; i < AxisCountValue; i++)
                        _target[i] = _actual[i];
                    break;

                case MotionCommandType.SetSpeed:
                    _speed = command.Speed;
                    break;

                case MotionCommandType.SetCoordinateSystem:
                    // 模拟服务不处理坐标系，直接接受
                    break;

                case MotionCommandType.Enable:
                case MotionCommandType.Disable:
                case MotionCommandType.Reset:
                    // 模拟服务直接接受
                    break;
            }
            return true;
        }

        // ============== 位置反馈 ==============

        /// <summary>获取当前位置快照。</summary>
        public AxisPosition GetPosition()
        {
            return BuildPosition();
        }

        // ============== 状态 ==============

        /// <summary>后端服务是否已连接。</summary>
        public bool IsConnected
        {
            get { return _isConnected; }
        }

        /// <summary>管理的轴数量（X/Y/Z/U = 4）。</summary>
        public int AxisCount
        {
            get { return AxisCountValue; }
        }

        // ============== 生命周期 ==============

        /// <summary>打开连接（启动模拟定时器）。</summary>
        public bool Open()
        {
            if (_isConnected) return true;

            _isConnected = true;

            // 创建 UI 线程定时器，每 20ms 推进一帧
            _timer = new Timer();
            _timer.Interval = 20;
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();

            return true;
        }

        /// <summary>关闭连接（停止模拟定时器）。</summary>
        public void CloseService()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            _isConnected = false;
        }

        // ============== 内部模拟逻辑 ==============

        /// <summary>
        /// 定时器回调：让 Actual 以插值方式逼近 Target，然后推送位置。
        /// 插值系数由速度档位决定：lerp = 0.02 + (speed/100)^2 * 0.98
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isConnected) return;

            // 计算插值系数
            float s = _speed / 100f;
            float lerp = 0.02f + s * s * 0.98f;

            bool anyMoved = false;
            for (int i = 0; i < AxisCountValue; i++)
            {
                float diff = _target[i] - _actual[i];
                if (Math.Abs(diff) < 0.0005f)
                {
                    // 已到位
                    if (_actual[i] != _target[i])
                    {
                        _actual[i] = _target[i];
                        anyMoved = true;
                    }
                    _isMoving[i] = false;
                }
                else
                {
                    _actual[i] += diff * lerp;
                    _isMoving[i] = true;
                    anyMoved = true;
                }
            }

            // 只有位置变化时才推送（减少不必要的事件）
            if (anyMoved)
            {
                AxisPosition pos = BuildPosition();
                EventHandler<AxisPositionEventArgs> h = PositionUpdated;
                if (h != null)
                    h(this, new AxisPositionEventArgs(pos));
            }
        }

        /// <summary>构建当前位置快照。</summary>
        private AxisPosition BuildPosition()
        {
            AxisPosition pos = new AxisPosition();
            pos.Target = (float[])_target.Clone();
            pos.Actual = (float[])_actual.Clone();
            pos.IsMoving = (bool[])_isMoving.Clone();
            pos.IsHomed = (bool[])_isHomed.Clone();
            pos.IsAlarmed = new bool[AxisCountValue];
            pos.Timestamp = DateTime.Now;
            return pos;
        }
    }
}
