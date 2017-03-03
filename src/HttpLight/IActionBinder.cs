using System;

namespace HttpLight
{
    /// <summary>
    /// Pulls data from given context and converts it to action parameter
    /// </summary>
    public interface IActionBinder
    {
        object Bind(ActionBinderContext actionBinderContext);
    }

    public class ActionBinderContext
    {
        public Type ParameterType { get; internal set; }
        public string ParameterName { get; internal set; }
        public Attribute[] ParameterAttributes { get; internal set; }
        public IHttpRequest HttpRequest { get; internal set; }
    }
}
