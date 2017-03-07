namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for DELETE requests
    /// </summary>
    public class DeleteAttribute : ActionAttribute
    {
        internal override void Apply(Action action)
        {
            action.Methods.Add(HttpMethod.Put);
        }
    }
}
