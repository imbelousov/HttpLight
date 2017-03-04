namespace HttpLight
{
    /// <summary>
    /// Provides modular application architecture, contains user actions
    /// </summary>
    public abstract class HttpModule
    {
        private IHttpRequest _request;
        private IHttpResponse _response;

        internal IHttpRequest InternalRequest
        {
            get { return _request; }
            set { _request = value; }
        }

        internal IHttpResponse InternalResponse
        {
            get { return _response; }
            set { _response = value; }
        }

        /// <summary>
        /// HTTP request info
        /// </summary>
        protected IHttpRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        /// HTTP response info
        /// </summary>
        protected IHttpResponse Response
        {
            get { return _response; }
        }
    }
}
