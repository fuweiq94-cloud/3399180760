using System.Collections.Generic;
using InterfaceDefine;
using ProcessModules.MainControl;
using ProcessModules.PointJump;
using ProcessModules.Trajectory;

namespace ProcessModules
{
    /// <summary>
    /// 工艺模组管理器：统一注册、初始化、停止、保存所有工艺模组。
    /// 对应 DOMO 模板中主平台对工艺模组的集中管理。
    /// </summary>
    public static class ProcessModuleManager
    {
        private static readonly Dictionary<string, ProcessModuleBase> _modules =
            new Dictionary<string, ProcessModuleBase>(System.StringComparer.OrdinalIgnoreCase);

        private static bool _initialized;

        /// <summary>所有已注册的工艺模组（只读视图）。</summary>
        public static IEnumerable<KeyValuePair<string, ProcessModuleBase>> Modules
        {
            get { return _modules; }
        }

        /// <summary>按名称获取模组，不存在时返回 null。</summary>
        public static ProcessModuleBase Get(string name)
        {
            ProcessModuleBase m;
            if (_modules.TryGetValue(name, out m))
                return m;
            return null;
        }

        /// <summary>
        /// 初始化全部内置工艺模组（主控制 / 点位跳转 / 轨迹查看）。
        /// 幂等：重复调用直接返回上次结果。
        /// </summary>
        public static bool InitAll()
        {
            if (_initialized)
                return true;

            bool ok = true;
            ok = RegisterAndInit(new MainControlProcessModule(), "MainControl") && ok;
            ok = RegisterAndInit(new PointJumpProcessModule(), "PointJump") && ok;
            ok = RegisterAndInit(new TrajectoryViewProcessModule(), "TrajectoryView") && ok;
            _initialized = ok;
            return ok;
        }

        /// <summary>注册并初始化一个工艺模组。</summary>
        public static bool RegisterAndInit(ProcessModuleBase module, string name)
        {
            _modules[name] = module;
            return module.Init(name);
        }

        /// <summary>停止所有模组的运动 / 流程。</summary>
        public static bool StopAll()
        {
            bool ok = true;
            foreach (KeyValuePair<string, ProcessModuleBase> kv in _modules)
                ok = kv.Value.StopAll() && ok;
            return ok;
        }

        /// <summary>保存所有模组参数。</summary>
        public static bool SaveAll()
        {
            bool ok = true;
            foreach (KeyValuePair<string, ProcessModuleBase> kv in _modules)
                ok = kv.Value.Save() && ok;
            return ok;
        }

        /// <summary>关闭所有模组。</summary>
        public static bool CloseAll()
        {
            bool ok = true;
            foreach (KeyValuePair<string, ProcessModuleBase> kv in _modules)
                ok = kv.Value.Close() && ok;
            _initialized = false;
            return ok;
        }
    }
}
