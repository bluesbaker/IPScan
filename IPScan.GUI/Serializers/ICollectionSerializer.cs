using System.Collections.Generic;

namespace IPScan.GUI.Serializers
{
    public interface ICollectionSerializer<T>
    {
        void Serialize(string filePath, ICollection<T> collection);
    }
}
