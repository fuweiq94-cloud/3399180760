using System;
using XyzController.Logic;
using XyzController.Tests.Testing;

namespace XyzController.Tests.Tests
{
    /// <summary>
    /// AxisController 单轴业务类的测试。
    /// </summary>
    [TestFixture]
    public class AxisControllerTests
    {
        private AxisController _axis;

        [Setup]
        public void Setup()
        {
            // 每个测试前都创建一个新的 [-100, +100] 轴，初始在 0
            _axis = new AxisController("X", -100f, 100f, 0f);
        }

        // ============== 构造与初始状态 ==============

        [Test("构造函数设置的范围与初始值")]
        public void Constructor_SetsRangeAndInitial()
        {
            Assert.AreEqual("X", _axis.Name);
            Assert.AreEqual(-100f, _axis.Min, 0.001f);
            Assert.AreEqual(100f, _axis.Max, 0.001f);
            Assert.AreEqual(0f, _axis.Current, 0.001f);
            Assert.AreEqual(0f, _axis.Target, 0.001f);
        }

        [Test("初始时已达目标")]
        public void Initial_AtTarget()
        {
            Assert.IsTrue(_axis.AtTarget);
        }

        [Test("构造时初始值会被限位")]
        public void Constructor_InitialIsClamped()
        {
            AxisController a1 = new AxisController("X", 0f, 100f, 200f);
            Assert.AreEqual(100f, a1.Current, 0.001f, "超出上限应被钳到上限");

            AxisController a2 = new AxisController("X", 0f, 100f, -50f);
            Assert.AreEqual(0f, a2.Current, 0.001f, "低于下限应被钳到下限");
        }

        [Test("范围非法时抛异常")]
        public void Constructor_InvalidRange_Throws()
        {
            Assert.Throws<ArgumentException>(delegate
            {
                new AxisController("X", 100f, -100f);
            });
            Assert.Throws<ArgumentException>(delegate
            {
                new AxisController("X", 50f, 50f);
            });
        }

        // ============== SetTarget ==============

        [Test("SetTarget 改变目标，不影响当前值")]
        public void SetTarget_ChangesTargetNotCurrent()
        {
            _axis.SetTarget(50f);
            Assert.AreEqual(50f, _axis.Target, 0.001f);
            Assert.AreEqual(0f, _axis.Current, 0.001f, "设目标后 Current 应不变");
            Assert.IsFalse(_axis.AtTarget);
        }

        [Test("SetTarget 自动限位")]
        public void SetTarget_ClampsToRange()
        {
            _axis.SetTarget(9999f);
            Assert.AreEqual(100f, _axis.Target, 0.001f, "超上限应钳到 100");

            _axis.SetTarget(-9999f);
            Assert.AreEqual(-100f, _axis.Target, 0.001f, "低于下限应钳到 -100");
        }

        [Test("设置与当前相同的目标不会触发 Changed")]
        public void SetTarget_SameValue_NoEvent()
        {
            bool fired = false;
            _axis.Changed += delegate { fired = true; };
            _axis.SetTarget(0f);   // 已经是 0
            Assert.IsFalse(fired, "目标没变时不应触发 Changed");
        }

        // ============== Step ==============

        [Test("Step 加减步长")]
        public void Step_AddsDelta()
        {
            _axis.Step(10);
            Assert.AreEqual(10f, _axis.Target, 0.001f);

            _axis.Step(10);
            Assert.AreEqual(20f, _axis.Target, 0.001f);

            _axis.Step(-30);
            Assert.AreEqual(-10f, _axis.Target, 0.001f);
        }

        [Test("Step 也会限位")]
        public void Step_ClampsToRange()
        {
            _axis.SetTarget(95f);
            _axis.Step(20);   // 应该是 115，钳到 100
            Assert.AreEqual(100f, _axis.Target, 0.001f);
        }

        // ============== 特殊位置 ==============

        [Test("SetToCenter 移到中点")]
        public void SetToCenter()
        {
            _axis.SetToCenter();
            Assert.AreEqual(0f, _axis.Target, 0.001f, "[-100,100] 中点是 0");

            AxisController a = new AxisController("X", 0f, 100f);
            a.SetToCenter();
            Assert.AreEqual(50f, a.Target, 0.001f);
        }

        [Test("SetToMin / SetToMax")]
        public void SetToMin_SetToMax()
        {
            _axis.SetToMin();
            Assert.AreEqual(-100f, _axis.Target, 0.001f);

            _axis.SetToMax();
            Assert.AreEqual(100f, _axis.Target, 0.001f);
        }

        [Test("ResetToOrigin 回到 0（如果在范围内）")]
        public void ResetToOrigin_WhenZeroInRange()
        {
            _axis.SetTarget(50f);
            _axis.ResetToOrigin();
            Assert.AreEqual(0f, _axis.Target, 0.001f);
        }

        [Test("ResetToOrigin 0 不在范围时回到中点")]
        public void ResetToOrigin_WhenZeroNotInRange()
        {
            AxisController a = new AxisController("X", 100f, 200f);
            a.ResetToOrigin();
            Assert.AreEqual(150f, a.Target, 0.001f, "0 不在范围时回中点");
        }

        // ============== 动画 Advance ==============

        [Test("Advance 让 Current 逼近 Target")]
        public void Advance_MovesCurrentTowardTarget()
        {
            _axis.SetTarget(100f);

            // lerp = 0.5，每次走剩余距离的一半
            _axis.Advance(0.5f);
            Assert.AreEqual(50f, _axis.Current, 0.001f);

            _axis.Advance(0.5f);
            Assert.AreEqual(75f, _axis.Current, 0.001f);

            _axis.Advance(0.5f);
            Assert.AreEqual(87.5f, _axis.Current, 0.001f);
        }

        [Test("Advance lerp=1 时立即到位")]
        public void Advance_FullLerp_ReachesTarget()
        {
            _axis.SetTarget(80f);
            _axis.Advance(1f);
            Assert.AreEqual(80f, _axis.Current, 0.001f);
            Assert.IsTrue(_axis.AtTarget);
        }

        [Test("Advance lerp=0 时不动")]
        public void Advance_ZeroLerp_DoesNotMove()
        {
            _axis.SetTarget(50f);
            _axis.Advance(0f);
            Assert.AreEqual(0f, _axis.Current, 0.001f, "lerp=0 不应移动");
            Assert.IsFalse(_axis.AtTarget);
        }

        [Test("Advance 多次后最终到达目标")]
        public void Advance_EventuallyReachesTarget()
        {
            _axis.SetTarget(100f);
            for (int i = 0; i < 200; i++)
            {
                _axis.Advance(0.1f);
            }
            Assert.IsTrue(_axis.AtTarget, "迭代 200 次后应到位");
            Assert.AreEqual(100f, _axis.Current, 0.001f);
        }

        [Test("Advance 反向移动也正常")]
        public void Advance_NegativeDirection()
        {
            _axis.SetTarget(-100f);
            _axis.Advance(0.5f);
            Assert.AreEqual(-50f, _axis.Current, 0.001f);

            _axis.Advance(0.5f);
            Assert.AreEqual(-75f, _axis.Current, 0.001f);
        }

        // ============== 事件 ==============

        [Test("SetTarget 触发 Changed 事件")]
        public void SetTarget_FiresChanged()
        {
            int count = 0;
            _axis.Changed += delegate { count++; };
            _axis.SetTarget(10f);
            Assert.AreEqual(1, count);

            _axis.SetTarget(20f);
            Assert.AreEqual(2, count);
        }

        [Test("Advance 推进时触发 Changed")]
        public void Advance_FiresChanged()
        {
            int count = 0;
            _axis.SetTarget(50f);   // 这次已经触发一次
            _axis.Changed += delegate { count++; };
            _axis.Advance(0.5f);
            Assert.AreEqual(1, count, "推进后应触发一次");
        }
    }
}
