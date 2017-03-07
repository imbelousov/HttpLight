using System;
using System.Linq;
using HttpLight.Attributes;
using HttpLight.Utils;

namespace HttpLight
{
    internal class ActionParameterSourceFactory
    {
        public IActionParameterSource CreateSource(IHttpRequest request, MethodParameter parameter)
        {
            var fromContentAttribute = GetAttribute<FromContentAttribute>(parameter);
            if (fromContentAttribute != null)
            {
                UpdateParameterName(parameter, fromContentAttribute.ParameterName);
                return new ContentActionParameterSource(request);
            }

            var fromUrlAttribute = GetAttribute<FromUrlAttribute>(parameter);
            if (fromUrlAttribute != null)
            {
                UpdateParameterName(parameter, fromUrlAttribute.ParameterName);
                return new UrlActionParameterSource(request);
            }

            return new UrlActionParameterSource(request);
        }

        private void UpdateParameterName(MethodParameter parameter, string name)
        {
            if (!string.IsNullOrEmpty(name))
                parameter.Name = name;
        }

        private T GetAttribute<T>(MethodParameter parameter)
            where T : Attribute
        {
            return parameter.Attributes.FirstOrDefault(x => x is T) as T;
        }
    }
}
