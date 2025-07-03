using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Water Detector Port Feeder Proxy")]
    public class WaterDetectorPortFeederProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string statePortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(statePortId), statePortId),
        };
    }
}
