using System.Collections.Generic;
using System.Linq;

namespace HttpLight
{
    internal class ContentActionParameterSource : IActionParameterSource
    {
        private IHttpRequest _request;

        public ContentActionParameterSource(IHttpRequest request)
        {
            _request = request;
        }

        public string[] GetValues(string name)
        {
            return _request.Content.ContentParameters.GetValues(name);
        }

        public IEnumerable<KeyValuePair<string, string[]>> GetAllValues()
        {
            return _request
                .Content
                .ContentParameters
                .AllKeys
                .Select(x => new KeyValuePair<string, string[]>(x, _request.UrlParameters.GetValues(x)));
        }
    }
}
