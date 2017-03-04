namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for POST requests
    /// </summary>
    public class PostAttribute : ActionAttribute
    {
        internal override void Apply(Action action)
        {
            action.Methods.Add(HttpMethod.Post);
        }
    }
}
