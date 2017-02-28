using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttpLight.Attributes;
using HttpLight.Utils;

namespace HttpLight
{
    public sealed class ModuleCollection : IEnumerable<Type>
    {
        private RouteCollection _routes;
        private Type[] _actionAttributes;
        private ICollection<Type> _modules;

        internal ModuleCollection(RouteCollection routes)
        {
            _routes = routes;
            _actionAttributes = GetActionAttributes().ToArray();
            _modules = new List<Type>();
        }

        public void Add(Type moduleType)
        {
            if (!typeof(HttpModule).IsAssignableFrom(moduleType))
                throw new Exception(moduleType.Name + " is not assignable to " + typeof(HttpModule).Name);
            var actions = GetActions(moduleType);
            foreach (var actionInfo in actions)
            {
                var actionInvoker = null as MethodInvoker;
                foreach (var httpMethod in actionInfo.HttpMethods)
                {
                    foreach (var path in actionInfo.Paths)
                    {
                        actionInvoker = actionInvoker ?? new MethodInvoker(actionInfo.MethodInfo, moduleType);
                        _routes.Add(httpMethod, path, new Route(actionInvoker, actionInfo));
                    }
                }
                foreach (var httpStatusCode in actionInfo.HttpStatusCodes)
                {
                    actionInvoker = actionInvoker ?? new MethodInvoker(actionInfo.MethodInfo, moduleType);
                    _routes.Add(httpStatusCode, new Route(actionInvoker, actionInfo));
                }
            }
            _modules.Add(moduleType);
        }

        public void Add<T>()
            where T : HttpModule
        {
            Add(typeof(T));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _modules.GetEnumerator();
        }

        private IEnumerable<ActionInfo> GetActions(Type moduleType)
        {
            var methods = moduleType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var methodInfo in methods)
            {
                var info = new ActionInfo(methodInfo);
                ApplyActionAttributes(info);
                yield return info;
            }
        }

        private IEnumerable<Type> GetActionAttributes()
        {
            return GetType()
                .Assembly
                .GetTypes()
                .Where(x => typeof(ActionAttribute).IsAssignableFrom(x));
        }

        private void ApplyActionAttributes(ActionInfo actionInfo)
        {
            foreach (var actionAttributeType in _actionAttributes)
            {
                foreach (ActionAttribute actionAttribute in actionInfo.MethodInfo.GetCustomAttributes(actionAttributeType, true))
                {
                    actionAttribute.Apply(actionInfo);
                }
            }
        }
    }
}
