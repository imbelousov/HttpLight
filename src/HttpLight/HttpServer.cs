﻿using System;
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

        private HttpListener _httpListener;
        private Thread _acceptThread;
        private TimeSpan _acceptTimeout;
        private bool _isDisposed;
        private RequestHandler _requestHandler;
        private ModuleCollection _modules;

        public bool IsStarted
        {
            get { return !_isDisposed && _httpListener != null && _httpListener.IsListening; }
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

        public ModuleCollection Modules
        {
            get { return _modules; }
        }

        public HttpServer()
        {
            AcceptTimeout = DefaultAcceptTimeout;
            _requestHandler = new RequestHandler();
            _modules = new ModuleCollection(_requestHandler.Routes);
        }

        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(HttpServer));
            if (IsStarted)
                return;
            _httpListener = new HttpListener();
            _httpListener.Start();
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
            ((IDisposable) _httpListener).Dispose();
            _httpListener = null;
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
                var task = _httpListener.BeginGetContext(ar =>
                {
                    HttpListenerContext context;
                    try
                    {
                        context = _httpListener.EndGetContext(ar);
                    }
                    catch
                    {
                        return;
                    }
#if FEATURE_ASYNC
                    Task.Run(() => _requestHandler.HandleRequestAsync(context));
#else
                    ThreadPool.QueueUserWorkItem(x => _requestHandler.HandleRequest(context));
#endif
                }, _httpListener);
                task.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, 1));
            }
        }
    }
}