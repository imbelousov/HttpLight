namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks method to be used for GET requests
    /// </summary>
    public class GetAttribute : ActionAttribute
    {
        internal override void Apply(ActionInfo actionInfo)
        {
            actionInfo.HttpMethods.Add(HttpMethod.Get);
        }
    }
}
