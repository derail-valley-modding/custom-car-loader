using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class WaterDetectorPortFeederProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string statePortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(statePortId), statePortId),
        };
    }
}
