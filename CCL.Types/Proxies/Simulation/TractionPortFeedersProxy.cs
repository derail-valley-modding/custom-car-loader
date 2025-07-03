using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Traction Port Feeders Proxy")]
    public class TractionPortFeedersProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string forwardSpeedPortId = string.Empty;
        [PortId(null, null, true)]
        public string wheelRpmPortId = string.Empty;
        [PortId(null, null, true)]
        public string wheelSpeedKmhPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(forwardSpeedPortId), forwardSpeedPortId),
            new PortIdField(this, nameof(wheelRpmPortId), wheelRpmPortId),
            new PortIdField(this, nameof(wheelSpeedKmhPortId), wheelSpeedKmhPortId),
        };
    }
}
