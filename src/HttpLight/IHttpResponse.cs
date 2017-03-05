using System;
using System.Net;
using System.Text;

namespace HttpLight
{
    public interface IHttpResponse
    {
        /// <summary>
        /// Gets or sets the content encoding
        /// </summary>
        Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Gets or sets the length of content
        /// </summary>
        long? ContentLength { get; set; }

        /// <summary>
        /// Get or sets the MIME type of content
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the collection of cookies
        /// </summary>
        CookieCollection Cookies { get; set; }

        /// <summary>
        /// Gets or sets the exception that occured during action invocation
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the collection of header name/value pairs
        /// </summary>
        WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the server requests a persistent connection
        /// </summary>
        bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets the HTTP version
        /// </summary>
        Version ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets whether the response uses chunked transfer encoding
        /// </summary>
        bool SendChunked { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a text description of the HTTP status code
        /// </summary>
        string StatusDescription { get; set; }
    }
}
