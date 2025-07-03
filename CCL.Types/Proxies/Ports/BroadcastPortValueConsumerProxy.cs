using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Broadcast Port Value Consumer Proxy")]
    public class BroadcastPortValueConsumerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string consumerPortId = string.Empty;

        public DVPortForwardConnectionType connection;
        public string connectionTag = string.Empty;
        public float disconnectedValue;
        public bool propagateConsumerValueChangeBackToProvider;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(consumerPortId), consumerPortId),
        };
    }
}
