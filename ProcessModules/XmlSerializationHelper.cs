using System.IO;
using System.Xml.Serialization;

namespace InterfaceDefine
{
    /// <summary>
    /// XML 序列化助手（对应 DOMO 模板中 InterfaceDefine.CommKit.XMLSerializationHelper）。
    /// 负责工艺模组参数（全局参数 / 项目参数）的读写。
    /// </summary>
    public static class XmlSerializationHelper
    {
        /// <summary>从 XML 文件读取对象；文件不存在时返回默认实例。</summary>
        public static T ReadFromFile<T>(string path) where T : new()
        {
            if (!File.Exists(path))
                return new T();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (T)serializer.Deserialize(fs);
            }
        }

        /// <summary>把对象保存到 XML 文件（目录不存在时自动创建）。</summary>
        public static void SaveToFile<T>(string path, T obj)
        {
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, obj);
            }
        }
    }
}
