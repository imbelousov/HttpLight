namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for PUT requests
    /// </summary>
    public class PutAttribute : ActionAttribute
    {
        internal override void Apply(Action action)
        {
            action.Methods.Add(HttpMethod.Put);
        }
    }
}
