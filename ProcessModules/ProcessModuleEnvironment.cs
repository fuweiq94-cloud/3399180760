using System;
using System.IO;

namespace InterfaceDefine
{
    /// <summary>
    /// 工艺模组运行环境：提供全局参数路径与当前项目路径，
    /// 并在项目切换时通知各模组重新加载项目参数。
    /// 对应 DOMO 模板中的 InterfaceDefine.AppParam（全局参数路径）
    /// 与 MainModule.ProjectManager（项目路径 + openProject 事件）。
    /// </summary>
    public static class ProcessModuleEnvironment
    {
        private static string _currentProjectPath;

        /// <summary>打开项目事件：参数为项目路径，返回值为各订阅方的处理结果。</summary>
        public delegate bool OpenProjectHandler(string strProjectPath);

        /// <summary>切换/打开项目时触发（对应 ProjectManager.openProject）。</summary>
        public static event OpenProjectHandler OpenProject;

        /// <summary>
        /// 全局参数根路径（对应 AppParam.AppParamPath()）。
        /// 位于 程序目录\AppParam 下，模组全局参数存放在其 ProcessModule\模组名 子目录。
        /// </summary>
        public static string AppParamPath()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppParam");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>
        /// 当前项目路径（对应 ProjectManager.projectSetting.strProjectPath）。
        /// 未显式设置时默认使用 程序目录\Projects\Default。
        /// </summary>
        public static string CurrentProjectPath
        {
            get
            {
                if (string.IsNullOrEmpty(_currentProjectPath))
                    _currentProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects", "Default");
                if (!Directory.Exists(_currentProjectPath))
                    Directory.CreateDirectory(_currentProjectPath);
                return _currentProjectPath;
            }
            set { _currentProjectPath = value; }
        }

        /// <summary>
        /// 切换当前项目并广播 openProject 事件，
        /// 各工艺模组收到后重新加载自己的项目参数。
        /// </summary>
        public static bool SwitchProject(string strProjectPath)
        {
            CurrentProjectPath = strProjectPath;
            bool ret = true;
            OpenProjectHandler h = OpenProject;
            if (h != null)
            {
                foreach (OpenProjectHandler handler in h.GetInvocationList())
                {
                    ret = handler(strProjectPath) && ret;
                }
            }
            return ret;
        }
    }
}
