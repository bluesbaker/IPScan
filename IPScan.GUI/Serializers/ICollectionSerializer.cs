using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Documents.Serialization;

namespace IPScan.GUI.Serializers
{
    public interface ICollectionSerializer<T>
    {
        void Serialize(string filePath, ICollection<T> collection);
    }
}
