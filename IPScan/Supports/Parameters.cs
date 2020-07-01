using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScan.Supports
{
    public class Parameters : Dictionary<string, string>
    {
        /// <summary>
        /// Parse args[] with keys to Parameters
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="defaultKey">Default first key</param>
        /// <param name="defaultValue">Default key value</param>
        /// <returns>arguments collection</returns>
        public static Parameters Parse(
            string[] args, string defaultKey = "", string defaultValue = "true")
        {
            var argCollection = new Parameters();
            string lastKey = defaultKey;

            foreach (string arg in args)
            {
                // arg is key
                if (arg[0] == '-')
                {
                    lastKey = arg;
                    argCollection[lastKey] = defaultValue;  // set default value
                }
                // or value
                else if (lastKey != String.Empty)
                {
                    argCollection[lastKey] = arg;
                }
            }

            return argCollection;
        }
    }
}
