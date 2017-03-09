using System;

namespace HttpLight.Test.Utils
{
    internal class RequestStateMachineScenario
    {
        private RequestStateMachine _requestStateMachine;
        private ControllerCollection _controllers;
        private string _pathAndQuery;
        private string _method;

        public RequestStateMachineScenario()
        {
            _requestStateMachine = new RequestStateMachine();
            _controllers = new ControllerCollection(_requestStateMachine.Actions);
            _pathAndQuery = "/";
            _method = HttpMethod.Get;
        }

        public RequestStateMachineScenario AddController(Type controller)
        {
            _controllers.Add(controller);
            return this;
        }

        public RequestStateMachineScenario SetPathAndQuery(string pathAndQuery)
        {
            _pathAndQuery = pathAndQuery;
            return this;
        }

        public RequestStateMachineScenario SetMethod(string method)
        {
            _method = method;
            return this;
        }

        public RequestStateMachine GetRequestStateMachine()
        {
            return _requestStateMachine;
        }

        public FakeRequestStateMachineContext GetContext()
        {
            var context = new FakeRequestStateMachineContext(_pathAndQuery, _method);
            return context;
        }
    }
}
