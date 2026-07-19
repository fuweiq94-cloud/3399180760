using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using XyzController.Controls;
using XyzController.Logic;

namespace XyzController
{
    /// <summary>
    /// 点位跳转窗体（纯 UI 层）。
    /// 用于工控机点位跳转操作：输入目标坐标或从预设列表选取，一键跳转。
    /// 所有运动逻辑委托给 XyzControllerHub（业务层），本类只负责：
    /// 捕获用户输入 → 转发给 hub → 监听 hub 变化 → 刷新控件。
    /// </summary>
    /// <remarks>
    /// 兼容性说明：工控机上的旧 .NET 编译器不支持 Lambda 表达式 (s,e) => ...
    /// 因此所有事件订阅都使用命名方法 + 显式委托构造（new EventHandler(...)）。
    /// </remarks>
    public partial class PointJumpForm : Form
    {
        // —— 业务核心 ——
        private readonly XyzControllerHub _hub;

        // —— UI 同步锁 ——
        private bool _syncing;

        // —— 预设点位列表（纯 UI 数据）——
        private readonly List<PresetPoint> _presets = new List<PresetPoint>();
        private int _presetCounter;

        public PointJumpForm()
        {
            InitializeComponent();

            // 1) 创建业务层（范围与 XYView 一致）
            _hub = new XyzControllerHub(
                xyView.RangeMin, xyView.RangeMax,
                xyView.RangeMin, xyView.RangeMax,
                -50f, 100f);
            _hub.SpeedSetting = trbSpeed.Value;

            // 2) 绑定 UI 事件
            HookEvents();

            // 3) 监听 hub 变化 → 刷新 UI
            _hub.Changed += new EventHandler(Hub_Changed);

            // 4) 添加默认预设点位
            AddDefaultPresets();

            // 5) 初始刷新
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
        }

        // ============== 预设点位数据结构 ==============

        /// <summary>预设点位（纯 UI 层数据结构）。</summary>
        private class PresetPoint
        {
            public string Name;
            public float X;
            public float Y;
            public float Z;
        }

        // ============== 事件绑定 ==============

        private void HookEvents()
        {
            btnJump.Click += new EventHandler(BtnJump_Click);
            btnSavePos.Click += new EventHandler(BtnSavePos_Click);
            btnDeletePos.Click += new EventHandler(BtnDeletePos_Click);
            btnGotoSelected.Click += new EventHandler(BtnGotoSelected_Click);
            trbSpeed.Scroll += new EventHandler(TrbSpeed_Scroll);
            lvPresets.DoubleClick += new EventHandler(LvPresets_DoubleClick);
            xyView.TargetSetByMouse += new EventHandler<PointF>(XyView_TargetSetByMouse);
            this.KeyDown += new KeyEventHandler(PointJumpForm_KeyDown);
        }

        // ============== UI → 业务 ==============

        /// <summary>跳转到用户输入的目标坐标。</summary>
        private void BtnJump_Click(object sender, EventArgs e)
        {
            float x = (float)nudTargetX.Value;
            float y = (float)nudTargetY.Value;
            float z = (float)nudTargetZ.Value;
            _hub.SetTarget(x, y, z);
            UpdateStatus("跳转到 X=" + x.ToString("F2") + " Y=" + y.ToString("F2") + " Z=" + z.ToString("F2"));
        }

        /// <summary>保存当前目标位置为预设。</summary>
        private void BtnSavePos_Click(object sender, EventArgs e)
        {
            _presetCounter++;
            PresetPoint pt = new PresetPoint();
            pt.Name = "P" + _presetCounter.ToString();
            pt.X = _hub.X.Target;
            pt.Y = _hub.Y.Target;
            pt.Z = _hub.Z.Target;
            _presets.Add(pt);
            RefreshPresetList();
            UpdateStatus("已保存预设 " + pt.Name);
        }

        /// <summary>删除选中的预设。</summary>
        private void BtnDeletePos_Click(object sender, EventArgs e)
        {
            if (lvPresets.SelectedIndices.Count == 0) return;
            int idx = lvPresets.SelectedIndices[0];
            if (idx >= 0 && idx < _presets.Count)
            {
                string name = _presets[idx].Name;
                _presets.RemoveAt(idx);
                RefreshPresetList();
                UpdateStatus("已删除预设 " + name);
            }
        }

        /// <summary>跳转到选中的预设点位。</summary>
        private void BtnGotoSelected_Click(object sender, EventArgs e)
        {
            JumpToSelectedPreset();
        }

        /// <summary>双击列表项直接跳转。</summary>
        private void LvPresets_DoubleClick(object sender, EventArgs e)
        {
            JumpToSelectedPreset();
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
        private void PointJumpForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnJump_Click(sender, EventArgs.Empty);
                e.Handled = true;
            }
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
                // DRO 实时坐标
                droX.SetValue(_hub.X.Current);
                droY.SetValue(_hub.Y.Current);
                droZ.SetValue(_hub.Z.Current);

                // XY 视图目标
                xyView.TargetX = _hub.X.Target;
                xyView.TargetY = _hub.Y.Target;
            }
            finally { _syncing = false; }
        }

        // ============== 动画 Tick ==============

        private void animTimer_Tick(object sender, EventArgs e)
        {
            _hub.Advance();
            xyView.Advance(_hub.CurrentLerpFraction);

            // 更新 DRO 为当前值
            droX.SetValue(_hub.X.Current);
            droY.SetValue(_hub.Y.Current);
            droZ.SetValue(_hub.Z.Current);

            UpdateStatusLive();
        }

        // ============== 辅助方法 ==============

        private void JumpToSelectedPreset()
        {
            if (lvPresets.SelectedIndices.Count == 0)
            {
                UpdateStatus("请先选择一个预设点位");
                return;
            }
            int idx = lvPresets.SelectedIndices[0];
            if (idx >= 0 && idx < _presets.Count)
            {
                PresetPoint pt = _presets[idx];
                _hub.SetTarget(pt.X, pt.Y, pt.Z);
                UpdateStatus("跳转到预设 " + pt.Name + " (X=" + pt.X.ToString("F2")
                    + " Y=" + pt.Y.ToString("F2") + " Z=" + pt.Z.ToString("F2") + ")");
            }
        }

        private void RefreshPresetList()
        {
            lvPresets.Items.Clear();
            for (int i = 0; i < _presets.Count; i++)
            {
                PresetPoint pt = _presets[i];
                ListViewItem item = new ListViewItem(pt.Name);
                item.SubItems.Add(pt.X.ToString("F2"));
                item.SubItems.Add(pt.Y.ToString("F2"));
                item.SubItems.Add(pt.Z.ToString("F2"));
                lvPresets.Items.Add(item);
            }
        }

        private void AddDefaultPresets()
        {
            PresetPoint p1 = new PresetPoint();
            p1.Name = "原点";
            p1.X = 0f;
            p1.Y = 0f;
            p1.Z = 0f;
            _presets.Add(p1);

            PresetPoint p2 = new PresetPoint();
            p2.Name = "中心";
            p2.X = 0f;
            p2.Y = 0f;
            p2.Z = 50f;
            _presets.Add(p2);

            PresetPoint p3 = new PresetPoint();
            p3.Name = "A工位";
            p3.X = 30f;
            p3.Y = 40f;
            p3.Z = 10f;
            _presets.Add(p3);

            PresetPoint p4 = new PresetPoint();
            p4.Name = "B工位";
            p4.X = -50f;
            p4.Y = 60f;
            p4.Z = 20f;
            _presets.Add(p4);

            _presetCounter = _presets.Count;
            RefreshPresetList();
        }

        private void UpdateStatusLive()
        {
            lblStatus.Text = string.Format(
                "当前 X={0:F2}  Y={1:F2}  Z={2:F2}   →  目标 X={3:F2}  Y={4:F2}  Z={5:F2}",
                _hub.X.Current, _hub.Y.Current, _hub.Z.Current,
                _hub.X.Target, _hub.Y.Target, _hub.Z.Target);
        }

        private void UpdateStatus(string msg)
        {
            lblStatus.Text = msg;
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
