using System.Collections;
using System.Collections.Generic;

namespace HttpLight
{
    public sealed class HostCollection : IEnumerable<HostEntry>
    {
        private ICollection<HostEntry> _hosts;

        internal HostCollection()
        {
            _hosts = new HashSet<HostEntry>();
        }

        public void Add(HostEntry host)
        {
            _hosts.Add(host);
        }

        public void Add(string protocol, string domain, int port)
        {
            _hosts.Add(new HostEntry(protocol, domain, port));
        }

        public void Add(string domain, int port)
        {
            _hosts.Add(new HostEntry(domain, port));
        }

        public void Add(string domain)
        {
            _hosts.Add(new HostEntry(domain));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<HostEntry> GetEnumerator()
        {
            return _hosts.GetEnumerator();
        }
    }

    public sealed class HostEntry
    {
        private const string DefaultProtocol = "http";
        private const int DefaultPort = 80;

        public string Protocol { get; }
        public string Domain { get; }
        public int Port { get; }

        public HostEntry(string protocol, string domain, int port)
        {
            Protocol = protocol;
            Domain = domain;
            Port = port;
        }

        public HostEntry(string domain, int port)
            : this(DefaultProtocol, domain, port)
        {
        }

        public HostEntry(string domain)
            : this(DefaultProtocol, domain, DefaultPort)
        {
        }

        public override bool Equals(object obj)
        {
            var hostEntry = obj as HostEntry;
            if (hostEntry == null)
                return false;
            return Protocol == hostEntry.Protocol && Domain == hostEntry.Domain && Port == hostEntry.Port;
        }

        public override int GetHashCode()
        {
            return (Protocol ?? string.Empty).GetHashCode() ^ (Domain ?? string.Empty).GetHashCode() ^ Port.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}://{1}:{2}/", Protocol, Domain, Port);
        }
    }
}
