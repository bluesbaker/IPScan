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
    public class ScannerArgumentAttribute : Attribute
    {
        /// <summary>
        /// Declarate an option argument
        /// </summary>
        /// <param name="argument">For example '-a'</param>
        /// <param name="description">Argument description</param>
        /// <param name="defaultValue">Default value</param>
        public ScannerArgumentAttribute(string argument, string description, object defaultValue, bool isRequired = false)
        {
            Argument = argument;
            Description = description;
            DefaultValue = defaultValue;
            IsRequired = isRequired;
        }

        public string Argument { get; private set; }
        public string Description { get; private set; }
        public object DefaultValue { get; private set; }
        public bool IsRequired { get; private set; }
    }
}
