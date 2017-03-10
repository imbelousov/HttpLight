using System;
using System.Collections.Generic;
using System.Linq;
using HttpLight.Attributes;
using HttpLight.Utils;

#if FEATURE_CONCURRENT
using System.Collections.Concurrent;
#endif

namespace HttpLight
{
    internal class ActionParameterBinderFactory
    {
        private IDictionary<MethodParameter, IActionParameterBinder> _cache;

        public ActionParameterBinderFactory()
        {
#if FEATURE_CONCURRENT
            _cache = new ConcurrentDictionary<MethodParameter, IActionParameterBinder>();
#else
            _cache = new Dictionary<MethodParameter, IActionParameterBinder>();
#endif
        }

        public IActionParameterBinder GetBinder(MethodParameter parameter)
        {
            var binder = LoadFromCache(parameter);
            if (binder != null)
                return binder;
            var binderAttribute = GetBinderAttribute(parameter.Attributes);
            var binderType = binderAttribute != null
                ? binderAttribute.Binder
                : GetDefaultBinderFor(parameter.Type);
            if (binderType == null)
                return null;
            binder = CreateBinder(binderType);
            if (binder != null)
                SaveToCache(parameter, binder);
            return binder;
        }

        private IActionParameterBinder LoadFromCache(MethodParameter parameter)
        {
            IActionParameterBinder binder;
#if FEATURE_CONCURRENT
            _cache.TryGetValue(parameter, out binder);
#else
            lock (_cache)
            {
                _cache.TryGetValue(parameter, out binder);
            }
#endif
            return binder;
        }

        private void SaveToCache(MethodParameter parameter, IActionParameterBinder binder)
        {
#if FEATURE_CONCURRENT
            _cache[parameter] = binder;
#else
            lock (_cache)
            {
                _cache[parameter] = binder;
            }
#endif
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

        private IActionParameterBinder CreateBinder(Type binderType)
        {
            var binder = (IActionParameterBinder) Activator.CreateInstance(binderType);
            return binder;
        }
    }
}
