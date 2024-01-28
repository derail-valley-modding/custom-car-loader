using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class TractionPortFeedersProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string forwardSpeedPortId;

        [PortId(null, null, true)]
        public string wheelRpmPortId;

        [PortId(null, null, true)]
        public string wheelSpeedKmhPortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(forwardSpeedPortId), forwardSpeedPortId),
            new PortIdField(this, nameof(wheelRpmPortId), wheelRpmPortId),
            new PortIdField(this, nameof(wheelSpeedKmhPortId), wheelSpeedKmhPortId),
        };
    }
}
