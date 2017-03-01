using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HttpLight.Utils;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight
{
    internal class RequestHandler
    {
        private RouteCollection _routes;
        private InstanceCollection _moduleInstances;
        private ActionBinderFactory _binderFactory;

        public RouteCollection Routes
        {
            get { return _routes; }
        }

        public RequestHandler()
        {
            _routes = new RouteCollection();
            _moduleInstances = new InstanceCollection();
            _binderFactory = new ActionBinderFactory();
        }

        public void HandleRequest(HttpListenerContext context)
        {
        }

#if FEATURE_ASYNC
        public async Task HandleRequestAsync(HttpListenerContext context)
        {
            var stream = context.Response.OutputStream;
            using (stream)
            {
                var request = new HttpRequest(context.Request);
                var response = new HttpResponse(context.Response);
                bool methodNotAllowed;
                var route = _routes.Get(request.HttpMethod, request.Url.LocalPath, out methodNotAllowed);
                if (route != null)
                {
                    var instance = (HttpModule) _moduleInstances.GetObjectForThread(route.ActionInvoker.InstanceType);
                    instance.InternalRequest = request;
                    instance.InternalResponse = response;
                    var parameters = GetActionParameters(request, route.ActionInvoker.Parameters);
                    try
                    {
                        response.StatusCode = HttpStatusCode.Ok;
                        var result = route.ActionInvoker.IsAsync
                            ? await route.ActionInvoker.InvokeAsync(instance, parameters)
                            : route.ActionInvoker.Invoke(instance, parameters);
                        var resultStream = StreamHelper.ObjectToStream(result, route.ActionInfo);
                        await resultStream.CopyToAsync(stream);
                    }
                    catch (Exception e)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    response.StatusCode = methodNotAllowed
                        ? HttpStatusCode.MethodNotAllowed
                        : HttpStatusCode.NotFound;
                }
            }
        }
#endif

        private object[] GetActionParameters(HttpRequest request, IList<MethodParameter> actionParameters)
        {
            var result = new object[actionParameters.Count];
            for (var i = 0; i < actionParameters.Count; i++)
            {
                var parameter = actionParameters[i];
                var binder = _binderFactory.GetBinder(parameter.Type, parameter.Attributes);
                if (binder != null)
                {
                    result[i] = binder.Bind(parameter.Type, parameter.Name, request);
                }
            }
            return result;
        }
    }
}
