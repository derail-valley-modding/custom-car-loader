using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Clapper Controller Proxy")]
    public class ClapperControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        public Transform clapperTransform = null!;
        [PortId(DVPortValueType.RPM, true)]
        public string engineRpmNormalizedPortId = string.Empty;
        [PortId(DVPortValueType.STATE, true)]
        public string engineOnPortId = string.Empty;
        public float highestMaxAngleAtPercentage = 0.33f;
        public float highestMinAngleAtPercentage = 0.66f;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(engineRpmNormalizedPortId), engineRpmNormalizedPortId, DVPortValueType.RPM),
            new PortIdField(this, nameof(engineOnPortId), engineOnPortId, DVPortValueType.STATE),
        };
    }
}
