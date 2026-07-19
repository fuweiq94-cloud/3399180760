using System;

namespace XyzController.Logic
{
    /// <summary>
    /// XYZ 三轴的统一控制器（业务层）。
    /// 把 3 个 AxisController 组合起来，统一暴露给 UI 层使用。
    /// 还封装了"速度档位 → 插值系数"的换算，让动画更平滑。
    /// </summary>
    public class XyzControllerHub
    {
        // —— 三轴 ——
        public AxisController X { get; private set; }
        public AxisController Y { get; private set; }
        public AxisController Z { get; private set; }

        /// <summary>
        /// 速度档位，取值 [0, 100]。0 = 最慢，100 = 瞬时到位。
        /// 内部会换算成插值系数。
        /// </summary>
        public int SpeedSetting { get; set; }

        /// <summary>任一轴的当前值或目标值变化时触发。</summary>
        public event EventHandler Changed;

        public XyzControllerHub(float xMin, float xMax,
                                float yMin, float yMax,
                                float zMin, float zMax)
        {
            X = new AxisController("X", xMin, xMax);
            Y = new AxisController("Y", yMin, yMax);
            Z = new AxisController("Z", zMin, zMax);

            // 工控机旧编译器不支持 Lambda（=>），这里用命名方法 + 委托构造。
            X.Changed += new EventHandler(ForwardAxisChanged);
            Y.Changed += new EventHandler(ForwardAxisChanged);
            Z.Changed += new EventHandler(ForwardAxisChanged);

            SpeedSetting = 20;
        }

        /// <summary>把三个子轴的 Changed 事件转发为 Hub 自身的 Changed。</summary>
        private void ForwardAxisChanged(object sender, EventArgs e)
        {
            OnChanged();
        }

        // ============== 批量操作 ==============

        /// <summary>一次性设置 X / Y / Z 三个目标。</summary>
        public void SetTarget(float x, float y, float z)
        {
            X.SetTarget(x);
            Y.SetTarget(y);
            Z.SetTarget(z);
        }

        /// <summary>三个轴都回到原点 (0, 0, 0)。</summary>
        public void ResetToOrigin()
        {
            X.ResetToOrigin();
            Y.ResetToOrigin();
            Z.ResetToOrigin();
        }

        /// <summary>三个轴都移到各自范围的中点。</summary>
        public void SetToCenter()
        {
            X.SetToCenter();
            Y.SetToCenter();
            Z.SetToCenter();
        }

        /// <summary>三个轴都随机选一个目标（用于演示）。</summary>
        public void SetRandomTarget(Random rng)
        {
            X.SetTarget(rng.Next((int)X.Min, (int)X.Max + 1));
            Y.SetTarget(rng.Next((int)Y.Min, (int)Y.Max + 1));
            Z.SetTarget(rng.Next((int)Z.Min, (int)Z.Max + 1));
        }

        // ============== 动画 ==============

        /// <summary>把 [0,100] 的速度档位换算成 [0.02, 1.0] 的插值系数。</summary>
        public float CurrentLerpFraction
        {
            get
            {
                float k = SpeedSetting / 100f;
                // 二次曲线：低速段更柔
                return 0.02f + k * k * 0.98f;
            }
        }

        /// <summary>推进一帧动画，让三个轴的 Current 都向 Target 靠近。</summary>
        public void Advance()
        {
            float lerp = CurrentLerpFraction;
            X.Advance(lerp);
            Y.Advance(lerp);
            Z.Advance(lerp);
        }

        protected virtual void OnChanged()
        {
            EventHandler h = Changed;
            if (h != null) h(this, EventArgs.Empty);
        }
    }
}
