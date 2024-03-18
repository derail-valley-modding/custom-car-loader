using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class BroadcastPortValueConsumerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string consumerPortId;

        public DVPortForwardConnectionType connection;
        public string connectionTag;
        public float disconnectedValue;
        public bool propagateConsumerValueChangeBackToProvider;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(consumerPortId), consumerPortId),
        };
    }
}
