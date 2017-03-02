using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HttpLight
{
    internal class ActionInfo
    {
        public MethodInfo MethodInfo { get; }
        public Encoding Encoding { get; set; }
        public ICollection<HttpMethod> HttpMethods { get; set; }
        public ICollection<HttpStatusCode> HttpStatusCodes { get; set; }
        public ICollection<string> Paths { get; set; }

        public ActionInfo(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            HttpMethods = new HashSet<HttpMethod>();
            HttpStatusCodes = new HashSet<HttpStatusCode>();
            Paths = new HashSet<string>();
            Encoding = Encoding.UTF8;
        }
    }
}
