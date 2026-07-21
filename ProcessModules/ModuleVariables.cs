using System.Collections.Generic;
using System.Xml.Serialization;

namespace MainModule
{
    /// <summary>
    /// 模组变量数据类型。对应 DOMO 模板中 MainModule.DataType。
    /// </summary>
    public enum DataType
    {
        布尔,
        数值,
        字符串
    }

    /// <summary>
    /// 模组变量：供流程编排（任务）读写。
    /// 对应 DOMO 模板中 MainModule.TaskVariable。
    /// </summary>
    public class TaskVariable
    {
        public string strName = "";
        public string strValue = "";
        public DataType DataType = DataType.字符串;

        public TaskVariable()
        {
        }
    }

    /// <summary>
    /// 任务项设置：持有模组向平台注册的全部变量。
    /// 对应 DOMO 模板中 MainModule.TaskItemSetting。
    /// 列表用于 XML 持久化；字典为运行时索引，不序列化。
    /// </summary>
    public class TaskItemSetting
    {
        /// <summary>变量列表（XML 序列化用）。</summary>
        public List<TaskVariable> listTaskVariables = new List<TaskVariable>();

        /// <summary>变量字典（运行时索引，不序列化）。</summary>
        [XmlIgnore]
        public Dictionary<string, TaskVariable> dicTaskVariables = new Dictionary<string, TaskVariable>();

        public TaskItemSetting()
        {
        }

        /// <summary>按列表重建运行时字典索引。</summary>
        public void RebuildDictionary()
        {
            dicTaskVariables = new Dictionary<string, TaskVariable>();
            foreach (TaskVariable v in listTaskVariables)
            {
                if (!string.IsNullOrEmpty(v.strName) && !dicTaskVariables.ContainsKey(v.strName))
                    dicTaskVariables.Add(v.strName, v);
            }
        }

        /// <summary>添加新变量（已存在同名变量时不重复添加）。</summary>
        public void AddNewVariable(TaskVariable taskVar)
        {
            if (taskVar == null || string.IsNullOrEmpty(taskVar.strName)) return;
            RebuildDictionary();
            if (dicTaskVariables.ContainsKey(taskVar.strName)) return;
            listTaskVariables.Add(taskVar);
            dicTaskVariables.Add(taskVar.strName, taskVar);
        }
    }
}
