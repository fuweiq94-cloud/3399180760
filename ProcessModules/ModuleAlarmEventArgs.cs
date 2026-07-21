using System;

namespace MainModule
{
    /// <summary>
    /// 工艺模组报警事件参数。
    /// 对应 DOMO 模板中 FrmManagement.frmAlarm.AlarmOccurred 的报警项，
    /// 在本项目中以事件参数形式自包含实现。
    /// </summary>
    public class ModuleAlarmEventArgs : EventArgs
    {
        /// <summary>产生报警的模组名称。</summary>
        public string ModuleName { get; private set; }

        /// <summary>报警内容。</summary>
        public string Message { get; private set; }

        /// <summary>报警发生时间。</summary>
        public DateTime Time { get; private set; }

        public ModuleAlarmEventArgs(string moduleName, string message)
        {
            ModuleName = moduleName;
            Message = message;
            Time = DateTime.Now;
        }
    }
}
