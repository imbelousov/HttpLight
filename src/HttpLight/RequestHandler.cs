using System;
using System.Collections.Generic;
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

        public RouteCollection Routes
        {
            get { return _routes; }
        }

        public RequestHandler()
        {
            _routes = new RouteCollection();
            _moduleInstances = new InstanceCollection();
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
                var route = _routes.Get(request.HttpMethod, request.Url.LocalPath);
                if (route != null)
                {
                    var instance = (HttpModule) _moduleInstances.GetObjectForThread(route.ActionInvoker.InstanceType);
                    instance.InternalRequest = request;
                    instance.InternalResponse = response;
                    var parameters = GetActionParameters(request, route.ActionInvoker.Parameters);
                    var result = route.ActionInvoker.IsAsync
                        ? await route.ActionInvoker.InvokeAsync(instance, parameters)
                        : route.ActionInvoker.Invoke(instance, parameters);
                    var resultStream = StreamHelper.ObjectToStream(result, route.ActionInfo);
                    await resultStream.CopyToAsync(stream);
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
                var values = request.UrlParameters.GetValues(parameter.Name);
                if (!parameter.Type.IsArray)
                {
                    if (values == null || values.Length == 0)
                        result[i] = SafeStringConvert.ChangeType(null as string, parameter.Type);
                    else
                        result[i] = SafeStringConvert.ChangeType(values[0], parameter.Type);
                }
                else if (parameter.Type.GetArrayRank() == 1)
                {
                    var elementType = parameter.Type.GetElementType();
                    if (values == null || values.Length == 0)
                        result[i] = Array.CreateInstance(elementType, 0);
                    else
                    {
                        var converted = SafeStringConvert.ChangeType(values, elementType);
                        var array = Array.CreateInstance(elementType, converted.Length);
                        for (var j = 0; j < converted.Length; j++)
                            array.SetValue(converted[j], j);
                        result[i] = array;
                    }
                }
            }
            return result;
        }
    }
}
