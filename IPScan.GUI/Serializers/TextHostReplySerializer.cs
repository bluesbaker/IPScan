using IPScan.BLL;
using IPScan.GUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace IPScan.GUI.Serializers
{
    public class TextHostReplySerializer : ICollectionSerializer<HostReply>
    {      
        public void Serialize(string filePath, ICollection<HostReply> collection)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                foreach (HostReply host in collection)
                {
                    string hostLines = $"{host.Address}\t{host.Status}\t{host.RoundtripTime}";

                    foreach (PortReply port in host.Ports)
                    {
                        hostLines += $"{host.Address}:{port.Port}\t{port.Status}\n";
                    }

                    writer.WriteLine(hostLines);
                }
            }
        }
    }
}
