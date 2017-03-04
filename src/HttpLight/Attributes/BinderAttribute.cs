using System;

namespace HttpLight.Attributes
{
    /// <summary>
    /// Determines custom parameter binder
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class BinderAttribute : Attribute
    {
        public Type Binder { get; }

        public BinderAttribute(Type binder)
        {
            if (!typeof(IActionParameterBinder).IsAssignableFrom(binder))
                throw new ArgumentException(binder.Name + " is not assignable to " + nameof(IActionParameterBinder), nameof(binder));
            Binder = binder;
        }
    }
}
