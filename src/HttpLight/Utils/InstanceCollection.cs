using System;
using System.Collections.Generic;
using System.Threading;

#if FEATURE_CONCURRENT
using System.Collections.Concurrent;
#endif

namespace HttpLight.Utils
{
    internal class InstanceCollection
    {
        private IDictionary<int, IDictionary<Type, object>> _objects;

        public InstanceCollection()
        {
#if FEATURE_CONCURRENT
            _objects = new ConcurrentDictionary<int, IDictionary<Type, object>>();
#else
            _objects = new Dictionary<int, IDictionary<Type, object>>();
#endif
        }

        public object GetObjectForThread(Type type, object[] ctorArgs = null)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            IDictionary<Type, object> innerDictionary;
#if FEATURE_CONCURRENT
            if (!_objects.TryGetValue(threadId, out innerDictionary))
            {
                innerDictionary = new Dictionary<Type, object>();
                _objects[threadId] = innerDictionary;
            }
#else
            lock (_objects)
            {
                if (!_objects.TryGetValue(threadId, out innerDictionary))
                {
                    innerDictionary = new Dictionary<Type, object>();
                    _objects[threadId] = innerDictionary;
                }
            }
#endif
            object obj;
            if (innerDictionary.TryGetValue(type, out obj))
                return obj;
            obj = ctorArgs == null
                ? Activator.CreateInstance(type)
                : Activator.CreateInstance(type, ctorArgs);
            innerDictionary[type] = obj;
            return obj;
        }
    }
}
