namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for GET requests
    /// </summary>
    public class GetAttribute : ActionAttribute
    {
        internal override void Apply(Action action)
        {
            action.Methods.Add(HttpMethod.Get);
        }
    }
}
