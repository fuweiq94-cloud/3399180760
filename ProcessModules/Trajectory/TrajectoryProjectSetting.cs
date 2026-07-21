using System.IO;
using InterfaceDefine;

namespace ProcessModules.Trajectory
{
    /// <summary>
    /// 轨迹查看工艺模组项目参数。
    /// 项目参数存放在 当前项目路径\模组名 目录，随项目切换而更换。
    /// </summary>
    public class TrajectoryProjectSetting
    {
        public string Name;

        /// <summary>本项目中累计清除轨迹的次数。</summary>
        public int ClearTrailCount;

        /// <summary>上次保存时记录的轨迹点数（仅供参考）。</summary>
        public int LastTrailPointCount;

        public TrajectoryProjectSetting()
        {
        }

        /// <summary>
        /// 加载参数 —— 项目参数路径。
        /// </summary>
        /// <param name="actionerName">工艺模组名称。</param>
        public static TrajectoryProjectSetting Load(string actionerName)
        {
            TrajectoryProjectSetting pDoc = null;
            string sDirectory = Path.Combine(
                ProcessModuleEnvironment.CurrentProjectPath, actionerName, "TrajectoryProjectSetting.xml");
            try
            {
                pDoc = XmlSerializationHelper.ReadFromFile<TrajectoryProjectSetting>(sDirectory);
            }
            catch
            {
                pDoc = new TrajectoryProjectSetting();
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
            sDirectory = Path.Combine(sDirectory, "TrajectoryProjectSetting.xml");
            XmlSerializationHelper.SaveToFile(sDirectory, this);
            return true;
        }
    }
}
