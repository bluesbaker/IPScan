using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScan
{
    public class IPInfo : Dictionary<string, string>
    {
        public void Merge(IPInfo ipInfo, bool isReplaced = true)
        {
            foreach(var field in ipInfo)
            {
                if (isReplaced)
                {
                    this[field.Key] = field.Value;
                }
                else
                {
                    var currentField = this.FirstOrDefault(f => f.Key == field.Key);
                    this[field.Key] = currentField.Value ?? field.Value;
                }
            }            
        }
    }
}
