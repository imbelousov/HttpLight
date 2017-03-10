using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HttpLight.Attributes;
using HttpLight.Test.Utils;
using NUnit.Framework;

#if !FEATURE_STREAMCOPYTO
using HttpLight.Utils;
#endif

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class RequestStateMachineTest
    {
        private static IEnumerable<TestCaseData> SelectUsualActionData
        {
            get
            {
                yield return new TestCaseData("/test", HttpMethod.Get, "/test", HttpMethod.Get)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode>(RequestState.InvokeBeforeActions, default(HttpStatusCode)),
                    TestName = "Ok"
                };
                yield return new TestCaseData("/test", HttpMethod.Get, "/qwerty", HttpMethod.Get)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode>(RequestState.InvokeBeforeActions, HttpStatusCode.NotFound),
                    TestName = "NotFound"
                };
                yield return new TestCaseData("/test", HttpMethod.Get, "/test", HttpMethod.Post)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode>(RequestState.InvokeBeforeActions, HttpStatusCode.MethodNotAllowed),
                    TestName = "MethodNotAllowed"
                };
            }
        }

        private static IEnumerable<TestCaseData> InvokeBeforeActionsData
        {
            get
            {
                yield return new TestCaseData(new string[0], new bool[0])
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SelectStatusCodeAction, default(HttpStatusCode), string.Empty),
                    TestName = "No actions"
                };
                yield return new TestCaseData(new string[] {null}, new[] {false})
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.InvokeUsualAction, default(HttpStatusCode), string.Empty),
                    TestName = "Return void"
                };
                yield return new TestCaseData(new[] {string.Empty}, new[] {false})
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SendResponse, default(HttpStatusCode), string.Empty),
                    TestName = "Return empty string"
                };
                yield return new TestCaseData(new[] {"1"}, new[] {false})
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SendResponse, default(HttpStatusCode), "1"),
                    TestName = "Return string"
                };
                yield return new TestCaseData(new string[] {null}, new[] {true})
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SelectStatusCodeAction, HttpStatusCode.InternalServerError, string.Empty),
                    TestName = "Throw"
                };
            }
        }

        private static IEnumerable<TestCaseData> InvokeUsualActionData
        {
            get
            {
                yield return new TestCaseData(null, false)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SendResponse, default(HttpStatusCode), string.Empty),
                    TestName = "Return void"
                };
                yield return new TestCaseData("test", false)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SendResponse, default(HttpStatusCode), "test"),
                    TestName = "Return string"
                };
                yield return new TestCaseData(null, true)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SelectStatusCodeAction, HttpStatusCode.InternalServerError, string.Empty),
                    TestName = "Throw"
                };
            }
        }

        private static IEnumerable<TestCaseData> InvokeStatusCodeActionData
        {
            get
            {
                yield return new TestCaseData("test", false)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SendResponse, default(HttpStatusCode), "test"),
                    TestName = "Ok"
                };
                yield return new TestCaseData("test", true)
                {
                    ExpectedResult = new Tuple<RequestState, HttpStatusCode, string>(RequestState.SendResponse, HttpStatusCode.InternalServerError, string.Empty),
                    TestName = "InternalServerError"
                };
            }
        }

        private static IEnumerable<TestCaseData> SendResponseData
        {
            get
            {
                yield return new TestCaseData("test", false, false)
                {
                    ExpectedResult = new Tuple<RequestState, string>(RequestState.End, "test"),
                    TestName = "Ok"
                };
                yield return new TestCaseData("test", true, false)
                {
                    ExpectedResult = new Tuple<RequestState, string>(RequestState.End, string.Empty),
                    TestName = "One exception"
                };
                yield return new TestCaseData("test", false, true)
                {
                    ExpectedResult = new Tuple<RequestState, string>(RequestState.End, string.Empty),
                    TestName = "Two exceptions"
                };
            }
        }

        private static IEnumerable<TestCaseData> StartData
        {
            get
            {
                yield return new TestCaseData(new RequestStateMachineScenario())
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.NotFound, string.Empty),
                    TestName = "Empty"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("test")
                        .NewAction("NotFound")
                        .AddAttribute(typeof(StatusCodeAttribute), HttpStatusCode.NotFound)
                        .SetReturnType(typeof(string))
                        .SetReturnValue("NotFound")
                        .Build()
                        .Invoker
                        .InstanceType
                    )
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.Ok, "test"),
                    TestName = "Usual action"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .NewAction("NotFound")
                        .AddAttribute(typeof(StatusCodeAttribute), HttpStatusCode.NotFound)
                        .SetReturnType(typeof(string))
                        .SetReturnValue("NotFound")
                        .Build()
                        .Invoker
                        .InstanceType)
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/qwerty"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.NotFound, "NotFound"),
                    TestName = "Status code 404 action"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Test")
                        .AddAttribute(typeof(PostAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("test")
                        .NewAction("MethodNotAllowed")
                        .AddAttribute(typeof(StatusCodeAttribute), HttpStatusCode.MethodNotAllowed)
                        .SetReturnType(typeof(string))
                        .SetReturnValue("MethodNotAllowed")
                        .Build()
                        .Invoker
                        .InstanceType
                    )
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.MethodNotAllowed, "MethodNotAllowed"),
                    TestName = "Status code 405 action"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .Throws(typeof(Exception))
                        .NewAction("InternalServerError")
                        .AddAttribute(typeof(StatusCodeAttribute), HttpStatusCode.InternalServerError)
                        .SetReturnType(typeof(string))
                        .SetReturnValue("InternalServerError")
                        .Build()
                        .Invoker
                        .InstanceType
                    )
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.InternalServerError, "InternalServerError"),
                    TestName = "Status code 500 action"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Before")
                        .AddAttribute(typeof(BeforeAttribute))
                        .NewAction("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("test")
                        .Build()
                        .Invoker
                        .InstanceType
                    )
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.Ok, "test"),
                    TestName = "Before action without output"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Before")
                        .AddAttribute(typeof(BeforeAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("before")
                        .NewAction("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("test")
                        .Build()
                        .Invoker
                        .InstanceType
                    )
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.Ok, "before"),
                    TestName = "Before action with output"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Before")
                        .AddAttribute(typeof(BeforeAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("before")
                        .Build()
                        .Invoker
                        .InstanceType)
                    .AddController(new ActionBuilder("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("test")
                        .Build()
                        .Invoker
                        .InstanceType)
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.Ok, "test"),
                    TestName = "Before action in another controller"
                };
                yield return new TestCaseData(new RequestStateMachineScenario().AddController(new ActionBuilder("Test")
                        .AddAttribute(typeof(GetAttribute))
                        .AddParameter("a", typeof(int))
                        .AddParameter("b", typeof(int))
                        .AddParameter("c", typeof(int), typeof(FromUrlAttribute), "cc")
                        .AddParameter("d", typeof(int), typeof(FromUrlAttribute))
                        .AddParameter("e", typeof(int), typeof(FromContentAttribute), "ee")
                        .AddParameter("f", typeof(int), typeof(FromContentAttribute))
                        .AddParameter("g", typeof(int), typeof(BinderAttribute), typeof(PrimitiveBinder))
                        .AddParameter("h", typeof(int), typeof(BinderAttribute), typeof(PrimitiveBinder))
                        .AddParameter("i", typeof(object))
                        .AddParameter("j", typeof(DateTime))
                        .SetReturnType(typeof(string))
                        .SetReturnValue("test")
                        .Build()
                        .Invoker
                        .InstanceType
                    )
                    .SetMethod(HttpMethod.Get)
                    .SetPathAndQuery("/test?a=3&cc=4&ee=5"))
                {
                    ExpectedResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.Ok, "test"),
                    TestName = "Binding"
                };
            }
        }

        [Test]
        public void Begin()
        {
            var stateMachine = new RequestStateMachine();
            var context = new FakeRequestStateMachineContext();
            var result = stateMachine.Begin(context);
            Assert.AreEqual(RequestState.SelectUsualAction, result);
        }

        [TestCaseSource(nameof(SelectUsualActionData))]
        public object SelectUsualAction(string actionPath, string actionMethod, string requestPath, string requestMethod)
        {
            var stateMachine = new RequestStateMachine();
            var context = new FakeRequestStateMachineContext(requestPath, requestMethod);
            var action = new ActionBuilder("Test").Build();
            stateMachine.Actions.Add(actionMethod, actionPath, action);
            var state = stateMachine.SelectUsualAction(context);
            return new Tuple<RequestState, HttpStatusCode>(state, context.Response.StatusCode);
        }

        [TestCaseSource(nameof(InvokeBeforeActionsData))]
        public object InvokeBeforeActions(string[] returnValues, bool[] throws)
        {
            var stateMachine = new RequestStateMachine();
            var context = new FakeRequestStateMachineContext();
            for (var i = 0; i < returnValues.Length; i++)
            {
                var returnValue = returnValues[i];
                var actionBuilder = new ActionBuilder("Test" + i);
                if (returnValue != null)
                    actionBuilder = actionBuilder.SetReturnType(typeof(string)).SetReturnValue(returnValue);
                if (throws[i])
                    actionBuilder = actionBuilder.Throws(typeof(Exception));
                var action = actionBuilder.Build();
                stateMachine.Actions.AddBefore(action);
                context.Action = action;
            }
            var state = stateMachine.InvokeBeforeActions(context);
            return new Tuple<RequestState, HttpStatusCode, string>(state, context.Response.StatusCode, StreamToString(context.Result));
        }

#if FEATURE_ASYNC
        [TestCaseSource(nameof(InvokeBeforeActionsData))]
        public object InvokeBeforeActionsAsync(string[] returnValues, bool[] throws)
        {
            var stateMachine = new RequestStateMachine();
            var context = new FakeRequestStateMachineContext();
            for (var i = 0; i < returnValues.Length; i++)
            {
                var returnValue = returnValues[i];
                var actionBuilder = new ActionBuilder("Test" + i);
                if (returnValue != null)
                    actionBuilder = actionBuilder.SetReturnType(typeof(string)).SetReturnValue(returnValue);
                if (throws[i])
                    actionBuilder = actionBuilder.Throws(typeof(Exception));
                var action = actionBuilder.Build();
                stateMachine.Actions.AddBefore(action);
                context.Action = action;
            }
            var state = stateMachine.InvokeBeforeActionsAsync(context).Result;
            return new Tuple<RequestState, HttpStatusCode, string>(state, context.Response.StatusCode, StreamToString(context.Result));
        }
#endif

        [TestCaseSource(nameof(InvokeUsualActionData))]
        public object InvokeUsualAction(string returnValue, bool throws)
        {
            var stateMachine = new RequestStateMachine();
            var actionBuilder = new ActionBuilder("Test");
            if (returnValue != null)
                actionBuilder = actionBuilder.SetReturnType(typeof(string)).SetReturnValue(returnValue);
            if (throws)
                actionBuilder = actionBuilder.Throws(typeof(Exception));
            var action = actionBuilder.Build();
            var context = new FakeRequestStateMachineContext();
            context.Action = action;
            var state = stateMachine.InvokeUsualAction(context);
            return new Tuple<RequestState, HttpStatusCode, string>(state, context.Response.StatusCode, StreamToString(context.Result));
        }

#if FEATURE_ASYNC
        [TestCaseSource(nameof(InvokeUsualActionData))]
        public object InvokeUsualActionAsync(string returnValue, bool throws)
        {
            var stateMachine = new RequestStateMachine();
            var actionBuilder = new ActionBuilder("Test");
            if (returnValue != null)
                actionBuilder = actionBuilder.SetReturnType(typeof(string)).SetReturnValue(returnValue);
            if (throws)
                actionBuilder = actionBuilder.Throws(typeof(Exception));
            var action = actionBuilder.Build();
            var context = new FakeRequestStateMachineContext();
            context.Action = action;
            var state = stateMachine.InvokeUsualActionAsync(context).Result;
            return new Tuple<RequestState, HttpStatusCode, string>(state, context.Response.StatusCode, StreamToString(context.Result));
        }
#endif

        [TestCase(HttpStatusCode.NotFound, HttpStatusCode.NotFound, ExpectedResult = RequestState.InvokeStatusCodeAction, TestName = "Found")]
        [TestCase(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError, ExpectedResult = RequestState.SendResponse, TestName = "Not found")]
        public object SelectStatusCodeAction(HttpStatusCode registeredStatusCode, HttpStatusCode setStatusCode)
        {
            var stateMachine = new RequestStateMachine();
            var action = new ActionBuilder("Test").Build();
            var context = new FakeRequestStateMachineContext();
            stateMachine.Actions.Add(registeredStatusCode, action);
            context.Response.StatusCode = setStatusCode;
            var state = stateMachine.SelectStatusCodeAction(context);
            return state;
        }

        [TestCaseSource(nameof(InvokeStatusCodeActionData))]
        public object InvokeStatusCodeAction(string returnValue, bool throws)
        {
            var stateMachine = new RequestStateMachine();
            var actionBuilder = new ActionBuilder("Test");
            if (returnValue != null)
                actionBuilder = actionBuilder.SetReturnType(typeof(string)).SetReturnValue(returnValue);
            if (throws)
                actionBuilder = actionBuilder.Throws(typeof(Exception));
            var action = actionBuilder.Build();
            var context = new FakeRequestStateMachineContext();
            context.Action = action;
            var state = stateMachine.InvokeStatusCodeAction(context);
            return new Tuple<RequestState, HttpStatusCode, string>(state, context.Response.StatusCode, StreamToString(context.Result));
        }

#if FEATURE_ASYNC
        [TestCaseSource(nameof(InvokeStatusCodeActionData))]
        public object InvokeStatusCodeActionAsync(string returnValue, bool throws)
        {
            var stateMachine = new RequestStateMachine();
            var actionBuilder = new ActionBuilder("Test");
            if (returnValue != null)
                actionBuilder = actionBuilder.SetReturnType(typeof(string)).SetReturnValue(returnValue);
            if (throws)
                actionBuilder = actionBuilder.Throws(typeof(Exception));
            var action = actionBuilder.Build();
            var context = new FakeRequestStateMachineContext();
            context.Action = action;
            var state = stateMachine.InvokeStatusCodeActionAsync(context).Result;
            return new Tuple<RequestState, HttpStatusCode, string>(state, context.Response.StatusCode, StreamToString(context.Result));
        }
#endif

        [TestCaseSource(nameof(SendResponseData))]
        public object SendResponse(string result, bool dispose, bool nullOutputStream)
        {
            var stateMachine = new RequestStateMachine();
            var context = new FakeRequestStateMachineContext();
            context.Result = StringToStream(result);
            if (dispose)
                context.OutputStream.Dispose();
            if (nullOutputStream)
                context.SetOutputStream(null);
            var state = stateMachine.SendResponse(context);
            var output = StreamToString(new MemoryStream(((MemoryStream) context.OutputStream ?? new MemoryStream()).ToArray()));
            return new Tuple<RequestState, string>(state, output);
        }

#if FEATURE_ASYNC
        [TestCaseSource(nameof(SendResponseData))]
        public object SendResponseAsync(string result, bool dispose, bool nullOutputStream)
        {
            var stateMachine = new RequestStateMachine();
            var context = new FakeRequestStateMachineContext();
            context.Result = StringToStream(result);
            if (dispose)
                context.OutputStream.Dispose();
            if (nullOutputStream)
                context.SetOutputStream(null);
            var state = stateMachine.SendResponseAsync(context).Result;
            var output = StreamToString(new MemoryStream(((MemoryStream) context.OutputStream ?? new MemoryStream()).ToArray()));
            return new Tuple<RequestState, string>(state, output);
        }
#endif

        [TestCaseSource(nameof(StartData))]
        public object Start(object scenario)
        {
            var scenarioInstance = (RequestStateMachineScenario) scenario;
            var stateMachine = scenarioInstance.GetRequestStateMachine();
            var context = scenarioInstance.GetContext();
            stateMachine.Start(context);
            var output = StreamToString(new MemoryStream(((MemoryStream) context.OutputStream ?? new MemoryStream()).ToArray()));
            return new Tuple<HttpStatusCode, string>(context.Response.StatusCode, output);
        }

#if FEATURE_ASYNC
        [TestCaseSource(nameof(StartData))]
        public object StartAsync(object scenario)
        {
            var scenarioInstance = (RequestStateMachineScenario) scenario;
            var stateMachine = scenarioInstance.GetRequestStateMachine();
            var context = scenarioInstance.GetContext();
            stateMachine.StartAsync(context).Wait();
            var output = StreamToString(new MemoryStream(((MemoryStream) context.OutputStream ?? new MemoryStream()).ToArray()));
            return new Tuple<HttpStatusCode, string>(context.Response.StatusCode, output);
        }
#endif

        private static string StreamToString(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream = stream ?? new MemoryStream();
                stream.CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static Stream StringToStream(string s)
        {
            if (s == null)
                return null;
            var buf = Encoding.UTF8.GetBytes(s);
            var ms = new MemoryStream(buf);
            return ms;
        }
    }
}
