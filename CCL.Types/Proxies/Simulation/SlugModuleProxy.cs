using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class SlugModuleProxy : MonoBehaviour
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, true)]
        public string appliedVoltagePortId;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.OHMS, true)]
        public string effectiveResistancePortId;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.AMPS, true)]
        public string totalAmpsPortId;
    }
}
