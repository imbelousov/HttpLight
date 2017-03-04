using System.Collections;
using System.Collections.Generic;

namespace HttpLight
{
    public sealed class HostCollection : IEnumerable<HostCollectionEntry>
    {
        private ICollection<HostCollectionEntry> _hosts;

        internal HostCollection()
        {
            _hosts = new HashSet<HostCollectionEntry>();
        }

        public void Add(HostCollectionEntry host)
        {
            _hosts.Add(host);
        }

        public void Add(string protocol, string domain, int port)
        {
            _hosts.Add(new HostCollectionEntry(protocol, domain, port));
        }

        public void Add(string domain, int port)
        {
            _hosts.Add(new HostCollectionEntry(domain, port));
        }

        public void Add(string domain)
        {
            _hosts.Add(new HostCollectionEntry(domain));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<HostCollectionEntry> GetEnumerator()
        {
            return _hosts.GetEnumerator();
        }
    }

    public sealed class HostCollectionEntry
    {
        private const string DefaultProtocol = "http";
        private const int DefaultPort = 80;

        public string Protocol { get; }
        public string Domain { get; }
        public int Port { get; }

        public HostCollectionEntry(string protocol, string domain, int port)
        {
            Protocol = protocol;
            Domain = domain;
            Port = port;
        }

        public HostCollectionEntry(string domain, int port)
            : this(DefaultProtocol, domain, port)
        {
        }

        public HostCollectionEntry(string domain)
            : this(DefaultProtocol, domain, DefaultPort)
        {
        }

        public override bool Equals(object obj)
        {
            var host = obj as HostCollectionEntry;
            if (host == null)
                return false;
            return Protocol == host.Protocol && Domain == host.Domain && Port == host.Port;
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
