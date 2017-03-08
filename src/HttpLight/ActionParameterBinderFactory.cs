using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HttpLight.Attributes;
using HttpLight.Utils;

namespace HttpLight
{
    internal class ActionParameterBinderFactory
    {
        private IDictionary<Type, IActionParameterBinder> _cache;

        public ActionParameterBinderFactory()
        {
            _cache = new ConcurrentDictionary<Type, IActionParameterBinder>();
        }

        public IActionParameterBinder GetBinder(Type parameterType, Attribute[] parameterAttributes)
        {
            var binderAttribute = GetBinderAttribute(parameterAttributes);
            var binderType = binderAttribute != null
                ? binderAttribute.Binder
                : GetDefaultBinderFor(parameterType);
            if (binderType == null)
                return null;
            return GetBinder(binderType);
        }

        private BinderAttribute GetBinderAttribute(Attribute[] parameterAttributes)
        {
            return parameterAttributes.FirstOrDefault(x => x is BinderAttribute) as BinderAttribute;
        }

        private Type GetDefaultBinderFor(Type parameterType)
        {
            if (SafeStringConvert.IsTypeSupported(parameterType))
                return typeof(PrimitiveBinder);
            return null;
        }

        private IActionParameterBinder GetBinder(Type binderType)
        {
            IActionParameterBinder binder;
            if (_cache.TryGetValue(binderType, out binder))
                return binder;
            binder = (IActionParameterBinder) Activator.CreateInstance(binderType);
            _cache[binderType] = binder;
            return binder;
        }
    }
}
