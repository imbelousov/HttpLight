using System;

namespace HttpLight
{
    /// <summary>
    /// Pulls data from given context and converts it to action parameter
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
        public IHttpRequest HttpRequest { get; internal set; }
    }
}
