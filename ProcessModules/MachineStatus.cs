namespace MainModule
{
    /// <summary>
    /// 机台状态（对应 DOMO 模板中 MainModule.MachineStatus）。
    /// 工艺模组在停止/手动切换时复位其中的流程标志。
    /// </summary>
    public static class MachineStatus
    {
        /// <summary>是否处于半自动流程中。</summary>
        public static bool bSemiProcess;
    }
}
