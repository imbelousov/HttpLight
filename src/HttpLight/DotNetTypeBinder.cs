using System;
using System.Linq;
using HttpLight.Utils;

namespace HttpLight
{
    internal class DotNetTypeBinder : IActionBinder
    {
        public object Bind(Type parameterType, string parameterName, HttpRequest request)
        {
            var values = request.UrlParameters.GetValues(parameterName);
            if (parameterType.IsArray)
                return SafeStringConvert.ChangeType(values, parameterType.GetElementType());
            else
                return SafeStringConvert.ChangeType(values != null ? values.FirstOrDefault() : null, parameterType);
        }
    }
}
