using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScan.Supports
{
    /// <summary>
    /// Declaration an option key with an description
    /// and the default value
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DeclarateKeyAttribute : Attribute
    {
        /// <summary>
        /// Declarate an option key
        /// </summary>
        /// <param name="key">For example '-k'</param>
        /// <param name="description">Key description</param>
        /// <param name="defaultValue">Default key value</param>
        public DeclarateKeyAttribute(string key, string description, string defaultValue)
        {
            Key = key;
            Description = description;
            DefaultValue = defaultValue;
        }

        public string Key { get; private set; }
        public string Description { get; private set; }
        public string DefaultValue { get; private set; }
    }
}
