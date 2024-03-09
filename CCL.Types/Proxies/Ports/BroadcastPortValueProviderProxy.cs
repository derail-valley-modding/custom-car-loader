using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class BroadcastPortValueProviderProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string providerPortId;

        public DVPortForwardConnectionType connection;
        public string connectionTag;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
{
            new PortIdField(this, nameof(providerPortId), providerPortId),
        };
    }
}
