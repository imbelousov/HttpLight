using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttpLight.Attributes;
using HttpLight.Utils;

namespace HttpLight
{
    public sealed class ControllerCollection : IEnumerable<Type>
    {
        private ActionCollection _actions;
        private ICollection<Type> _controllerTypes;

        internal ControllerCollection(ActionCollection actions)
        {
            _actions = actions;
            _controllerTypes = new List<Type>();
        }

        public void Add(Type controllerType)
        {
            if (!typeof(Controller).IsAssignableFrom(controllerType))
                throw new Exception(controllerType.Name + " is not assignable to " + typeof(Controller).Name);
            var actions = GetActions(controllerType);
            foreach (var action in actions)
            {
                foreach (var httpMethod in action.Methods)
                {
                    if (!action.Paths.Any())
                        action.Paths.Add(action.Invoker.MethodInfo.Name);
                    foreach (var path in action.Paths)
                        _actions.Add(httpMethod, path, action);
                }
                foreach (var httpStatusCode in action.StatusCodes)
                    _actions.Add(httpStatusCode, action);
                if (action.Before)
                    _actions.AddBefore(action);
            }
            _controllerTypes.Add(controllerType);
        }

        public void Add<T>()
            where T : Controller
        {
            Add(typeof(T));
        }

        public void Clear()
        {
            _actions.Clear();
            _controllerTypes.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _controllerTypes.GetEnumerator();
        }

        private IEnumerable<Action> GetActions(Type controllerType)
        {
            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var methodInfo in methods)
            {
                var info = new Action(new MethodInvoker(methodInfo, controllerType));
                ApplyAttributes(info);
                yield return info;
            }
        }

        private void ApplyAttributes(Action action)
        {
            var actionAttributes = action.Invoker.MethodInfo.GetCustomAttributes().Select(x => x as ActionAttribute).Where(x => x != null);
            foreach (var actionAttribute in actionAttributes)
            {
                actionAttribute.Apply(action);
            }
        }
    }
}
