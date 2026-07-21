using System;
using System.Windows.Forms;
using InterfaceDefine;
using MainModule;

namespace ProcessModules.MainControl
{
    /// <summary>
    /// 主控制工艺模组（对应 DOMO.CS 中的 ProcessModuleDemo）。
    /// 继承 ProcessModuleBase，封装 XYZ 三轴手动控制业务：
    /// 持有 XyzControllerHub（业务层）与三个 AxisJogService（JOG 服务），
    /// 运行界面为 RunForm（DOMO 模式：new RunForm(this)，与模组共享同一份业务状态）。
    /// </summary>
    /// <remarks>
    /// 支持的平台命令（Action）：
    ///   GOTO x y z     → 三轴移动到指定目标
    ///   ORIGIN         → 回原点
    ///   CENTER         → 回中心
    ///   JOG axis dir   → 寸动一步（axis = X/Y/Z，dir = +1/-1）
    ///   SETSPEED value → 设置速度档位 [0,100]
    ///   STOP / ESTOP   → 停止所有运动
    /// </remarks>
    public class MainControlProcessModule : ProcessModuleBase
    {
        internal RunForm runForm;
        public ModuleSettingForm settingForm;

        public MainControlGlobalSetting globalSetting;
        public MainControlProjectSetting projectSetting;

        // —— 业务核心：与运行界面共享同一个 Hub ——
        private XyzControllerHub _hub;
        private AxisJogService[] _jogServices;

        /// <summary>模组持有的 XYZ 控制器（集成现有业务层）。</summary>
        public XyzControllerHub Hub { get { return _hub; } }

        public override dynamic FunctionCaller
        {
            get { return this; }
        }

        public override string GetInfo()
        {
            return "主控制工艺模组";
        }

        public override bool Init(string strName)
        {
            try
            {
                processModuleName = strName;
                bInitOK = false;
                LoadSetting();

                // 创建业务层（轴范围与速度取自全局参数）
                // null = 后端服务尚未接入（待引入真实 DLL 后通过 SetService 注入）
                _hub = new XyzControllerHub(null,
                    globalSetting.XMin, globalSetting.XMax,
                    globalSetting.YMin, globalSetting.YMax,
                    globalSetting.ZMin, globalSetting.ZMax,
                    globalSetting.UMin, globalSetting.UMax);
                _hub.SpeedSetting = globalSetting.SpeedSetting;

                // JOG 服务：每个轴一个（对应 MainForm 中的 _jogServices）
                _jogServices = new AxisJogService[]
                {
                    new AxisJogService(_hub.X),
                    new AxisJogService(_hub.Y),
                    new AxisJogService(_hub.Z)
                };
                ApplyJogSetting();

                // 注册平台事件：打开项目时重新加载项目参数
                ProcessModuleEnvironment.OpenProject += new ProcessModuleEnvironment.OpenProjectHandler(OnOpenProject);

                bInitOK = true;
                SetModuleVariable("主控制初始化完成", "true");
                return bInitOK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(strName + "工艺模组初始化异常" + ex.Message + ex.StackTrace);
                return false;
            }
        }

        #region 注册平台事件

        /// <summary>
        /// 打开项目时重新加载项目文件，初始化工艺模组。
        /// </summary>
        private bool OnOpenProject(string strProjectPath)
        {
            LoadSetting();
            return bInitOK;
        }

        #endregion

        /// <summary>把 JOG 全局参数应用到 JOG 服务。</summary>
        private void ApplyJogSetting()
        {
            JogMode mode = globalSetting.JogIncremental ? JogMode.Incremental : JogMode.Continuous;
            foreach (AxisJogService s in _jogServices)
            {
                s.SetMode(mode);
                s.SetStepDistance(globalSetting.JogStep);
            }
        }

        /// <summary>重新加载工艺模组。</summary>
        public override bool ReOpen()
        {
            LoadSetting();
            if (_hub != null)
                _hub.SpeedSetting = globalSetting.SpeedSetting;
            if (_jogServices != null)
                ApplyJogSetting();
            return bInitOK;
        }

        public override bool Close()
        {
            ProcessModuleEnvironment.OpenProject -= new ProcessModuleEnvironment.OpenProjectHandler(OnOpenProject);
            return true;
        }

        public override bool LoadSetting()
        {
            try
            {
                globalSetting = MainControlGlobalSetting.Load(processModuleName);
                projectSetting = MainControlProjectSetting.Load(processModuleName);

                taskItemSetting = globalSetting.taskItemSetting;
                GetModuleVariable("主控制首次运行", DataType.布尔, "false");
                GetModuleVariable("主控制初始化完成", DataType.布尔, "false");
                GetModuleVariable("主平台请求移动", DataType.布尔, "false");
                GetModuleVariable("移动完成", DataType.布尔, "false");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(processModuleName + "加载setting发生异常" + ex.Message + ex.StackTrace);
                globalSetting = MainControlGlobalSetting.Load(processModuleName);
                return false;
            }
        }

        /// <summary>保存工艺模组参数。</summary>
        public override bool Save()
        {
            try
            {
                // 把当前业务状态回写到参数对象
                if (_hub != null && projectSetting != null)
                {
                    projectSetting.LastTargetX = _hub.X.Target;
                    projectSetting.LastTargetY = _hub.Y.Target;
                    projectSetting.LastTargetZ = _hub.Z.Target;
                }
                if (globalSetting != null && _hub != null)
                    globalSetting.SpeedSetting = _hub.SpeedSetting;

                globalSetting.Save();
                projectSetting.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存参数时发生异常," + ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>显示工艺模组运行界面（DOMO 模式：RunForm 持有模组引用）。</summary>
        public override bool ShowRunForm(Panel panel)
        {
            foreach (Control controlItem in panel.Controls)
            {
                controlItem.Visible = false;
            }
            panel.Controls.Clear();
            if (runForm == null || runForm.IsDisposed)
                runForm = new RunForm(this);

            runForm.TopLevel = false;
            panel.Controls.Add(runForm);
            runForm.Dock = DockStyle.Fill;
            runForm.Show();
            return true;
        }

        /// <summary>显示工艺模组设置界面。</summary>
        public override bool ShowSettingForm(Panel panel)
        {
            foreach (Control controlItem in panel.Controls)
            {
                controlItem.Visible = false;
            }
            panel.Controls.Clear();
            if (settingForm == null || settingForm.IsDisposed)
                settingForm = new ModuleSettingForm(this, globalSetting, projectSetting);

            settingForm.TopLevel = false;
            panel.Controls.Add(settingForm);
            settingForm.Dock = DockStyle.Fill;
            settingForm.Show();
            return true;
        }

        /// <summary>执行工艺模组相关动作。</summary>
        public override int Action(params object[] param)
        {
            if (param == null || param.Length == 0)
            {
                InsertAlarm("空命令");
                return -1;
            }

            string swName = param[0].ToString().ToUpper();
            int ret = 0;
            switch (swName)
            {
                case "GOTO":
                    // GOTO x y z
                    if (param.Length < 4)
                    {
                        InsertAlarm("GOTO 命令需要 3 个坐标参数");
                        ret = -2;
                        break;
                    }
                    float x = Convert.ToSingle(param[1]);
                    float y = Convert.ToSingle(param[2]);
                    float z = Convert.ToSingle(param[3]);
                    SetModuleVariable("主平台请求移动", "true");
                    _hub.SetTarget(x, y, z);
                    projectSetting.GotoCount++;
                    SetModuleVariable("移动完成", "true");
                    break;
                case "ORIGIN":
                    _hub.ResetToOrigin();
                    break;
                case "CENTER":
                    _hub.SetToCenter();
                    break;
                case "JOG":
                    // JOG axis direction，例如 JOG X +1
                    if (param.Length < 3)
                    {
                        InsertAlarm("JOG 命令需要轴名和方向参数");
                        ret = -2;
                        break;
                    }
                    ret = JogAxis(param[1].ToString().ToUpper(), Convert.ToInt32(param[2]));
                    break;
                case "SETSPEED":
                    if (param.Length < 2)
                    {
                        InsertAlarm("SETSPEED 命令需要速度参数");
                        ret = -2;
                        break;
                    }
                    _hub.SpeedSetting = Math.Max(0, Math.Min(100, Convert.ToInt32(param[1])));
                    break;
                case "STOP":
                case "ESTOP":
                    StopAll();
                    break;
                default:
                    InsertAlarm("未知命令:" + param[0].ToString());
                    ret = -1;
                    break;
            }
            return ret;
        }

        /// <summary>对指定轴执行一次寸动。</summary>
        private int JogAxis(string axisName, int direction)
        {
            if (direction != 1 && direction != -1)
            {
                InsertAlarm("JOG 方向必须是 +1 或 -1");
                return -2;
            }
            int index;
            switch (axisName)
            {
                case "X": index = 0; break;
                case "Y": index = 1; break;
                case "Z": index = 2; break;
                default:
                    InsertAlarm("未知轴名:" + axisName);
                    return -2;
            }
            // 寸动：按一下走一步；连续模式的停止由 STOP 命令完成
            _jogServices[index].SetMode(JogMode.Incremental);
            _jogServices[index].OnJogStart(direction);
            _jogServices[index].OnJogStop();
            _jogServices[index].SetMode(globalSetting.JogIncremental ? JogMode.Incremental : JogMode.Continuous);
            return 0;
        }

        /// <summary>根据需要设置模组参数，比如 CTQ 参数等。</summary>
        public override bool SetParam(object sKey, object sValue)
        {
            string key = sKey.ToString();
            switch (key)
            {
                case "SpeedSetting":
                    _hub.SpeedSetting = Math.Max(0, Math.Min(100, Convert.ToInt32(sValue)));
                    return true;
                case "JogStep":
                    globalSetting.JogStep = Convert.ToSingle(sValue);
                    ApplyJogSetting();
                    return true;
                case "JogIncremental":
                    globalSetting.JogIncremental = Convert.ToBoolean(sValue);
                    ApplyJogSetting();
                    return true;
                default:
                    SetModuleVariable(key, sValue == null ? "" : sValue.ToString());
                    return true;
            }
        }

        public override object GetParam(object itemName)
        {
            string key = itemName.ToString();
            switch (key)
            {
                case "SpeedSetting": return _hub.SpeedSetting;
                case "CurrentX": return _hub.X.Current;
                case "CurrentY": return _hub.Y.Current;
                case "CurrentZ": return _hub.Z.Current;
                case "TargetX": return _hub.X.Target;
                case "TargetY": return _hub.Y.Target;
                case "TargetZ": return _hub.Z.Target;
                default: return GetStringVariable(key);
            }
        }

        public override bool StopAll()
        {
            foreach (AxisJogService s in _jogServices)
                s.EmergencyStop();
            MachineStatus.bSemiProcess = false;
            SetModuleVariable("主平台请求移动", "false");
            return true;
        }

        public override bool ReleaseAlarm()
        {
            bAlarm = false;
            return true;
        }
    }
}
