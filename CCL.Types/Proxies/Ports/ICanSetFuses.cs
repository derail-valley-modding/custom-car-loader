using System.Collections.Generic;

namespace CCL.Types.Proxies.Ports
{
    public interface ICanSetFuses
    {
        public IEnumerable<string> SettableFuses { get; }
    }
}
