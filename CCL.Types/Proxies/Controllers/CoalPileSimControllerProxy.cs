using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class CoalPileSimControllerProxy : MonoBehaviour
    {
        public float coalChunkMass;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.COAL, true)]
        public string coalAvailablePortId = string.Empty;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.COAL, true)]
        public string coalCapacityPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.COAL, true)]
        public string coalConsumePortId = string.Empty;
    }
}
