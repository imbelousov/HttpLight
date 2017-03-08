using System.Linq;

namespace HttpLight.Attributes
{
    /// <summary>
    /// Specifies HTTP methods which triggers the action
    /// </summary>
    public class MethodAttribute : ActionAttribute
    {
        internal string[] Methods { get; }

        public MethodAttribute(string method)
            : this(new[] {method})
        {
        }

        public MethodAttribute(params string[] methods)
        {
            Methods = methods
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToUpper())
                .ToArray();
        }

        internal override void Apply(Action action)
        {
            foreach (var method in Methods)
                action.Methods.Add(method);
        }
    }
}
