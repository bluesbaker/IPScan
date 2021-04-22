using System;
using System.Collections.Generic;
using System.Linq;

namespace IPScan
{
    /// <summary>
    /// Parameters of TerminalStream
    /// </summary>
    public class TerminalParameters : Dictionary<string, string>, ICloneable
    {
        /// <summary>
        /// Parse args[] with keys to Parameters
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="defaultKey">Default first key</param>
        /// <param name="defaultValue">Default key value</param>
        /// <returns>Arguments collection</returns>
        public static TerminalParameters Parse(string[] args, string defaultKey = "", string defaultValue = "true")
        {
            var parameters = new TerminalParameters();
            // filter
            var arguments = from a in args
                            where a != String.Empty
                            select a;
            var lastKey = defaultKey;

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

        /// <summary>
        /// Returns the typed clone with\without replaced dictionary of params
        /// </summary>
        /// <param name="dictionary">Parameters dictionary</param>
        /// <returns>Terminal parameters</returns>
        public TerminalParameters Copy(IDictionary<string, string> dictionary = null)
        {
            var parametersClone = (TerminalParameters)this.Clone();

            if (dictionary != null)
            {
                foreach (var field in dictionary)
                    parametersClone[field.Key] = field.Value;
            }

            return parametersClone;
        }

        // implementation ICloneable
        public object Clone()
        {
            var parameters = new TerminalParameters();
            foreach (var field in this)
                parameters[field.Key] = field.Value;
            return parameters;
        }
    }
}
