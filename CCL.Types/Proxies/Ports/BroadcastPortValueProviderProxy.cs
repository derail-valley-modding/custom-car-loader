using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Broadcast Port Value Provider Proxy")]
    public class BroadcastPortValueProviderProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string providerPortId = string.Empty;
        public DVPortForwardConnectionType connection;
        public string connectionTag = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
{
            new PortIdField(this, nameof(providerPortId), providerPortId),
        };
    }
}
