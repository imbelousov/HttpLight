using System.Collections.Generic;
using HttpLight.Utils;

namespace HttpLight
{
    internal class RouteCollection
    {
        private IDictionary<string, IDictionary<HttpMethod, Route>> _actions;
        private IDictionary<HttpStatusCode, Route> _statusCodePages;

        public RouteCollection()
        {
            _actions = new Dictionary<string, IDictionary<HttpMethod, Route>>();
            _statusCodePages = new Dictionary<HttpStatusCode, Route>();
        }

        public void Add(HttpMethod httpMethod, string path, Route route)
        {
            path = NormalizePath(path);
            IDictionary<HttpMethod, Route> innerDictionary;
            if (!_actions.TryGetValue(path, out innerDictionary))
            {
                innerDictionary = new Dictionary<HttpMethod, Route>();
                _actions[path] = innerDictionary;
            }
            innerDictionary[httpMethod] = route;
        }

        public void Add(HttpStatusCode statusCode, Route route)
        {
            _statusCodePages[statusCode] = route;
        }

        public Route Get(HttpMethod httpMethod, string path)
        {
            path = NormalizePath(path);
            IDictionary<HttpMethod, Route> innerDictionary;
            if (!_actions.TryGetValue(path, out innerDictionary))
                return null;
            Route result;
            innerDictionary.TryGetValue(httpMethod, out result);
            return result;
        }

        public Route Get(HttpStatusCode statusCode)
        {
            Route result;
            _statusCodePages.TryGetValue(statusCode, out result);
            return result;
        }

        private string NormalizePath(string path)
        {
            path = path.ToLower();
            if (!path.StartsWith("/"))
                path = "/" + path;
            return path;
        }
    }

    internal class Route
    {
        public MethodInvoker ActionInvoker { get; }
        public ActionInfo ActionInfo { get; }

        public Route(MethodInvoker actionInvoker, ActionInfo actionInfo)
        {
            ActionInvoker = actionInvoker;
            ActionInfo = actionInfo;
        }
    }
}
