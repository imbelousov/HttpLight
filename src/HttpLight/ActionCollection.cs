using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HttpLight
{
    internal class ActionCollection : IEnumerable<ActionCollectionEntry>
    {
        private IDictionary<string, IDictionary<HttpMethod, Action>> _usualActions;
        private IDictionary<HttpStatusCode, Action> _statusCodeActions;
        private IList<Action> _beforeActions;

        public ActionCollection()
        {
            _usualActions = new Dictionary<string, IDictionary<HttpMethod, Action>>();
            _statusCodeActions = new Dictionary<HttpStatusCode, Action>();
            _beforeActions = new List<Action>();
        }

        public void Add(HttpMethod method, string path, Action action)
        {
            path = NormalizePath(path);
            IDictionary<HttpMethod, Action> innerDictionary;
            if (!_usualActions.TryGetValue(path, out innerDictionary))
            {
                innerDictionary = new Dictionary<HttpMethod, Action>();
                _usualActions[path] = innerDictionary;
            }
            innerDictionary[method] = action;
        }

        public void Add(HttpStatusCode statusCode, Action action)
        {
            _statusCodeActions[statusCode] = action;
        }

        public void AddBefore(Action action)
        {
            _beforeActions.Add(action);
        }

        public Action Get(HttpMethod method, string path, out bool methodNotAllowed)
        {
            path = NormalizePath(path);
            IDictionary<HttpMethod, Action> innerDictionary;
            if (!_usualActions.TryGetValue(path, out innerDictionary))
            {
                methodNotAllowed = false;
                return null;
            }
            Action result;
            methodNotAllowed = !innerDictionary.TryGetValue(method, out result);
            return result;
        }

        public Action Get(HttpStatusCode statusCode)
        {
            Action result;
            _statusCodeActions.TryGetValue(statusCode, out result);
            return result;
        }

        public IEnumerable<Action> GetBefore()
        {
            return _beforeActions;
        }

        public void Clear()
        {
            _usualActions.Clear();
            _statusCodeActions.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ActionCollectionEntry> GetEnumerator()
        {
            var usualActions = _usualActions.SelectMany(x => x.Value.Select(y => (ActionCollectionEntry) new UsualActionCollectionEntry
            {
                Path = x.Key,
                Method = y.Key,
                Action = y.Value
            }));
            var statusCodeActions = _statusCodeActions.Select(x => (ActionCollectionEntry) new StatusCodeActionCollectionEntry()
            {
                StatusCode = x.Key,
                Action = x.Value
            });
            return usualActions.Concat(statusCodeActions).GetEnumerator();
        }

        private string NormalizePath(string path)
        {
            path = path.ToLower();
            if (!path.StartsWith("/"))
                path = "/" + path;
            return path;
        }
    }

    internal class ActionCollectionEntry
    {
        public Action Action { get; set; }
    }

    internal class UsualActionCollectionEntry : ActionCollectionEntry
    {
        public string Path { get; set; }
        public HttpMethod Method { get; set; }
    }

    internal class StatusCodeActionCollectionEntry : ActionCollectionEntry
    {
        public HttpStatusCode StatusCode { get; set; }
    }
}
