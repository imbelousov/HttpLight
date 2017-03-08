using System;
using System.IO;
using HttpLight.Utils;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight
{
    internal class RequestStateMachine : FiniteStateMachine<RequestState, RequestStateMachineContext>
    {
        private ActionCollection _actions;
        private InstanceCollection _controllers;
        private ActionParameterBinderFactory _binderFactory;
        private ActionParameterSourceFactory _parameterSourceFactory;

        public ActionCollection Actions
        {
            get { return _actions; }
        }

        protected override RequestState FirstState
        {
            get { return RequestState.Begin; }
        }

        public RequestStateMachine()
        {
            _actions = new ActionCollection();
            _controllers = new InstanceCollection();
            _binderFactory = new ActionParameterBinderFactory();
            _parameterSourceFactory = new ActionParameterSourceFactory();
            AddState(RequestState.Begin, Begin);
            AddState(RequestState.SelectUsualAction, SelectUsualAction);
            AddState(RequestState.InvokeBeforeActions, InvokeBeforeActions);
            AddState(RequestState.InvokeUsualAction, InvokeUsualAction);
            AddState(RequestState.SelectStatusCodeAction, SelectStatusCodeAction);
            AddState(RequestState.InvokeStatusCodeAction, InvokeStatusCodeAction);
            AddState(RequestState.SendResponse, SendResponse);
#if FEATURE_ASYNC
            AddAsyncState(RequestState.InvokeBeforeActions, InvokeBeforeActionsAsync);
            AddAsyncState(RequestState.InvokeUsualAction, InvokeUsualActionAsync);
            AddAsyncState(RequestState.InvokeStatusCodeAction, InvokeStatusCodeActionAsync);
            AddAsyncState(RequestState.SendResponse, SendResponseAsync);
#endif
        }

        /// <summary>
        /// Initial state
        /// </summary>
        internal RequestState Begin(RequestStateMachineContext context)
        {
            context.Response.StatusCode = HttpStatusCode.Ok;
            return RequestState.SelectUsualAction;
        }

        /// <summary>
        /// Attempt to select an appropriate action from collection
        /// </summary>
        internal RequestState SelectUsualAction(RequestStateMachineContext context)
        {
            bool methodNotAllowed;
            var action = _actions.Get(context.Request.Method, context.Request.Url.LocalPath, out methodNotAllowed);
            if (action == null)
            {
                context.Response.StatusCode = methodNotAllowed
                    ? HttpStatusCode.MethodNotAllowed
                    : HttpStatusCode.NotFound;
            }
            context.Action = action;
            return RequestState.InvokeBeforeActions;
        }

        /// <summary>
        /// Attempt to invoke actions marked with <see cref="Attributes.BeforeAttribute"/>
        /// </summary>
        internal RequestState InvokeBeforeActions(RequestStateMachineContext context)
        {
            try
            {
                var beforeActions = _actions.GetBefore(context.Action != null ? context.Action.Invoker.InstanceType : null);
                foreach (var beforeAction in beforeActions)
                {
                    var result = InvokeAction(beforeAction, context);
                    if (result != null)
                    {
                        context.Result = StreamHelper.ObjectToStream(result, context.Response.ContentEncoding, beforeAction.Invoker.ReturnType);
                        return RequestState.SendResponse;
                    }
                }
            }
            catch (Exception e)
            {
                context.Response.Exception = e;
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Result = null;
                return RequestState.SelectStatusCodeAction;
            }
            return context.Action != null ? RequestState.InvokeUsualAction : RequestState.SelectStatusCodeAction;
        }

#if FEATURE_ASYNC
        /// <summary>
        /// Attempt to invoke actions marked with <see cref="Attributes.BeforeAttribute"/>
        /// </summary>
        internal async Task<RequestState> InvokeBeforeActionsAsync(RequestStateMachineContext context)
        {
            try
            {
                var beforeActions = _actions.GetBefore(context.Action != null ? context.Action.Invoker.InstanceType : null);
                foreach (var beforeAction in beforeActions)
                {
                    var result = await InvokeActionAsync(beforeAction, context);
                    if (result != null)
                    {
                        context.Result = StreamHelper.ObjectToStream(result, context.Response.ContentEncoding, beforeAction.Invoker.ReturnType);
                        return RequestState.SendResponse;
                    }
                }
            }
            catch (Exception e)
            {
                context.Response.Exception = e;
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Result = null;
                return RequestState.SelectStatusCodeAction;
            }
            return context.Action != null ? RequestState.InvokeUsualAction : RequestState.SelectStatusCodeAction;
        }
#endif

        /// <summary>
        /// Attempt to invoke selected action
        /// </summary>
        internal RequestState InvokeUsualAction(RequestStateMachineContext context)
        {
            try
            {
                var result = InvokeAction(context.Action, context);
                context.Result = StreamHelper.ObjectToStream(result, context.Response.ContentEncoding, context.Action.Invoker.ReturnType);
                return RequestState.SendResponse;
            }
            catch (Exception e)
            {
                context.Response.Exception = e;
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Result = null;
                return RequestState.SelectStatusCodeAction;
            }
        }

#if FEATURE_ASYNC
        /// <summary>
        /// Attempt to invoke selected action
        /// </summary>
        internal async Task<RequestState> InvokeUsualActionAsync(RequestStateMachineContext context)
        {
            try
            {
                var result = await InvokeActionAsync(context.Action, context);
                context.Result = StreamHelper.ObjectToStream(result, context.Response.ContentEncoding, context.Action.Invoker.ReturnType);
                return RequestState.SendResponse;
            }
            catch (Exception e)
            {
                context.Response.Exception = e;
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Result = null;
                return RequestState.SelectStatusCodeAction;
            }
        }
#endif

        /// <summary>
        /// Attempt to select an appropriate status code action. Usually this is custom 404 or 500 error page.
        /// </summary>
        internal RequestState SelectStatusCodeAction(RequestStateMachineContext context)
        {
            var action = _actions.Get(context.Response.StatusCode);
            if (action == null)
                return RequestState.SendResponse;
            context.Action = action;
            return RequestState.InvokeStatusCodeAction;
        }

        /// <summary>
        /// Attempt to invoke selected status code action
        /// </summary>
        internal RequestState InvokeStatusCodeAction(RequestStateMachineContext context)
        {
            try
            {
                var result = InvokeAction(context.Action, context);
                context.Result = StreamHelper.ObjectToStream(result, context.Response.ContentEncoding, context.Action.Invoker.ReturnType);
            }
            catch (Exception e)
            {
                context.Response.Exception = e;
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Result = null;
            }
            return RequestState.SendResponse;
        }

#if FEATURE_ASYNC
        /// <summary>
        /// Attempt to invoke selected status code action
        /// </summary>
        internal async Task<RequestState> InvokeStatusCodeActionAsync(RequestStateMachineContext context)
        {
            try
            {
                var result = await InvokeActionAsync(context.Action, context);
                context.Result = StreamHelper.ObjectToStream(result, context.Response.ContentEncoding, context.Action.Invoker.ReturnType);
            }
            catch (Exception e)
            {
                context.Response.Exception = e;
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Result = null;
            }
            return RequestState.SendResponse;
        }
#endif

        /// <summary>
        /// Sending previous results
        /// </summary>
        internal RequestState SendResponse(RequestStateMachineContext context)
        {
            var stream = context.Result ?? new MemoryStream(new byte[0]);
            try
            {
                stream.CopyTo(context.OutputStream);
            }
            catch
            {
            }
            finally
            {
                try
                {
                    context.OutputStream.Dispose();
                    stream.Dispose();
                }
                catch
                {
                }
            }
            return RequestState.End;
        }

#if FEATURE_ASYNC
        internal async Task<RequestState> SendResponseAsync(RequestStateMachineContext context)
        {
            var stream = context.Result ?? new MemoryStream(new byte[0]);
            try
            {
                await stream.CopyToAsync(context.OutputStream);
            }
            catch
            {
            }
            finally
            {
                try
                {
                    context.OutputStream.Dispose();
                    stream.Dispose();
                }
                catch
                {
                }
            }
            return RequestState.End;
        }
#endif

        private object[] BindParameters(IHttpRequest request, Action action)
        {
            var result = new object[action.Invoker.Parameters.Count];
            for (var i = 0; i < action.Invoker.Parameters.Count; i++)
            {
                var parameter = action.Invoker.Parameters[i];
                var binder = _binderFactory.GetBinder(parameter.Type, parameter.Attributes);
                if (binder != null)
                {
                    result[i] = binder.Bind(new ActionParameterBinderContext
                    {
                        Source = _parameterSourceFactory.CreateSource(request, parameter),
                        ParameterName = parameter.Name,
                        ParameterType = parameter.Type,
                        ParameterAttributes = parameter.Attributes
                    });
                }
                else if (parameter.Type.IsValueType)
                    result[i] = Activator.CreateInstance(parameter.Type);
            }
            return result;
        }

        private object InvokeAction(Action action, RequestStateMachineContext context)
        {
            var instance = (Controller) _controllers.GetObjectForThread(action.Invoker.InstanceType);
            instance.Initialize(context.Request, context.Response);
            var parameters = BindParameters(context.Request, action);
            var result = action.Invoker.Invoke(instance, parameters);
            instance.Release();
            return result;
        }

#if FEATURE_ASYNC
        private async Task<object> InvokeActionAsync(Action action, RequestStateMachineContext context)
        {
            var instance = (Controller) _controllers.GetObjectForThread(action.Invoker.InstanceType);
            instance.Initialize(context.Request, context.Response);
            var parameters = BindParameters(context.Request, action);
            var result = await action.Invoker.InvokeAsync(instance, parameters);
            instance.Release();
            return result;
        }
#endif
    }

    internal enum RequestState
    {
        Begin,
        SelectUsualAction,
        InvokeBeforeActions,
        InvokeUsualAction,
        SelectStatusCodeAction,
        InvokeStatusCodeAction,
        SendResponse,
        End
    }

    internal class RequestStateMachineContext
    {
        public IHttpRequest Request { get; protected set; }
        public IHttpResponse Response { get; protected set; }
        public Stream OutputStream { get; protected set; }
        public Action Action { get; set; }
        public Stream Result { get; set; }

        protected RequestStateMachineContext()
        {
        }

        public RequestStateMachineContext(IHttpRequest request, IHttpResponse response, Stream outputStream)
        {
            Request = request;
            Response = response;
            OutputStream = outputStream;
        }
    }
}
