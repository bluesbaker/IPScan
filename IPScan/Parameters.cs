using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScan
{
    public class Parameters : Dictionary<string, object>, ICloneable
    {
        /// <summary>
        /// Parse args[] with keys to Parameters
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="defaultKey">Default first key</param>
        /// <param name="defaultValue">Default key value</param>
        /// <returns>arguments collection</returns>
        public static Parameters Parse(string[] args, string defaultKey = "", object defaultValue = null)
        {
            var scanParameters = new Parameters();
            string lastKey = defaultKey;

            // filter
            var arguments = from a in args
                            where a != String.Empty
                            select a;

            foreach (string arg in arguments)
            {
                // arg is key
                if (arg[0] == '-')
                {
                    lastKey = arg;
                    scanParameters[lastKey] = defaultValue;
                }
                // or value
                else if (lastKey != String.Empty)
                {
                    scanParameters[lastKey] = arg;
                }
            }

            return scanParameters;
        } 

        public Parameters Copy(params string[][] changes)
        {
            var parametersClone = (Parameters)this.Clone();
            foreach(var field in changes)
            {
                parametersClone[field[0]] = field[1];
            }
            return parametersClone;
        }

        public object Clone()
        {
            var scanParameters = new Parameters();
            foreach (var field in this)
            {
                scanParameters[field.Key] = field.Value;
            }
            return scanParameters;
        }
    }
}
