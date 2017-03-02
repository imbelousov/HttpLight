using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using HttpLight.Utils;

namespace HttpLight
{
    internal class RequestStateMachine : FiniteStateMachine<RequestState, HttpContext>
    {
        private RouteCollection _routes;
        private InstanceCollection _moduleInstances;
        private ActionBinderFactory _binderFactory;

        public RouteCollection Routes
        {
            get { return _routes; }
        }

        protected override RequestState FirstState
        {
            get { return RequestState.Begin; }
        }

        public RequestStateMachine()
        {
            _routes = new RouteCollection();
            _moduleInstances = new InstanceCollection();
            _binderFactory = new ActionBinderFactory();
            AddState(RequestState.Begin, Begin);
            AddState(RequestState.SelectAction, SelectAction);
            AddState(RequestState.InvokeAction, InvokeAction);
            AddState(RequestState.SelectStatusCodeAction, SelectStatusCodeAction);
            AddState(RequestState.InvokeStatusCodeAction, InvokeStatusCodeAction);
            AddState(RequestState.SendResponse, SendResponse);
#if FEATURE_ASYNC
            AddAsyncState(RequestState.InvokeAction, InvokeActionAsync);
            AddAsyncState(RequestState.InvokeStatusCodeAction, InvokeStatusCodeActionAsync);
            AddAsyncState(RequestState.SendResponse, SendResponseAsync);
#endif
        }

        /// <summary>
        /// Initial state
        /// </summary>
        private RequestState Begin(HttpContext context)
        {
            return RequestState.SelectAction;
        }

        /// <summary>
        /// Attempt to select an appropriate action from collection
        /// </summary>
        private RequestState SelectAction(HttpContext context)
        {
            bool methodNotAllowed;
            var route = _routes.Get(context.HttpRequest.HttpMethod, context.HttpRequest.Url.LocalPath, out methodNotAllowed);
            if (route == null)
            {
                context.HttpResponse.StatusCode = methodNotAllowed
                    ? HttpStatusCode.MethodNotAllowed
                    : HttpStatusCode.NotFound;
                return RequestState.SelectStatusCodeAction;
            }
            context.Route = route;
            return RequestState.InvokeAction;
        }

        /// <summary>
        /// Attempt to invoke selected action
        /// </summary>
        private RequestState InvokeAction(HttpContext context)
        {
            context.HttpResponse.StatusCode = HttpStatusCode.Ok;
            return InvokeAction(context, RequestState.SelectStatusCodeAction);
        }

#if FEATURE_ASYNC
        /// <summary>
        /// Attempt to invoke selected action
        /// </summary>
        private Task<RequestState> InvokeActionAsync(HttpContext context)
        {
            context.HttpResponse.StatusCode = HttpStatusCode.Ok;
            return InvokeActionAsync(context, RequestState.SelectStatusCodeAction);
        }
#endif

        /// <summary>
        /// Attempt to select an appropriate status code action. Usually this is custom 404 or 500 error page.
        /// </summary>
        private RequestState SelectStatusCodeAction(HttpContext context)
        {
            var route = _routes.Get(context.HttpResponse.StatusCode);
            if (route == null)
                return RequestState.SendResponse;
            context.Route = route;
            return RequestState.InvokeStatusCodeAction;
        }

        /// <summary>
        /// Attempt to invoke selected status code action
        /// </summary>
        private RequestState InvokeStatusCodeAction(HttpContext context)
        {
            return InvokeAction(context, RequestState.SendResponse);
        }

#if FEATURE_ASYNC
        /// <summary>
        /// Attempt to invoke selected status code action
        /// </summary>
        private Task<RequestState> InvokeStatusCodeActionAsync(HttpContext context)
        {
            return InvokeActionAsync(context, RequestState.SendResponse);
        }
#endif

        /// <summary>
        /// Sending previous results
        /// </summary>
        private RequestState SendResponse(HttpContext context)
        {
            var stream = context.Result ?? new MemoryStream(new byte[0]);
            try
            {
                stream.CopyTo(context.HttpResponse.InnerResponse.OutputStream);
            }
            catch
            {
            }
            finally
            {
                context.HttpResponse.InnerResponse.OutputStream.Dispose();
                stream.Dispose();
            }
            return RequestState.End;
        }

#if FEATURE_ASYNC
        private async Task<RequestState> SendResponseAsync(HttpContext context)
        {
            var stream = context.Result ?? new MemoryStream(new byte[0]);
            try
            {
                await stream.CopyToAsync(context.HttpResponse.InnerResponse.OutputStream);
            }
            catch
            {
            }
            finally
            {
                context.HttpResponse.InnerResponse.OutputStream.Dispose();
                stream.Dispose();
            }
            return RequestState.End;
        }
#endif

        private object[] BindParameters(HttpRequest request, IList<MethodParameter> actionParameters)
        {
            var result = new object[actionParameters.Count];
            for (var i = 0; i < actionParameters.Count; i++)
            {
                var parameter = actionParameters[i];
                var binder = _binderFactory.GetBinder(parameter.Type, parameter.Attributes);
                if (binder != null)
                    result[i] = binder.Bind(parameter.Type, parameter.Name, request);
                else if (parameter.Type.IsValueType)
                    result[i] = Activator.CreateInstance(parameter.Type);
            }
            return result;
        }

        private RequestState InvokeAction(HttpContext context, RequestState failState)
        {
            var instance = (HttpModule) _moduleInstances.GetObjectForThread(context.Route.ActionInvoker.InstanceType);
            instance.InternalRequest = context.HttpRequest;
            instance.InternalResponse = context.HttpResponse;
            var parameters = BindParameters(context.HttpRequest, context.Route.ActionInvoker.Parameters);
            try
            {
                var result = context.Route.ActionInvoker.Invoke(instance, parameters);
                var stream = StreamHelper.ObjectToStream(result, context.HttpResponse.ContentEncoding, context.Route.ActionInvoker.ReturnType);
                context.Result = stream;
                return RequestState.SendResponse;
            }
            catch (Exception e)
            {
                context.HttpResponse.Exception = e;
                context.HttpResponse.StatusCode = HttpStatusCode.InternalServerError;
                return failState;
            }
        }

#if FEATURE_ASYNC
        private async Task<RequestState> InvokeActionAsync(HttpContext context, RequestState failState)
        {
            var instance = (HttpModule) _moduleInstances.GetObjectForThread(context.Route.ActionInvoker.InstanceType);
            instance.InternalRequest = context.HttpRequest;
            instance.InternalResponse = context.HttpResponse;
            var parameters = BindParameters(context.HttpRequest, context.Route.ActionInvoker.Parameters);
            try
            {
                var result = context.Route.ActionInvoker.IsAsync
                    ? await context.Route.ActionInvoker.InvokeAsync(instance, parameters)
                    : context.Route.ActionInvoker.Invoke(instance, parameters);
                var stream = StreamHelper.ObjectToStream(result, context.HttpResponse.ContentEncoding, context.Route.ActionInvoker.ReturnType);
                context.Result = stream;
                return RequestState.SendResponse;
            }
            catch (Exception e)
            {
                context.HttpResponse.Exception = e;
                context.HttpResponse.StatusCode = HttpStatusCode.InternalServerError;
                return failState;
            }
        }
#endif
    }

    internal enum RequestState
    {
        Begin,
        SelectAction,
        InvokeAction,
        SelectStatusCodeAction,
        InvokeStatusCodeAction,
        SendResponse,
        End
    }

    internal class HttpContext
    {
        public HttpRequest HttpRequest { get; }
        public HttpResponse HttpResponse { get; }
        public Route Route { get; set; }
        public Stream Result { get; set; }

        private HttpListenerContext _innerContext;

        public HttpContext(HttpListenerContext httpListenerContext)
        {
            _innerContext = httpListenerContext;
            HttpRequest = new HttpRequest(httpListenerContext.Request);
            HttpResponse = new HttpResponse(httpListenerContext.Response);
        }
    }
}
