using System;
using System.Net;
using System.Threading;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight
{
    /// <summary>
    /// Hosts an HTTP server with <see cref="HttpListener"/>
    /// </summary>
    public class HttpServer : IDisposable
    {
        private static readonly TimeSpan DefaultAcceptTimeout = new TimeSpan(0, 0, 1);

        private HttpListener _listener;
        private Thread _acceptThread;
        private TimeSpan _acceptTimeout;
        private bool _isDisposed;
        private ControllerCollection _controllers;
        private HostCollection _hosts;
        private RequestStateMachine _requestStateMachine;

        public bool IsStarted
        {
            get { return !_isDisposed && _listener != null && _listener.IsListening; }
        }

        /// <summary>
        /// Determines how long <see cref="HttpServer"/> waits for new HTTP requests
        /// </summary>
        public TimeSpan AcceptTimeout
        {
            get { return _acceptTimeout; }
            set
            {
                if (value <= TimeSpan.Zero)
                    throw new Exception("Timeout must be greater than 0");
                _acceptTimeout = value;
            }
        }

        public ControllerCollection Controllers
        {
            get { return _controllers; }
        }

        public HostCollection Hosts
        {
            get { return _hosts; }
        }

        public HttpServer()
        {
            AcceptTimeout = DefaultAcceptTimeout;
            _requestStateMachine = new RequestStateMachine();
            _controllers = new ControllerCollection(_requestStateMachine.Actions);
            _hosts = new HostCollection();
            _controllers.Add<DefaultController>();
        }

        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(HttpServer));
            if (IsStarted)
                return;
            _listener = new HttpListener();
            foreach (var hostEntry in _hosts)
                _listener.Prefixes.Add(hostEntry.ToString());
            _listener.Start();
            _acceptThread = new Thread(AcceptThread)
            {
                Name = "HttpServer accept thread"
            };
            _acceptThread.Start();
        }

        public void Stop()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(HttpServer));
            if (!IsStarted)
                return;
            ((IDisposable) _listener).Dispose();
            _listener = null;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            Stop();
            _isDisposed = true;
        }

        private void AcceptThread()
        {
            while (IsStarted)
            {
                try
                {
                    var task = _listener.BeginGetContext(ar =>
                    {
                        HttpListenerContext context;
                        try
                        {
                            context = _listener.EndGetContext(ar);
                        }
                        catch
                        {
                            return;
                        }
                        var stateMachineContext = new RequestStateMachineContext(
                            new HttpRequest(context.Request),
                            new HttpResponse(context.Response),
                            context.Response.OutputStream
                        );
#if FEATURE_ASYNC
                        Task.Run(() => _requestStateMachine.StartAsync(stateMachineContext));
#else
                        ThreadPool.QueueUserWorkItem(x => _requestStateMachine.Start(stateMachineContext));
#endif
                    }, _listener);
                    task.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, 1));
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
