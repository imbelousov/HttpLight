namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for POST requests
    /// </summary>
    public class PostAttribute : ActionAttribute
    {
        internal override void Apply(ActionInfo actionInfo)
        {
            if (!actionInfo.HttpMethods.Contains(HttpMethod.Post))
                actionInfo.HttpMethods.Add(HttpMethod.Post);
        }
    }
}
