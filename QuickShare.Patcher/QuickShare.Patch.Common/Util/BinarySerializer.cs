using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace QuickShare.Patch.Common.Util
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    public static class BinarySerializer
    {
        public static T Deserialize<T>(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                IFormatter serializer = new BinaryFormatter();
                var obj = serializer.Deserialize(stream);
                return (T)obj;
            }
        }

        public static bool Serialize<T>(T data, string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
                return true;
            }
        }

        public static Stream Serialize<T>(T data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
                return stream;
            }
        }
    }
}