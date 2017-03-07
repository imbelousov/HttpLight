namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for HEAD requests
    /// </summary>
    public class HeadAttribute : ActionAttribute
    {
        internal override void Apply(Action action)
        {
            action.Methods.Add(HttpMethod.Head);
        }
    }
}
