using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class SlugsPowerProviderModuleProxy : MonoBehaviour
    {
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, true)]
        public string generatorVoltagePortId;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.OHMS, true)]
        public string slugsEffectiveResistancePortId;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, true)]
        public string slugsTotalAmpsPortId;
    }
}
