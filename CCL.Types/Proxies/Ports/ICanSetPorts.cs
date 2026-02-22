using System.Collections.Generic;

namespace CCL.Types.Proxies.Ports
{
    public interface ICanSetPorts
    {
        public IEnumerable<string> SettablePorts { get; }
    }
}
