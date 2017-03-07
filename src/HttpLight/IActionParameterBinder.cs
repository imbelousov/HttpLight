using System;

namespace HttpLight
{
    /// <summary>
    /// Pulls data from given source and converts it to action parameter
    /// </summary>
    public interface IActionParameterBinder
    {
        object Bind(ActionParameterBinderContext context);
    }

    public class ActionParameterBinderContext
    {
        public Type ParameterType { get; internal set; }
        public string ParameterName { get; internal set; }
        public Attribute[] ParameterAttributes { get; internal set; }
        public IActionParameterSource Source { get; internal set; }
    }
}
