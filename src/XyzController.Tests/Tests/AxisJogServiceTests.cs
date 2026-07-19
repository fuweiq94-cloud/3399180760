using System;
using XyzController.Logic;
using XyzController.Tests.Testing;

namespace XyzController.Tests.Tests
{
    /// <summary>
    /// AxisJogService 的测试 —— 寸动 / 连续 / 急停 三种核心场景。
    /// </summary>
    [TestFixture]
    public class AxisJogServiceTests
    {
        private AxisController _axis;
        private AxisJogService _service;

        [Setup]
        public void Setup()
        {
            // [-100, 100] 的轴，初始在 0；用真实轴测试
            _axis = new AxisController("X", -100f, 100f, 0f);
            _service = new AxisJogService(_axis);
        }

        // ============== 初始状态 ==============

        [Test("新建后默认是寸动模式")]
        public void Default_IsIncrementalMode()
        {
            Assert.AreEqual(JogMode.Incremental, _service.Mode);
        }

        [Test("默认步长是 1.0")]
        public void Default_StepIs1()
        {
            Assert.AreEqual(1.0f, _service.StepDistance, 0.001f);
        }

        [Test("初始时不在 JOG 中")]
        public void Initial_NotJogging()
        {
            Assert.IsFalse(_service.IsJogging);
        }

        // ============== 寸动模式 ==============

        [Test("寸动：按一次走一个步长")]
        public void Incremental_SinglePress_MovesOneStep()
        {
            _service.SetStepDistance(5f);
            _service.OnJogStart(+1);

            Assert.AreEqual(5f, _axis.Target, 0.001f);
            Assert.IsTrue(_service.IsJogging);
        }

        [Test("寸动：连续按多次累计步长")]
        public void Incremental_MultiplePress_Accumulates()
        {
            _service.SetStepDistance(2f);
            _service.OnJogStart(+1);
            _service.OnJogStart(+1);
            _service.OnJogStart(+1);

            Assert.AreEqual(6f, _axis.Target, 0.001f, "按 3 次 +1，累计 6");
        }

        [Test("寸动：反方向按减步长")]
        public void Incremental_NegativeDirection()
        {
            _service.SetStepDistance(3f);
            _service.OnJogStart(-1);
            Assert.AreEqual(-3f, _axis.Target, 0.001f);

            _service.OnJogStart(-1);
            Assert.AreEqual(-6f, _axis.Target, 0.001f);
        }

        [Test("寸动：OnJogStop 不会撤销目标")]
        public void Incremental_Stop_DoesNotChangeTarget()
        {
            _service.SetStepDistance(10f);
            _service.OnJogStart(+1);
            _service.OnJogStop();

            Assert.AreEqual(10f, _axis.Target, 0.001f, "寸动模式下松开不影响目标");
            Assert.IsFalse(_service.IsJogging);
        }

        [Test("寸动：步长受轴范围限位")]
        public void Incremental_StepClampedToRange()
        {
            _service.SetStepDistance(50f);
            _service.OnJogStart(+1);   // +50
            _service.OnJogStart(+1);   // +100
            _service.OnJogStart(+1);   // +150 → 钳到 100

            Assert.AreEqual(100f, _axis.Target, 0.001f);
        }

        // ============== 连续模式 ==============

        [Test("连续：按下正向按钮目标设为正限位")]
        public void Continuous_PositivePress_SetsTargetToMax()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(+1);

            Assert.AreEqual(100f, _axis.Target, 0.001f, "连续正向：目标应该是上限");
            Assert.IsTrue(_service.IsJogging);
        }

        [Test("连续：按下负向按钮目标设为负限位")]
        public void Continuous_NegativePress_SetsTargetToMin()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(-1);

            Assert.AreEqual(-100f, _axis.Target, 0.001f);
        }

        [Test("连续：松开按钮把目标冻结在当前值")]
        public void Continuous_Stop_FreezesTargetAtCurrent()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(+1);     // 目标=100
            _axis.Advance(0.5f);          // 推进到 50
            Assert.AreEqual(50f, _axis.Current, 0.001f);

            _service.OnJogStop();

            Assert.AreEqual(50f, _axis.Target, 0.001f, "松开后目标应冻结在当前值 50");
            Assert.AreEqual(50f, _axis.Current, 0.001f, "Current 不变");
            Assert.IsFalse(_service.IsJogging);
        }

        [Test("连续：松开后再 Advance 不会继续移动")]
        public void Continuous_AfterStop_AdvanceNoEffect()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(+1);
            _axis.Advance(0.3f);
            float curAfterFirstAdvance = _axis.Current;

            _service.OnJogStop();
            _axis.Advance(0.5f);   // 应该不动
            _axis.Advance(0.5f);
            _axis.Advance(0.5f);

            Assert.AreEqual(curAfterFirstAdvance, _axis.Current, 0.001f,
                "松开后继续推进，Current 应保持不变");
        }

        [Test("连续：当前未推进时松开，目标和当前都还是 0")]
        public void Continuous_Stop_Immediately_Stays()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(+1);
            // 不调 Advance
            _service.OnJogStop();

            Assert.AreEqual(0f, _axis.Target, 0.001f);
            Assert.AreEqual(0f, _axis.Current, 0.001f);
        }

        // ============== 模式切换 ==============

        [Test("切换模式时如果在 JOG 中会停止")]
        public void ModeChange_StopsActiveJog()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(+1);
            _axis.Advance(0.2f);

            // 模式切换：内部应该先停
            _service.SetMode(JogMode.Incremental);

            Assert.IsFalse(_service.IsJogging, "切换模式后应停止");
            Assert.AreEqual(_axis.Current, _axis.Target, 0.001f, "目标应冻结在当前值");
        }

        [Test("切到寸动后按一下走一步")]
        public void AfterSwitchToIncremental_ButtonActsAsStep()
        {
            _service.SetMode(JogMode.Continuous);
            _service.SetMode(JogMode.Incremental);

            _service.SetStepDistance(7f);
            _service.OnJogStart(+1);
            Assert.AreEqual(7f, _axis.Target, 0.001f);
        }

        // ============== 急停 ==============

        [Test("急停在寸动模式下冻结目标")]
        public void EmergencyStop_InIncremental()
        {
            _service.SetStepDistance(50f);
            _service.OnJogStart(+1);
            Assert.AreEqual(50f, _axis.Target, 0.001f);

            _service.EmergencyStop();
            Assert.AreEqual(_axis.Current, _axis.Target, 0.001f, "急停：目标应等于当前");
            Assert.IsFalse(_service.IsJogging);
        }

        [Test("急停在连续模式下立即停")]
        public void EmergencyStop_InContinuous()
        {
            _service.SetMode(JogMode.Continuous);
            _service.OnJogStart(+1);
            _axis.Advance(0.4f);   // 推进到 40

            _service.EmergencyStop();

            Assert.AreEqual(40f, _axis.Target, 0.001f, "急停：目标冻结在当前");
            Assert.AreEqual(40f, _axis.Current, 0.001f);
        }

        // ============== 参数校验 ==============

        [Test("步长必须 > 0")]
        public void SetStep_ZeroOrNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.SetStepDistance(0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.SetStepDistance(-1f));
        }

        [Test("方向必须是 +1 或 -1")]
        public void OnJogStart_InvalidDirection_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.OnJogStart(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.OnJogStart(2));
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.OnJogStart(-2));
        }

        [Test("Axis 属性返回同一对象")]
        public void Axis_ReturnsSameReference()
        {
            Assert.AreSame(_axis, _service.Axis);
        }
    }
}
