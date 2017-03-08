using System.Linq;
using HttpLight.Utils;

namespace HttpLight
{
    internal class PrimitiveBinder : IActionParameterBinder
    {
        public object Bind(ActionParameterBinderContext context)
        {
            var values = context.Source.GetValues(context.ParameterName);
            return SafeStringConvert.IsCollection(context.ParameterType)
                ? SafeStringConvert.ChangeType(values, SafeStringConvert.GetElementType(context.ParameterType))
                : SafeStringConvert.ChangeType(values != null ? values.FirstOrDefault() : null, context.ParameterType);
        }
    }
}
