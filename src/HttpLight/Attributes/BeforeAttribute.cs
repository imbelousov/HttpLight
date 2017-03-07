namespace HttpLight.Attributes
{
    /// <summary>
    /// Invokes action before any request, then appropriate usual action will be
    /// invoked. If action marked with <see cref="BeforeAttribute"/> returns something
    /// but null, this data will be sent to the client and usual action will be ignored.
    /// </summary>
    public class BeforeAttribute : ActionAttribute
    {
        internal override void Apply(Action action)
        {
            action.Before = true;
        }
    }
}
