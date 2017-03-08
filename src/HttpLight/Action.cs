using System.Collections.Generic;
using HttpLight.Utils;

namespace HttpLight
{
    internal class Action
    {
        public MethodInvoker Invoker { get; }
        public ICollection<string> Methods { get; }
        public ICollection<HttpStatusCode> StatusCodes { get; }
        public ICollection<string> Paths { get; }
        public bool Before { get; set; }

        public Action(MethodInvoker invoker)
        {
            Invoker = invoker;
            Methods = new HashSet<string>();
            StatusCodes = new HashSet<HttpStatusCode>();
            Paths = new HashSet<string>();
        }
    }
}
