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
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        /// <summary>
        /// Declarate an option key
        /// </summary>
        /// <param name="key">For example '-k'</param>
        /// <param name="description">Key description</param>
        /// <param name="defaultValue">Default value</param>
        public KeyAttribute(string key, string description, object defaultValue, bool isRequired = false)
        {
            Key = key;
            Description = description;
            DefaultValue = defaultValue;
            IsRequired = isRequired;
        }

        public string Key { get; private set; }
        public string Description { get; private set; }
        public object DefaultValue { get; private set; }
        public bool IsRequired { get; private set; }
    }
}
