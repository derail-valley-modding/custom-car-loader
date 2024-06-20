using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class ClapperControllerProxy : MonoBehaviour
    {
        public Transform clapperTransform;
        [PortId(DVPortValueType.RPM, true)]
        public string engineRpmNormalizedPortId;
        [PortId(DVPortValueType.STATE, true)]
        public string engineOnPortId;
        public float highestMaxAngleAtPercentage = 0.33f;
        public float highestMinAngleAtPercentage = 0.66f;
    }
}
