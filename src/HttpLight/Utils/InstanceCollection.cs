using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace HttpLight.Utils
{
    internal class InstanceCollection
    {
        private IDictionary<int, IDictionary<Type, object>> _objects;

        public InstanceCollection()
        {
            _objects = new ConcurrentDictionary<int, IDictionary<Type, object>>();
        }

        public object GetObjectForThread(Type type, object[] ctorArgs = null)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            IDictionary<Type, object> innerDictionary;
            if (!_objects.TryGetValue(threadId, out innerDictionary))
            {
                innerDictionary = new Dictionary<Type, object>();
                _objects[threadId] = innerDictionary;
            }
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
