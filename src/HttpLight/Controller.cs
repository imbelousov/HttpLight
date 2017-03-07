namespace HttpLight
{
    /// <summary>
    /// Provides modular application architecture, contains user actions
    /// </summary>
    public abstract class Controller
    {
        /// <summary>
        /// An HTTP request info
        /// </summary>
        protected IHttpRequest Request { get; private set; }

        /// <summary>
        /// An HTTP response info
        /// </summary>
        protected IHttpResponse Response { get; private set; }

        internal void Initialize(IHttpRequest request, IHttpResponse response)
        {
            Request = request;
            Response = response;
        }

        internal void Release()
        {
            Request = null;
            Response = null;
        }
    }
}
