﻿namespace HttpLight.Attributes
{
    /// <summary>
    /// Marks action as custom HTTP error page
    /// </summary>
    public class StatusCodeAttribute : ActionAttribute
    {
        internal HttpStatusCode StatusCode { get; }

        public StatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        internal override void Apply(Action action)
        {
            action.StatusCodes.Add(StatusCode);
        }
    }
}
