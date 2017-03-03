using System.Linq;
using HttpLight.Utils;

namespace HttpLight
{
    internal class DotNetTypeBinder : IActionBinder
    {
        public object Bind(ActionBinderContext actionBinderContext)
        {
            var values = actionBinderContext.HttpRequest.UrlParameters.GetValues(actionBinderContext.ParameterName);
            if (actionBinderContext.ParameterType.IsArray)
                return SafeStringConvert.ChangeType(values, actionBinderContext.ParameterType.GetElementType());
            else
                return SafeStringConvert.ChangeType(values != null ? values.FirstOrDefault() : null, actionBinderContext.ParameterType);
        }
    }
}
