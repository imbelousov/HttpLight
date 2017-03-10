using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight.Utils
{
    /// <summary>
    /// Calls method much faster than MethodInfo.Invoke
    /// </summary>
    internal class MethodInvoker
    {
        private MethodParameter[] _parameters;
        private Type _returnType;
        private Type _instanceType;
        private Func<object, object[], object> _method;
        private Action<object, object[]> _voidMethod;
        private MethodInfo _methodInfo;
#if FEATURE_ASYNC
        private bool _isAsync;
        private Func<Task, object> _extractResult;
#endif

        public IList<MethodParameter> Parameters
        {
            get { return _parameters; }
        }

#if FEATURE_ASYNC
        public bool IsAsync
        {
            get { return _isAsync; }
        }
#endif

        public Type ReturnType
        {
            get { return _returnType; }
        }

        public Type InstanceType
        {
            get { return _instanceType; }
        }

        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }

        private bool IsVoidMethod
        {
            get { return MethodInfo.ReturnType == typeof(void); }
        }

        public MethodInvoker(MethodInfo methodInfo, Type instanceType)
        {
            _parameters = GetParameters(methodInfo);
            _returnType = methodInfo.ReturnType;
            _instanceType = instanceType;
            _methodInfo = methodInfo;
#if FEATURE_ASYNC
            _isAsync = _returnType == typeof(Task) || _returnType.IsGenericType && _returnType.GetGenericTypeDefinition() == typeof(Task<>);
            if (_isAsync)
            {
                _returnType = _isAsync && _returnType.IsGenericType ? _returnType.GetGenericArguments().Single() : typeof(void);
                if (_returnType != typeof(void))
                    _extractResult = CreateExtractResultMethod(_returnType);
            }
#endif
            if (IsVoidMethod)
                _voidMethod = CreateVoidMethod(methodInfo, instanceType);
            else
                _method = CreateMethod(methodInfo, instanceType);
        }

        public object Invoke(object instance, object[] parameters)
        {
            if (IsVoidMethod)
            {
                _voidMethod(instance, parameters);
                return null;
            }
            else
                return _method(instance, parameters);
        }

#if FEATURE_ASYNC
        public Task<object> InvokeAsync(object instance, object[] parameters)
        {
            if (!_isAsync)
                return Task.FromResult(Invoke(instance, parameters));
            var task = (Task) _method(instance, parameters);
            if (_returnType == typeof(void))
                return task.ContinueWith(x => null as object);
            return task.ContinueWith(x => _extractResult(x));
        }
#endif

        private MethodParameter[] GetParameters(MethodInfo methodInfo)
        {
            return methodInfo
                .GetParameters()
                .Select(x => new MethodParameter
                {
                    Type = x.ParameterType,
                    Name = x.Name,
                    Attributes = x.GetCustomAttributes(true).Cast<Attribute>().ToArray()
                })
                .ToArray();
        }

        private Func<object, object[], object> CreateMethod(MethodInfo methodInfo, Type instanceType)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameters = Expression.Parameter(typeof(object[]), "parameters");
            return Expression.Lambda<Func<object, object[], object>>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(instance, instanceType),
                        methodInfo,
                        _parameters.Select((x, i) => (Expression) Expression.Convert(
                            Expression.ArrayIndex(parameters, Expression.Constant(i)),
                            x.Type
                        ))
                    ), typeof(object)
                ), instance, parameters
            ).Compile();
        }

        private Action<object, object[]> CreateVoidMethod(MethodInfo methodInfo, Type instanceType)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameters = Expression.Parameter(typeof(object[]), "parameters");
            return Expression.Lambda<Action<object, object[]>>(
                Expression.Call(
                    Expression.Convert(instance, instanceType),
                    methodInfo,
                    _parameters.Select((x, i) => (Expression) Expression.Convert(
                        Expression.ArrayIndex(parameters, Expression.Constant(i)),
                        x.Type
                    ))
                ), instance, parameters
            ).Compile();
        }

#if FEATURE_ASYNC
        private Func<Task, object> CreateExtractResultMethod(Type resultType)
        {
            var taskType = typeof(Task<>).MakeGenericType(resultType);
            var resultProperty = taskType.GetProperty(nameof(Task<object>.Result));
            var parameter = Expression.Parameter(typeof(Task));
            return Expression.Lambda<Func<Task, object>>(
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(
                            parameter,
                            taskType
                        ), resultProperty
                    ), typeof(object)
                ), parameter
            ).Compile();
        }
#endif
    }

    internal class MethodParameter
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public Attribute[] Attributes { get; set; }
    }
}
