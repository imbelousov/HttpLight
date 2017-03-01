using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HttpLight.Attributes;
using HttpLight.Utils;

namespace HttpLight
{
    internal class ActionBinderFactory
    {
        private IDictionary<Type, IActionBinder> _cache;

        public ActionBinderFactory()
        {
            _cache = new ConcurrentDictionary<Type, IActionBinder>();
        }

        public IActionBinder GetBinder(Type parameterType, Attribute[] parameterAttributes)
        {
            var binderAttribute = parameterAttributes.FirstOrDefault(x => x is BinderAttribute) as BinderAttribute;
            Type binderType;
            if (binderAttribute != null)
                binderType = binderAttribute.Binder;
            else if (SafeStringConvert.IsTypeSupported(parameterType))
                binderType = typeof(DotNetTypeBinder);
            else
                binderType = null;
            if (binderType == null)
                return null;
            IActionBinder binder;
            if (_cache.TryGetValue(binderType, out binder))
                return binder;
            binder = (IActionBinder) Activator.CreateInstance(binderType);
            _cache[binderType] = binder;
            return binder;
        }
    }
}
