using System;
using System.Windows.Forms;
using MainModule;

namespace InterfaceDefine
{
    /// <summary>
    /// 工艺模组基类（对应 DOMO 模板中 InterfaceDefine.ProcessModuleBase）。
    /// 所有工艺模组（主控制 / 点位跳转 / 轨迹查看）都继承自本类，
    /// 统一实现：初始化 Init、运行界面 ShowRunForm、设置界面 ShowSettingForm、
    /// 参数加载 LoadSetting / 保存 Save、命令执行 Action、停止 StopAll、报警释放 ReleaseAlarm。
    /// </summary>
    public abstract class ProcessModuleBase
    {
        // —— ProcessModuleBase 默认变量（与 DOMO 模板保持一致）——
        /// <summary>模组当前是否处于报警状态。</summary>
        public bool bAlarm;

        /// <summary>模组是否初始化完成。</summary>
        public bool bInitOK;

        /// <summary>工艺模组名称（Init 时传入）。</summary>
        public string processModuleName;

        /// <summary>模组任务项设置（流程变量集合），LoadSetting 时从全局参数中取出。</summary>
        public TaskItemSetting taskItemSetting;

        /// <summary>模组内部线程安全锁。</summary>
        protected object lockObj = new object();

        /// <summary>模组产生报警时触发（对应 DOMO 模板中 FrmManagement.frmAlarm）。</summary>
        public event EventHandler<ModuleAlarmEventArgs> AlarmOccurred;

        /// <summary>
        /// 供平台脚本/流程动态调用的对象，默认返回模组自身。
        /// </summary>
        public abstract dynamic FunctionCaller { get; }

        /// <summary>模组描述信息。</summary>
        public abstract string GetInfo();

        /// <summary>初始化工艺模组。</summary>
        /// <param name="strName">模组名称（同时作为参数目录名）。</param>
        public abstract bool Init(string strName);

        /// <summary>重新加载工艺模组（重新加载配置并重新初始化）。</summary>
        public abstract bool ReOpen();

        /// <summary>关闭工艺模组，释放资源。</summary>
        public abstract bool Close();

        /// <summary>从参数路径加载模组参数（全局参数 + 项目参数）。</summary>
        public abstract bool LoadSetting();

        /// <summary>保存模组参数（全局参数 + 项目参数）。</summary>
        public abstract bool Save();

        /// <summary>在指定面板中显示模组运行界面。</summary>
        public abstract bool ShowRunForm(Panel panel);

        /// <summary>在指定面板中显示模组设置界面。</summary>
        public abstract bool ShowSettingForm(Panel panel);

        /// <summary>执行工艺模组相关动作命令。</summary>
        /// <param name="param">param[0] 为命令名（不区分大小写），后续为命令参数。</param>
        /// <returns>0 = 成功；-1 = 未知命令；其他 = 命令自定义错误码。</returns>
        public abstract int Action(params object[] param);

        /// <summary>根据需要设置模组参数（比如 CTQ 参数等）。</summary>
        public abstract bool SetParam(object sKey, object sValue);

        /// <summary>读取模组参数。</summary>
        public abstract object GetParam(object itemName);

        /// <summary>停止模组所有运动/流程。</summary>
        public abstract bool StopAll();

        /// <summary>释放（复位）模组报警。</summary>
        public abstract bool ReleaseAlarm();

        // ============== 供子类使用的辅助方法 ==============

        /// <summary>插入一条模组报警（对应 DOMO 模板中的 InsertAlarm）。</summary>
        protected void InsertAlarm(string message)
        {
            bAlarm = true;
            EventHandler<ModuleAlarmEventArgs> h = AlarmOccurred;
            if (h != null)
                h(this, new ModuleAlarmEventArgs(processModuleName, message));
        }

        /// <summary>
        /// 为模组注册变量（对应 DOMO 模板中的 GetModuleVariable）。
        /// 变量不存在时按给定类型和初值创建，存在时直接返回当前值。
        /// </summary>
        protected string GetModuleVariable(string varName, DataType dataType, string varValue = "")
        {
            if (taskItemSetting == null)
                taskItemSetting = new TaskItemSetting();

            taskItemSetting.RebuildDictionary();
            if (!taskItemSetting.dicTaskVariables.ContainsKey(varName))
            {
                TaskVariable taskVar = new TaskVariable();
                taskVar.strName = varName;
                taskVar.strValue = varValue;
                taskVar.DataType = dataType;
                taskItemSetting.AddNewVariable(taskVar);
            }
            return GetStringVariable(varName);
        }

        /// <summary>读取模组字符串变量，不存在时返回空字符串。</summary>
        protected string GetStringVariable(string varName)
        {
            if (taskItemSetting == null) return "";
            taskItemSetting.RebuildDictionary();
            TaskVariable v;
            if (taskItemSetting.dicTaskVariables.TryGetValue(varName, out v))
                return v.strValue;
            return "";
        }

        /// <summary>写入模组变量，不存在时按字符串类型创建。</summary>
        protected void SetModuleVariable(string varName, string varValue)
        {
            GetModuleVariable(varName, DataType.字符串, varValue);
            taskItemSetting.dicTaskVariables[varName].strValue = varValue;
        }
    }
}
