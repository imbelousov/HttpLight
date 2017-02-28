using System;

namespace HttpLight.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ActionAttribute : Attribute
    {
        internal abstract void Apply(ActionInfo actionInfo);
    }
}
