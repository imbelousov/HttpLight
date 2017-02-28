namespace HttpLight
{
    /// <summary>
    /// Provides modular application architecture, contains user actions
    /// </summary>
    public abstract class HttpModule
    {
        private HttpRequest _request;
        private HttpResponse _response;

        internal HttpRequest InternalRequest
        {
            get { return _request; }
            set { _request = value; }
        }

        internal HttpResponse InternalResponse
        {
            get { return _response; }
            set { _response = value; }
        }

        /// <summary>
        /// HTTP request info
        /// </summary>
        protected HttpRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        /// HTTP response info
        /// </summary>
        protected HttpResponse Response
        {
            get { return _response; }
        }
    }
}
