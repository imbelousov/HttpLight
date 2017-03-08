using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight
{
    public interface IHttpRequest
    {
        /// <summary>
        /// Gets the MIME types accepted by the client
        /// </summary>
        string[] AcceptTypes { get; }

        /// <summary>
        /// Temporary storage
        /// </summary>
        IDictionary<string, object> Bag { get; }

        /// <summary>
        /// Gets an error code that identifies a problem with the <see cref="System.Security.Cryptography.X509Certificates.X509Certificate"/>
        /// provided by the client
        /// </summary>
        int ClientCertificateError { get; }

        /// <summary>
        /// Payload that sent by client
        /// </summary>
        IHttpRequestContent Content { get; }

        /// <summary>
        /// Gets the content encoding that can be used with data sent with the request
        /// </summary>
        Encoding ContentEncoding { get; }

        /// <summary>
        /// Gets the length of the body data included in the request
        /// </summary>
        long? ContentLength { get; }

        /// <summary>
        /// Gets the MIME type of the payload included in the request
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the cookies sent with the request
        /// </summary>
        CookieCollection Cookies { get; }

        /// <summary>
        /// Gets a boolean value that indicates whether the request has associated payload
        /// </summary>
        bool HasContent { get; }

        /// <summary>
        /// Gets the collection of header name/value pairs sent in the request
        /// </summary>
        NameValueCollection Headers { get; }

        /// <summary>
        /// Gets a boolean value that indicates whether the client sending this request 
        /// is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a boolean value that indicates whether the request is sent from the
        /// local computer
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// Gets a boolean value that indicates whether the TCP connection used to
        /// send the request is using the Secure Sockets Layer (SSL) protocol
        /// </summary>
        bool IsSecureConnection { get; }

        /// <summary>
        /// Gets a boolean value that indicates whether the TCP connection was a WebSocket request
        /// </summary>
        bool IsWebSocketRequest { get; }

        /// <summary>
        /// Gets a boolean value that indicates whether the client requests a persistent connection
        /// </summary>
        bool KeepAlive { get; }

        /// <summary>
        /// Get the server IP address and port number to which the request is directed
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// HTTP method
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the HTTP version used by the requesting client
        /// </summary>
        Version ProtocolVersion { get; }

        /// <summary>
        /// Gets the client IP address and port number from which the request originated
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets the request identifier of the incoming HTTP request
        /// </summary>
        Guid RequestTraceIdentifier { get; }

        /// <summary>
        /// Gets the Service Provider Name (SPN) that the client sent on the request
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.TransportContext"/> for the client request
        /// </summary>
        TransportContext TransportContext { get; }

        /// <summary>
        /// Requested URL
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Parameters passed through URL
        /// </summary>
        NameValueCollection UrlParameters { get; }

        /// <summary>
        /// Gets the URL of the resource that referred the client to the server
        /// </summary>
        Uri UrlReferrer { get; }

        /// <summary>
        /// Gets the user agent presented by the client
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// Gets the natural languages that are preferred for the response
        /// </summary>
        string[] UserLanguages { get; }

        /// <summary>
        /// Retrieves the client's X.509 v.3 certificate
        /// </summary>
        X509Certificate2 GetClientCertificate();

#if FEATURE_ASYNC
        /// <summary>
        /// Retrieves the client's X.509 v.3 certificate
        /// </summary>
        Task<X509Certificate2> GetClientCertificateAsync();
#endif
    }

    public interface IHttpRequestContent
    {
        /// <summary>
        /// Gets a stream that contains the payload sent by client
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// Parameters passed through payload. Note that if you call getter, <see cref="Stream"/>
        /// will be read to end, then parsed and results will be cached.
        /// </summary>
        NameValueCollection ContentParameters { get; }

        /// <summary>
        /// Raw content passed through payload. Note that if you call getter, <see cref="Stream"/>
        /// will be read to end and results will be cached.
        /// </summary>
        string RawContent { get; }
    }
}
