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
            if (!typeof(IActionBinder).IsAssignableFrom(binder))
                throw new ArgumentException(binder.Name + " is not assignable to " + nameof(IActionBinder), nameof(binder));
            Binder = binder;
        }
    }
}
