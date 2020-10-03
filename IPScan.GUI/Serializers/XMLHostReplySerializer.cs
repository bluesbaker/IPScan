using IPScan.BLL;
using IPScan.GUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IPScan.GUI.Serializers
{
    /// <summary>
    /// Additional class for serializing HostReply
    /// </summary>
    public class XMLSerializableHostReply
    {
        public string Address { get; set; }
        public string Status { get; set; }
        public long RoundtripTime { get; set; }
        public List<PortReply> Ports { get; set; }
    }

    public class XMLHostReplySerializer : ICollectionSerializer<HostReply>
    {
        public void Serialize(string filePath, ICollection<HostReply> collection)
        {
            var serializer = new XmlSerializer(typeof(List<XMLSerializableHostReply>));
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                var serializableList = new List<XMLSerializableHostReply>();

                // creating a serializable list from the "HostReply" source data
                foreach (HostReply host in collection)
                {
                    serializableList.Add(new XMLSerializableHostReply
                    {
                        Address = host.Address.ToString(),
                        Status = host.Status.ToString(),
                        RoundtripTime = host.RoundtripTime,
                        Ports = host.Ports.ToList()
                    });
                }

                serializer.Serialize(fs, serializableList);
            }
        }
    }
}
