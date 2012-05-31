namespace OmahaBot.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    [Serializable]
    public class BinaryCloneable : ICloneable
    {
        public static byte[] ToByte(object graph)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, graph);
                return ms.ToArray();
            }
        }

        public virtual object Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                return bf.Deserialize(ms);
            }
        }
    }
}
