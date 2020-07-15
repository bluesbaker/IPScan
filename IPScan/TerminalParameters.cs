using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScan
{
    public class TerminalParameters : Dictionary<string, string>, ICloneable
    {
        /// <summary>
        /// Parse args[] with keys to Parameters
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="defaultKey">Default first key</param>
        /// <param name="defaultValue">Default key value</param>
        /// <returns>arguments collection</returns>
        public static TerminalParameters Parse(string[] args, string defaultKey = "", string defaultValue = "true")
        {
            var parameters = new TerminalParameters();
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
                    parameters[lastKey] = defaultValue;
                }
                // or value
                else if (lastKey != String.Empty)
                {
                    parameters[lastKey] = arg;
                }
            }

            return parameters;
        } 

        public TerminalParameters Copy(IDictionary<string, string> changes = null)
        {
            var parametersClone = (TerminalParameters)this.Clone();

            if(changes != null)
            {
                foreach (var field in changes)
                {
                    parametersClone[field.Key] = field.Value;
                }
            }           

            return parametersClone;
        }

        public object Clone()
        {
            var parameters = new TerminalParameters();
            foreach (var field in this)
            {
                parameters[field.Key] = field.Value;
            }
            return parameters;
        }
    }
}
