using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InterfaceDefine;
using MainModule;
using ProcessModules;

namespace DOMOPlatform
{
    /// <summary>
    /// DOMO 模拟平台主界面。
    /// 模拟真实 DOMO 上位机平台的核心能力：
    /// 加载工艺模组、显示运行界面、注入模拟运动服务、接收报警。
    /// 
    /// 启动流程：创建模拟服务 → 初始化模组 → 注入服务 → 订阅报警 → 显示界面
    /// 关闭流程：退订报警 → 保存参数 → 关闭模组 → 断开服务
    /// </summary>
    public partial class MainForm : Form
    {
        private SimulatedMotionService _simService;
        private bool _modulesInitialized;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载时执行完整的平台启动流程。
        /// VS 设计器不会执行此方法（只执行构造函数），避免设计时异常。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            StartPlatform();
        }

        /// <summary>平台启动流程。</summary>
        private void StartPlatform()
        {
            try
            {
                // 1. 创建模拟运动服务并连接
                _simService = new SimulatedMotionService();
                _simService.Open();
                _simService.PositionUpdated += new EventHandler<AxisPositionEventArgs>(SimService_PositionUpdated);

                // 2. 初始化全部工艺模组
                bool ok = ProcessModuleManager.InitAll();
                _modulesInitialized = ok;

                // 3. 注入模拟运动服务到所有模组
                ProcessModuleManager.InjectServiceToAll(_simService);

                // 4. 订阅报警
                ProcessModuleManager.SubscribeAlarms(OnModuleAlarm);

                // 5. 为每个模组创建 Tab 并显示运行界面
                LoadModuleTabs();

                // 6. 更新状态栏
                UpdateStatus(ok ? "平台就绪 - 模拟服务已连接" : "平台启动异常 - 部分模组初始化失败");
            }
            catch (Exception ex)
            {
                MessageBox.Show("平台启动异常：" + ex.Message + "\n" + ex.StackTrace,
                    "DOMO 模拟平台", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>遍历所有已注册模组，为每个模组创建 TabPage 并显示运行界面。</summary>
        private void LoadModuleTabs()
        {
            tabModules.TabPages.Clear();

            foreach (KeyValuePair<string, ProcessModuleBase> kv in ProcessModuleManager.Modules)
            {
                ProcessModuleBase module = kv.Value;
                if (module == null || !module.bInitOK) continue;

                // 创建承载面板
                Panel host = new Panel();
                host.Dock = DockStyle.Fill;
                host.BackColor = System.Drawing.Color.White;

                // 创建 TabPage
                TabPage page = new TabPage(module.GetInfo());
                page.Padding = new Padding(0);
                page.Controls.Add(host);
                tabModules.TabPages.Add(page);

                // 调用模组的 ShowRunForm 将运行界面嵌入面板
                module.ShowRunForm(host);
            }

            if (tabModules.TabPages.Count > 0)
                tabModules.SelectedIndex = 0;
        }

        // ============== 报警处理 ==============

        /// <summary>模组报警回调：在报警日志中显示。</summary>
        private void OnModuleAlarm(object sender, ModuleAlarmEventArgs e)
        {
            // 报警事件可能来自非 UI 线程，需要 Invoke
            if (InvokeRequired)
            {
                Invoke(new EventHandler<ModuleAlarmEventArgs>(OnModuleAlarm), sender, e);
                return;
            }

            string line = string.Format("[{0:HH:mm:ss}] [{1}] {2}", e.Time, e.ModuleName, e.Message);
            lstAlarms.Items.Add(line);

            // 保持列表不过长
            while (lstAlarms.Items.Count > 200)
                lstAlarms.Items.RemoveAt(0);

            // 自动滚动到最新
            lstAlarms.TopIndex = lstAlarms.Items.Count - 1;
        }

        // ============== 位置反馈 ==============

        /// <summary>模拟服务位置更新回调：刷新状态栏位置显示。</summary>
        private void SimService_PositionUpdated(object sender, AxisPositionEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<AxisPositionEventArgs>(SimService_PositionUpdated), sender, e);
                return;
            }

            if (e.Position != null && e.Position.Actual != null && e.Position.Actual.Length >= 4)
            {
                lblPosition.Text = string.Format(
                    "X={0:F2}  Y={1:F2}  Z={2:F2}  U={3:F2}",
                    e.Position.Actual[0], e.Position.Actual[1],
                    e.Position.Actual[2], e.Position.Actual[3]);
            }
        }

        // ============== 菜单事件 ==============

        private void MnuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MnuModuleInit_Click(object sender, EventArgs e)
        {
            bool ok = ProcessModuleManager.InitAll();
            _modulesInitialized = ok;
            if (ok)
            {
                ProcessModuleManager.InjectServiceToAll(_simService);
                LoadModuleTabs();
            }
            UpdateStatus(ok ? "模组初始化完成" : "模组初始化失败");
        }

        private void MnuModuleSave_Click(object sender, EventArgs e)
        {
            bool ok = ProcessModuleManager.SaveAll();
            UpdateStatus(ok ? "全部模组参数已保存" : "保存失败");
        }

        private void MnuModuleClose_Click(object sender, EventArgs e)
        {
            ProcessModuleManager.CloseAll();
            _modulesInitialized = false;
            tabModules.TabPages.Clear();
            UpdateStatus("全部模组已关闭");
        }

        private void MnuServiceConnect_Click(object sender, EventArgs e)
        {
            if (_simService == null)
            {
                _simService = new SimulatedMotionService();
                _simService.PositionUpdated += new EventHandler<AxisPositionEventArgs>(SimService_PositionUpdated);
            }
            _simService.Open();
            ProcessModuleManager.InjectServiceToAll(_simService);
            UpdateStatus("模拟服务已连接");
        }

        private void MnuServiceDisconnect_Click(object sender, EventArgs e)
        {
            if (_simService != null)
                _simService.CloseService();
            UpdateStatus("模拟服务已断开");
        }

        // ============== 关闭流程 ==============

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            try
            {
                // 1. 退订报警
                ProcessModuleManager.UnsubscribeAlarms(OnModuleAlarm);

                // 2. 保存参数
                ProcessModuleManager.SaveAll();

                // 3. 关闭所有模组
                ProcessModuleManager.CloseAll();

                // 4. 断开模拟服务
                if (_simService != null)
                {
                    _simService.PositionUpdated -= new EventHandler<AxisPositionEventArgs>(SimService_PositionUpdated);
                    _simService.CloseService();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("关闭异常：" + ex.Message);
            }
        }

        // ============== 工具方法 ==============

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }
    }
}
