using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class SlugsPowerProviderModuleProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, true)]
        public string generatorVoltagePortId;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.OHMS, true)]
        public string slugsEffectiveResistancePortId;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, true)]
        public string slugsTotalAmpsPortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(generatorVoltagePortId), generatorVoltagePortId, DVPortType.READONLY_OUT, DVPortValueType.VOLTS),
            new PortIdField(this, nameof(slugsEffectiveResistancePortId), slugsEffectiveResistancePortId, DVPortType.EXTERNAL_IN, DVPortValueType.OHMS),
            new PortIdField(this, nameof(slugsTotalAmpsPortId), slugsTotalAmpsPortId, DVPortType.EXTERNAL_IN, DVPortValueType.AMPS),
        };
    }
}
