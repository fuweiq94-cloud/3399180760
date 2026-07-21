using System.IO;
using InterfaceDefine;

namespace ProcessModules.MainControl
{
    /// <summary>
    /// 主控制工艺模组项目参数（对应 DOMO 模板 MAINMODUO.CS 中的 DemoProjectSetting）。
    /// 项目参数存放在 当前项目路径\模组名 目录，随项目切换而更换。
    /// </summary>
    public class MainControlProjectSetting
    {
        public string Name;

        /// <summary>上次目标位置（保存时从 Hub 取回）。</summary>
        public float LastTargetX;
        public float LastTargetY;
        public float LastTargetZ;

        /// <summary>本项目中该模组累计执行 GOTO 命令的次数。</summary>
        public int GotoCount;

        public MainControlProjectSetting()
        {
        }

        /// <summary>
        /// 加载参数 —— 项目参数路径。
        /// </summary>
        /// <param name="actionerName">工艺模组名称。</param>
        public static MainControlProjectSetting Load(string actionerName)
        {
            MainControlProjectSetting pDoc = null;
            string sDirectory = Path.Combine(
                ProcessModuleEnvironment.CurrentProjectPath, actionerName, "MainControlProjectSetting.xml");
            try
            {
                pDoc = XmlSerializationHelper.ReadFromFile<MainControlProjectSetting>(sDirectory);
            }
            catch
            {
                pDoc = new MainControlProjectSetting();
            }
            pDoc.Name = actionerName;
            return pDoc;
        }

        /// <summary>
        /// 保存参数 —— 项目参数路径。
        /// </summary>
        public bool Save()
        {
            string sDirectory = Path.Combine(ProcessModuleEnvironment.CurrentProjectPath, Name);
            if (!Directory.Exists(sDirectory))
            {
                Directory.CreateDirectory(sDirectory);
            }
            sDirectory = Path.Combine(sDirectory, "MainControlProjectSetting.xml");
            XmlSerializationHelper.SaveToFile(sDirectory, this);
            return true;
        }
    }
}
