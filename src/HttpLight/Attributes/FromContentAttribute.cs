using System;

namespace HttpLight.Attributes
{
    /// <summary>
    /// Indicates that values should be extracted from payload
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromContentAttribute : Attribute
    {
        internal string ParameterName { get; }

        public FromContentAttribute()
            : this(null)
        {
        }

        public FromContentAttribute(string customName)
        {
            ParameterName = customName;
        }
    }
}
