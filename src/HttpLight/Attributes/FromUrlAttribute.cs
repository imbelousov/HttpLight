using System;

namespace HttpLight.Attributes
{
    /// <summary>
    /// Indicates that values should be extracted from url
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromUrlAttribute : Attribute
    {
        internal string ParameterName { get; }

        public FromUrlAttribute()
            : this(null)
        {
        }

        public FromUrlAttribute(string customName)
        {
            ParameterName = customName;
        }
    }
}
