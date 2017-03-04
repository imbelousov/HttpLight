using System.Linq;
using HttpLight.Utils;

namespace HttpLight
{
    internal class PrimitivesBinder : IActionParameterBinder
    {
        public object Bind(ActionParameterBinderContext context)
        {
            var values = context.HttpRequest.UrlParameters.GetValues(context.ParameterName);
            if (context.ParameterType.IsArray)
                return SafeStringConvert.ChangeType(values, context.ParameterType.GetElementType());
            else
                return SafeStringConvert.ChangeType(values != null ? values.FirstOrDefault() : null, context.ParameterType);
        }
    }
}
