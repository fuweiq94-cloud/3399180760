using XyzController.Logic;
using XyzController.Tests.Testing;

namespace XyzController.Tests.Tests
{
    /// <summary>
    /// XyzControllerHub 三轴组合控制器的测试。
    /// </summary>
    [TestFixture]
    public class XyzControllerHubTests
    {
        private XyzControllerHub _hub;

        [Setup]
        public void Setup()
        {
            _hub = new XyzControllerHub(-100f, 100f, -100f, 100f, -50f, 100f);
        }

        // ============== 构造 ==============

        [Test("构造后 X/Y/Z 都有正确的范围")]
        public void Constructor_CreatesThreeAxes()
        {
            Assert.IsNotNull(_hub.X);
            Assert.IsNotNull(_hub.Y);
            Assert.IsNotNull(_hub.Z);

            Assert.AreEqual(-100f, _hub.X.Min, 0.001f);
            Assert.AreEqual(100f, _hub.X.Max, 0.001f);

            Assert.AreEqual(-50f, _hub.Z.Min, 0.001f);
            Assert.AreEqual(100f, _hub.Z.Max, 0.001f);
        }

        [Test("默认速度档位是 20")]
        public void DefaultSpeed_Is20()
        {
            Assert.AreEqual(20, _hub.SpeedSetting);
        }

        // ============== 速度换算 ==============

        [Test("CurrentLerpFraction 在 SpeedSetting=0 时接近 0.02")]
        public void LerpFraction_MinSpeed()
        {
            _hub.SpeedSetting = 0;
            Assert.AreEqual(0.02f, _hub.CurrentLerpFraction, 0.0001f);
        }

        [Test("CurrentLerpFraction 在 SpeedSetting=100 时等于 1.0")]
        public void LerpFraction_MaxSpeed()
        {
            _hub.SpeedSetting = 100;
            Assert.AreEqual(1.0f, _hub.CurrentLerpFraction, 0.0001f);
        }

        [Test("Lerp 单调递增（速度档位越高，移动越快）")]
        public void LerpFraction_IsMonotonic()
        {
            _hub.SpeedSetting = 0;
            float v0 = _hub.CurrentLerpFraction;
            _hub.SpeedSetting = 50;
            float v50 = _hub.CurrentLerpFraction;
            _hub.SpeedSetting = 100;
            float v100 = _hub.CurrentLerpFraction;

            Assert.IsTrue(v0 < v50, "速度 0 < 速度 50");
            Assert.IsTrue(v50 < v100, "速度 50 < 速度 100");
        }

        // ============== 批量操作 ==============

        [Test("SetTarget 同时设置三轴")]
        public void SetTarget_SetsAllAxes()
        {
            _hub.SetTarget(10f, 20f, 30f);
            Assert.AreEqual(10f, _hub.X.Target, 0.001f);
            Assert.AreEqual(20f, _hub.Y.Target, 0.001f);
            Assert.AreEqual(30f, _hub.Z.Target, 0.001f);
        }

        [Test("SetTarget 自动限位")]
        public void SetTarget_ClampsEachAxis()
        {
            _hub.SetTarget(999f, -999f, 999f);
            Assert.AreEqual(100f, _hub.X.Target, 0.001f);
            Assert.AreEqual(-100f, _hub.Y.Target, 0.001f);
            Assert.AreEqual(100f, _hub.Z.Target, 0.001f);
        }

        [Test("ResetToOrigin 三轴回到 0")]
        public void ResetToOrigin()
        {
            _hub.SetTarget(50f, -50f, 30f);
            _hub.ResetToOrigin();
            Assert.AreEqual(0f, _hub.X.Target, 0.001f);
            Assert.AreEqual(0f, _hub.Y.Target, 0.001f);
            Assert.AreEqual(0f, _hub.Z.Target, 0.001f);
        }

        [Test("SetToCenter 三轴移到中点")]
        public void SetToCenter()
        {
            _hub.SetTarget(50f, 50f, 50f);
            _hub.SetToCenter();
            Assert.AreEqual(0f, _hub.X.Target, 0.001f, "X 中点 = 0");
            Assert.AreEqual(0f, _hub.Y.Target, 0.001f);
            Assert.AreEqual(25f, _hub.Z.Target, 0.001f, "Z 范围 [-50, 100]，中点 = 25");
        }

        [Test("SetRandomTarget 三个轴目标都在各自范围内")]
        public void SetRandomTarget_StaysInRange()
        {
            System.Random rng = new System.Random(42);
            for (int i = 0; i < 50; i++)
            {
                _hub.SetRandomTarget(rng);
                Assert.IsTrue(_hub.X.Target >= _hub.X.Min && _hub.X.Target <= _hub.X.Max, "X 越界");
                Assert.IsTrue(_hub.Y.Target >= _hub.Y.Min && _hub.Y.Target <= _hub.Y.Max, "Y 越界");
                Assert.IsTrue(_hub.Z.Target >= _hub.Z.Min && _hub.Z.Target <= _hub.Z.Max, "Z 越界");
            }
        }

        // ============== 动画 ==============

        [Test("Advance 推进三轴（按二次曲线公式算 lerp）")]
        public void Advance_MovesAllAxes()
        {
            _hub.SetTarget(100f, 100f, 100f);
            _hub.SpeedSetting = 50;
            _hub.Advance();

            // SpeedSetting=50 → lerp = 0.02 + 0.5^2 * 0.98 = 0.265
            // 0 + (100-0) * 0.265 = 26.5
            float expected = 100f * _hub.CurrentLerpFraction;
            Assert.AreEqual(expected, _hub.X.Current, 0.01f);
            Assert.AreEqual(expected, _hub.Y.Current, 0.01f);
            Assert.AreEqual(expected, _hub.Z.Current, 0.01f);
        }

        // ============== 事件 ==============

        [Test("任一轴变化时 hub.Changed 触发")]
        public void HubChanged_Fires_WhenAxisChanged()
        {
            int count = 0;
            _hub.Changed += delegate { count++; };

            _hub.X.SetTarget(10f);
            _hub.Y.SetTarget(20f);
            _hub.Z.SetTarget(30f);

            Assert.AreEqual(3, count, "三轴各变一次，应触发 3 次");
        }

        [Test("Advance 推进时触发 hub.Changed（三轴各一次，共 3 次）")]
        public void HubChanged_Fires_OnAdvance()
        {
            int count = 0;
            _hub.SetTarget(50f, 50f, 50f);
            _hub.Changed += delegate { count++; };

            _hub.Advance();
            Assert.AreEqual(3, count, "三轴一起 Advance，hub.Changed 应被触发 3 次（X/Y/Z 各一次）");
        }
    }
}
