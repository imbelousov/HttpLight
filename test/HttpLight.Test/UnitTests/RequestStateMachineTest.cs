using System;
using System.IO;
using System.Reflection;
using System.Text;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using HttpLight.Attributes;
using HttpLight.Test.Utils;
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class RequestStateMachineTest
    {
        private const string BaseUrl = "http://localhost:8080";
        private const string GetUrl = BaseUrl + "/Get";
        private const string NotExistUrl = BaseUrl + "/NotExist";
        private const string ErrorGetUrl = BaseUrl + "/ErrorGet";
#if FEATURE_ASYNC
        private const string GetAsyncUrl = BaseUrl + "/GetAsync";
        private const string ErrorGetAsyncUrl = BaseUrl + "/ErrorGetAsync";
#endif

        [Test]
        public void Begin()
        {
            var stateMachine = new RequestStateMachine();
            var context = CreateContext();
            var state = stateMachine.Begin(context);
            Assert.AreEqual(RequestState.SelectUsualAction, state);
        }

        [Test]
        public void SelectAction_Exist()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext(GetUrl);
            var state = stateMachine.SelectUsualAction(context);
            Assert.AreEqual(RequestState.InvokeUsualAction, state);
        }

        [Test]
        public void SelectAction_NotExist()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext(NotExistUrl);
            var state = stateMachine.SelectUsualAction(context);
            Assert.AreEqual(RequestState.SelectStatusCodeAction, state);
            Assert.AreEqual(HttpStatusCode.NotFound, context.HttpResponse.StatusCode);
        }

        [Test]
        public void SelectAction_InvalidMethod()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext(GetUrl, HttpMethod.Post);
            var state = stateMachine.SelectUsualAction(context);
            Assert.AreEqual(RequestState.SelectStatusCodeAction, state);
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, context.HttpResponse.StatusCode);
        }

        [Test]
        public void InvokeAction_Success()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            bool methodNotAllowed;
            var context = CreateContext(GetUrl);
            context.Action = stateMachine.Actions.Get(HttpMethod.Get, context.HttpRequest.Url.LocalPath, out methodNotAllowed);
            var state = stateMachine.InvokeUsualAction(context);
            var result = StreamToString(context.Result);
            Assert.AreEqual(RequestState.SendResponse, state);
            Assert.AreEqual(HttpMethod.Get.ToString(), result);
        }

        [Test]
        public void InvokeAction_Error()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            bool methodNotAllowed;
            var context = CreateContext(ErrorGetUrl);
            context.Action = stateMachine.Actions.Get(HttpMethod.Get, context.HttpRequest.Url.LocalPath, out methodNotAllowed);
            var state = stateMachine.InvokeUsualAction(context);
            Assert.IsNull(context.Result);
            Assert.AreEqual(RequestState.SelectStatusCodeAction, state);
            Assert.AreEqual(HttpStatusCode.InternalServerError, context.HttpResponse.StatusCode);
        }

#if FEATURE_ASYNC
        [TestCase(GetUrl, TestName = "Sync")]
        [TestCase(GetAsyncUrl, TestName = "Async")]
        public void InvokeActionAsync_Success(string url)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            bool methodNotAllowed;
            var context = CreateContext(url);
            context.Action = stateMachine.Actions.Get(HttpMethod.Get, context.HttpRequest.Url.LocalPath, out methodNotAllowed);
            var state = stateMachine.InvokeUsualActionAsync(context).Result;
            var result = StreamToString(context.Result);
            Assert.AreEqual(RequestState.SendResponse, state);
            Assert.AreEqual(HttpMethod.Get.ToString(), result);
        }

        [TestCase(ErrorGetUrl, TestName = "Sync")]
        [TestCase(ErrorGetAsyncUrl, TestName = "Async")]
        public void InvokeActionAsync_Error(string url)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            bool methodNotAllowed;
            var context = CreateContext(url);
            context.Action = stateMachine.Actions.Get(HttpMethod.Get, context.HttpRequest.Url.LocalPath, out methodNotAllowed);
            var state = stateMachine.InvokeUsualActionAsync(context).Result;
            Assert.IsNull(context.Result);
            Assert.AreEqual(RequestState.SelectStatusCodeAction, state);
            Assert.AreEqual(HttpStatusCode.InternalServerError, context.HttpResponse.StatusCode);
        }
#endif

        [TestCase(HttpStatusCode.NotFound, ExpectedResult = RequestState.InvokeStatusCodeAction, TestName = "Exist")]
        [TestCase(HttpStatusCode.InternalServerError, ExpectedResult = RequestState.SendResponse, TestName = "Not exist")]
        public object SelectStatusCodeAction(object httpStatusCode)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext();
            context.HttpResponse.StatusCode = (HttpStatusCode) httpStatusCode;
            var state = stateMachine.SelectStatusCodeAction(context);
            return state;
        }

        [Test]
        public void InvokeStatusCodeAction_Success()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext();
            context.Action = stateMachine.Actions.Get(HttpStatusCode.NotFound);
            var state = stateMachine.InvokeStatusCodeAction(context);
            var result = StreamToString(context.Result);
            Assert.AreEqual(RequestState.SendResponse, state);
            Assert.AreEqual(HttpStatusCode.NotFound.ToString(), result);
        }

        [Test]
        public void InvokeStatusCodeAction_Error()
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext();
            context.Action = stateMachine.Actions.Get(HttpStatusCode.MethodNotAllowed);
            var state = stateMachine.InvokeStatusCodeAction(context);
            Assert.IsNull(context.Result);
            Assert.AreEqual(RequestState.SendResponse, state);
        }

#if FEATURE_ASYNC
        [TestCase(HttpStatusCode.NotFound, ExpectedResult = nameof(HttpStatusCode.NotFound), TestName = "Sync")]
        [TestCase(HttpStatusCode.BadRequest, ExpectedResult = nameof(HttpStatusCode.BadRequest), TestName = "Async")]
        public string InvokeStatusCodeActionAsync_Success(object httpStatusCode)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext();
            context.Action = stateMachine.Actions.Get((HttpStatusCode) httpStatusCode);
            var state = stateMachine.InvokeStatusCodeActionAsync(context).Result;
            var result = StreamToString(context.Result);
            Assert.AreEqual(RequestState.SendResponse, state);
            return result;
        }

        [TestCase(HttpStatusCode.MethodNotAllowed, TestName = "Sync")]
        [TestCase(HttpStatusCode.Unauthorized, TestName = "Async")]
        public void InvokeStatusCodeActionAsync_Error(object httpStatusCode)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var context = CreateContext();
            context.Action = stateMachine.Actions.Get((HttpStatusCode) httpStatusCode);
            var state = stateMachine.InvokeStatusCodeActionAsync(context).Result;
            Assert.IsNull(context.Result);
            Assert.AreEqual(RequestState.SendResponse, state);
        }
#endif

        [Test]
        public void SendResponse_Success()
        {
            var stateMachine = new RequestStateMachine();
            var outputStream = new MemoryStream();
            var resultStream = new MemoryStream(new byte[] {1, 2, 3, 4});
            var context = CreateContext(outputStream: outputStream);
            context.Result = resultStream;
            var state = stateMachine.SendResponse(context);
            Assert.AreEqual(RequestState.End, state);
            CollectionAssert.AreEqual(resultStream.ToArray(), outputStream.ToArray());
        }

        [TestCase(false, TestName = "Disposed")]
        [TestCase(true, TestName = "Null")]
        public void SendResponse_Error(bool errorOnDispose)
        {
            var stateMachine = new RequestStateMachine();
            var outputStream = !errorOnDispose ? new MemoryStream() : null;
            if (!errorOnDispose)
                outputStream.Dispose();
            var resultStream = new MemoryStream(new byte[] {1, 2, 3, 4});
            var context = CreateContext(outputStream: outputStream);
            context.Result = resultStream;
            var state = stateMachine.SendResponse(context);
            Assert.AreEqual(RequestState.End, state);
            if (!errorOnDispose)
                CollectionAssert.AreEqual(new byte[0], outputStream.ToArray());
        }

#if FEATURE_ASYNC
        [Test]
        public void SendResponseAsync_Success()
        {
            var stateMachine = new RequestStateMachine();
            var outputStream = new MemoryStream();
            var resultStream = new MemoryStream(new byte[] {1, 2, 3, 4});
            var context = CreateContext(outputStream: outputStream);
            context.Result = resultStream;
            var state = stateMachine.SendResponseAsync(context).Result;
            Assert.AreEqual(RequestState.End, state);
            CollectionAssert.AreEqual(resultStream.ToArray(), outputStream.ToArray());
        }

        [TestCase(false, TestName = "Disposed")]
        [TestCase(true, TestName = "Null")]
        public void SendResponseAsync_Error(bool errorOnDispose)
        {
            var stateMachine = new RequestStateMachine();
            var outputStream = !errorOnDispose ? new MemoryStream() : null;
            if (!errorOnDispose)
                outputStream.Dispose();
            var resultStream = new MemoryStream(new byte[] {1, 2, 3, 4});
            var context = CreateContext(outputStream: outputStream);
            context.Result = resultStream;
            var state = stateMachine.SendResponseAsync(context).Result;
            Assert.AreEqual(RequestState.End, state);
            if (!errorOnDispose)
                CollectionAssert.AreEqual(new byte[0], outputStream.ToArray());
        }
#endif

        [TestCase(GetUrl + "?a=5", 5, 0, null, 0, TestName = "Custom binder")]
        [TestCase(GetUrl + "?b=5", 0, 5, null, 0, TestName = "Primitives binder")]
        [TestCase(GetUrl + "?c=5", 0, 0, null, 0, TestName = "Default value (reference type)")]
        [TestCase(GetUrl + "?d=5", 0, 0, null, 0, TestName = "Default value (value type)")]
        [TestCase(GetUrl, 0, 0, null, 0, TestName = "Default value (all parameters)")]
        public void BindParameters(string url, int? expectedA, int expectedB, int? expectedC, int expectedD)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var bindParameters = stateMachine.GetType().GetMethod("BindParameters", BindingFlags.Instance | BindingFlags.NonPublic);
            var context = CreateContext(url);
            bool methodNotAllowed;
            context.Action = stateMachine.Actions.Get(HttpMethod.Get, new Uri(url).LocalPath, out methodNotAllowed);
            var parameters = (object[]) bindParameters.Invoke(stateMachine, new object[] {context.HttpRequest, context.Action.Invoker.Parameters});
            var a = parameters[0] as CustomModel;
            var b = (int) parameters[1];
            var c = parameters[2] as CustomModel;
            var d = (CustomValueType) parameters[3];
            Assert.AreEqual(expectedA, a != null ? (int?) a.Value : null);
            Assert.AreEqual(expectedB, b);
            Assert.AreEqual(expectedC, c != null ? (int?) c.Value : null);
            Assert.AreEqual(expectedD, d.Value);
        }

        [TestCase(GetUrl, HttpMethod.Get, ExpectedResult = nameof(HttpMethod.Get), TestName = "Success")]
        [TestCase(GetUrl, HttpMethod.Post, ExpectedResult = "", TestName = "Method not allowed")]
        [TestCase(NotExistUrl, HttpMethod.Get, ExpectedResult = nameof(HttpStatusCode.NotFound), TestName = "Not found")]
        [TestCase(ErrorGetUrl, HttpMethod.Get, ExpectedResult = "", TestName = "Error")]
        public string Complex(string url, object method)
        {
            var stateMachine = new RequestStateMachine();
            InitActions(stateMachine);
            var outputStream = new SavingDataMemoryStream();
            var context = CreateContext(url, (HttpMethod) method, outputStream);
            stateMachine.Start(context);
            var result = StreamToString(outputStream.DataBeforeDispose);
            return result;
        }

        private RequestStateMachineContext CreateContext(string url = null, HttpMethod method = HttpMethod.Get, Stream outputStream = null)
        {
            var request = string.IsNullOrEmpty(url)
                ? new FakeHttpRequest()
                : new FakeHttpRequest(url);
            request.Method = method;
            var response = new FakeHttpResponse();
            if (outputStream != null)
                response.OutputStream = outputStream;
            return new RequestStateMachineContext(request, response, response.OutputStream);
        }

        private void InitActions(RequestStateMachine stateMachine)
        {
            var controllers = new ControllerCollection(stateMachine.Actions);
            controllers.Add<RequestStateMachineTestController>();
        }

        private string StreamToString(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    internal class RequestStateMachineTestController : Controller
    {
        [Get]
        [Path("/Get")]
        public string GetTestAction([Binder(typeof(CustomBinder))] CustomModel a, int b, CustomModel c, CustomValueType d)
        {
            return HttpMethod.Get.ToString();
        }

        [Get]
        [Path("/ErrorGet")]
        public string ErrorTestAction()
        {
            throw new Exception();
        }

        [StatusCode(HttpStatusCode.NotFound)]
        public string NotFoundTestAction()
        {
            return HttpStatusCode.NotFound.ToString();
        }

        [StatusCode(HttpStatusCode.MethodNotAllowed)]
        public string MethodNotAllowedTestAction()
        {
            throw new Exception();
        }

#if FEATURE_ASYNC
        [Get]
        [Path("/GetAsync")]
        public Task<string> GetTestActionAsync([Binder(typeof(CustomBinder))] CustomModel a, int b, CustomModel c, CustomValueType d)
        {
            return Task.FromResult(HttpMethod.Get.ToString());
        }

        [Get]
        [Path("/ErrorGetAsync")]
        public Task<string> ErrorTestActionAsync()
        {
            throw new Exception();
        }

        [StatusCode(HttpStatusCode.BadRequest)]
        public Task<string> NotFoundTestActionAsync()
        {
            return Task.FromResult(HttpStatusCode.BadRequest.ToString());
        }

        [StatusCode(HttpStatusCode.Unauthorized)]
        public Task<string> MethodNotAllowedTestActionAsync()
        {
            throw new Exception();
        }
#endif
    }

    internal class CustomBinder : IActionParameterBinder
    {
        public object Bind(ActionParameterBinderContext actionParameterBinderContext)
        {
            var value = (int) SafeStringConvert.ChangeType(actionParameterBinderContext.HttpRequest.UrlParameters.Get(actionParameterBinderContext.ParameterName), typeof(int));
            return new CustomModel {Value = value};
        }
    }

    internal class CustomModel
    {
        public int Value { get; set; }
    }

    internal struct CustomValueType
    {
        public int Value { get; set; }
    }

    internal class SavingDataMemoryStream : MemoryStream
    {
        public Stream DataBeforeDispose { get; private set; }

        protected override void Dispose(bool disposing)
        {
            DataBeforeDispose = new MemoryStream(ToArray());
            base.Dispose(disposing);
        }
    }
}
