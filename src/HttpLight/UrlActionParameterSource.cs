using System.Collections.Generic;
using System.Linq;

namespace HttpLight
{
    internal class UrlActionParameterSource : IActionParameterSource
    {
        private IHttpRequest _request;

        public UrlActionParameterSource(IHttpRequest request)
        {
            _request = request;
        }

        public string[] GetValues(string name)
        {
            return _request.UrlParameters.GetValues(name);
        }

        public IEnumerable<KeyValuePair<string, string[]>> GetAllValues()
        {
            return _request
                .UrlParameters
                .AllKeys
                .Select(x => new KeyValuePair<string, string[]>(x, _request.UrlParameters.GetValues(x)));
        }
    }
}
