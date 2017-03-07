using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HttpLight.Utils;

namespace HttpLight.Test.Utils
{
    internal class ActionBuilder : ICloneable
    {
        private const string TempAssemblyName = "HttpLight.Test.TempAssembly";
        private const string TempModuleName = "TempModule";
        private const string DefaultControllerName = "MainController";
        private const string FieldName = "Value";

        private string _controllerName;
        private string _name;
        private ICollection<AttributeInfo> _attributes;
        private object _returnValue;
        private Type _returnType;
        private IList<Tuple<Type, string, ICollection<AttributeInfo>>> _parameters;
        private Type _exceptionType;
        private string _exceptionMessage;

        private ActionBuilder()
        {
        }

        public ActionBuilder(string name)
        {
            _controllerName = DefaultControllerName;
            _name = name;
            _attributes = new List<AttributeInfo>();
            _returnValue = null;
            _returnType = typeof(void);
            _parameters = new List<Tuple<Type, string, ICollection<AttributeInfo>>>();
            _exceptionType = null;
        }

        public ActionBuilder SetControllerName(string name)
        {
            var copy = CreateCopy();
            copy._controllerName = name;
            return copy;
        }

        public ActionBuilder AddAttribute(Type type)
        {
            return AddAttribute(new AttributeInfo(type));
        }

        public ActionBuilder AddAttribute(Type type, object ctorArg)
        {
            return AddAttribute(new AttributeInfo(type, new[] {ctorArg}));
        }

        public ActionBuilder AddAttribute(Type type, object[] ctorArgs)
        {
            return AddAttribute(new AttributeInfo(type, ctorArgs));
        }

        public ActionBuilder AddAttribute(AttributeInfo attributeInfo)
        {
            var copy = CreateCopy();
            copy._attributes.Add(attributeInfo);
            return copy;
        }

        public ActionBuilder SetReturnType(Type type)
        {
            var copy = CreateCopy();
            copy._returnType = type;
            return copy;
        }

        public ActionBuilder SetReturnValue(object value)
        {
            var copy = CreateCopy();
            copy._returnValue = value;
            return copy;
        }

        public ActionBuilder AddParameter(string name, Type type)
        {
            return AddParameter(name, type, Enumerable.Empty<AttributeInfo>());
        }

        public ActionBuilder AddParameter(string name, Type type, Type attributeType)
        {
            return AddParameter(name, type, new[] {new AttributeInfo(attributeType)});
        }

        public ActionBuilder AddParameter(string name, Type type, Type attributeType, object attributeCtorArg)
        {
            return AddParameter(name, type, new[] {new AttributeInfo(attributeType, new[] {attributeCtorArg})});
        }

        public ActionBuilder AddParameter(string name, Type type, Type attributeType, object[] attributeCtorArgs)
        {
            return AddParameter(name, type, new[] {new AttributeInfo(attributeType, attributeCtorArgs)});
        }

        public ActionBuilder AddParameter(string name, Type type, IEnumerable<AttributeInfo> attributes)
        {
            var copy = CreateCopy();
            copy._parameters.Add(new Tuple<Type, string, ICollection<AttributeInfo>>(type, name, attributes.ToList()));
            return copy;
        }

        public ActionBuilder Throws(Type exceptionType)
        {
            return Throws(exceptionType, null);
        }

        public ActionBuilder Throws(Type exceptionType, string exceptionMessage)
        {
            var copy = CreateCopy();
            copy._exceptionType = exceptionType;
            copy._exceptionMessage = exceptionMessage;
            return copy;
        }

        public Action Build()
        {
            var typeBuilder = BuildType();
            var methodBuilder = BuildMethod(typeBuilder);
            BuildMethodBody(typeBuilder, methodBuilder);
            var type = typeBuilder.CreateType();
            var field = type.GetField(FieldName, BindingFlags.Public | BindingFlags.Static);
            if (field != null)
                field.SetValue(null, _returnValue);
            var methodInfo = type.GetMethod(_name, BindingFlags.Instance | BindingFlags.Public);
            var invoker = new MethodInvoker(methodInfo, type);
            return new Action(invoker);
        }

        public object Clone()
        {
            return CreateCopy();
        }

        private ActionBuilder CreateCopy()
        {
            var copy = new ActionBuilder();
            copy._controllerName = _controllerName;
            copy._name = _name;
            copy._attributes = _attributes.ToList();
            copy._returnValue = _returnValue;
            copy._returnType = _returnType;
            copy._parameters = _parameters.ToList();
            copy._exceptionType = _exceptionType;
            copy._exceptionMessage = _exceptionMessage;
            return copy;
        }

        private TypeBuilder BuildType()
        {
            var assemblyName = new AssemblyName(TempAssemblyName);
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(TempModuleName);
            var typeBuilder = moduleBuilder.DefineType(_controllerName, TypeAttributes.Public | TypeAttributes.Class, typeof(Controller));
            return typeBuilder;
        }

        private MethodBuilder BuildMethod(TypeBuilder typeBuilder)
        {
            var methodBuilder = typeBuilder.DefineMethod(_name, MethodAttributes.Public, _returnType, _parameters.Select(x => x.Item1).ToArray());
            for (var i = 0; i < _parameters.Count; i++)
            {
                var parameterBuilder = methodBuilder.DefineParameter(i + 1, ParameterAttributes.In, _parameters[i].Item2);
                foreach (var attribute in _parameters[i].Item3)
                {
                    var ctor = attribute.Type.GetConstructor(attribute.CtorArgs.Select(x => x.GetType()).ToArray());
                    parameterBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, attribute.CtorArgs));
                }
            }
            foreach (var attribute in _attributes)
            {
                var ctor = attribute.Type.GetConstructor(attribute.CtorArgs.Select(x => x.GetType()).ToArray());
                methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, attribute.CtorArgs));
            }
            return methodBuilder;
        }

        private void BuildMethodBody(TypeBuilder typeBuilder, MethodBuilder methodBuilder)
        {
            var ilGenerator = methodBuilder.GetILGenerator();
            if (_exceptionType != null)
            {
                ConstructorInfo ctor;
                if (!string.IsNullOrEmpty(_exceptionMessage))
                {
                    ctor = _exceptionType.GetConstructor(new[] {typeof(string)});
                    ilGenerator.Emit(OpCodes.Ldstr, _exceptionMessage);
                }
                else
                    ctor = _exceptionType.GetConstructor(new Type[0]);
                ilGenerator.Emit(OpCodes.Newobj, ctor);
                ilGenerator.Emit(OpCodes.Throw);
            }
            if (_returnType != typeof(void))
            {
                var fieldBuilder = typeBuilder.DefineField(FieldName, _returnType, FieldAttributes.Public | FieldAttributes.Static);
                ilGenerator.Emit(OpCodes.Ldsfld, fieldBuilder);
            }
            ilGenerator.Emit(OpCodes.Ret);
        }
    }

    internal class AttributeInfo
    {
        public Type Type { get; }
        public object[] CtorArgs { get; }

        public AttributeInfo(Type type)
            : this(type, null)
        {
        }

        public AttributeInfo(Type type, object[] ctorArgs)
        {
            Type = type;
            CtorArgs = ctorArgs ?? new object[0];
        }
    }
}
