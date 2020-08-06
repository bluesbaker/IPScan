using System;

namespace IPScan.SUP
{
    /// <summary>
    /// Declaration an option key with an description
    /// and the default value
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class KeySetterAttribute : Attribute
    {
        /// <summary>
        /// Declarate an option key
        /// </summary>
        /// <param name="key">For example '-k'</param>
        /// <param name="description">Key description</param>
        public KeySetterAttribute(string key, string description)
        {
            Key = key;
            Description = description;
        }

        public string Key { get; private set; }
        public string Description { get; private set; }
    }
}
