using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    [AddComponentMenu("CCL/Proxies/Simulation/Electric/Slugs Power Provider Module Proxy")]
    public class SlugsPowerProviderModuleProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, true)]
        public string generatorVoltagePortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.OHMS, true)]
        public string slugsEffectiveResistancePortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, true)]
        public string slugsTotalAmpsPortId = string.Empty;
        [FuseId(true)]
        public string powerFuseId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(generatorVoltagePortId), generatorVoltagePortId, DVPortType.READONLY_OUT, DVPortValueType.VOLTS),
            new PortIdField(this, nameof(slugsEffectiveResistancePortId), slugsEffectiveResistancePortId, DVPortType.EXTERNAL_IN, DVPortValueType.OHMS),
            new PortIdField(this, nameof(slugsTotalAmpsPortId), slugsTotalAmpsPortId, DVPortType.EXTERNAL_IN, DVPortValueType.AMPS),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId, true)
        };
    }
}
