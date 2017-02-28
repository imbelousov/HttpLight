namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks action as custom HTTP error page
    /// </summary>
    public class StatusCodeAttribute : ActionAttribute
    {
        public HttpStatusCode StatusCode { get; }

        public StatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        internal override void Apply(ActionInfo actionInfo)
        {
            actionInfo.HttpStatusCodes.Add(StatusCode);
        }
    }
}
