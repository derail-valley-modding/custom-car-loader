using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class ClapperControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        public Transform clapperTransform;
        [PortId(DVPortValueType.RPM, true)]
        public string engineRpmNormalizedPortId;
        [PortId(DVPortValueType.STATE, true)]
        public string engineOnPortId;
        public float highestMaxAngleAtPercentage = 0.33f;
        public float highestMinAngleAtPercentage = 0.66f;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(engineRpmNormalizedPortId), engineRpmNormalizedPortId, DVPortValueType.RPM),
            new PortIdField(this, nameof(engineOnPortId), engineOnPortId, DVPortValueType.STATE),
        };
    }
}
