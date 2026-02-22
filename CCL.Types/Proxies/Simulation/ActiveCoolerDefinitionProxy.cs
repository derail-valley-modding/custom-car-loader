using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Active Cooler Definition Proxy")]
    public class ActiveCoolerDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float coolingRate = 12500f;
        public float easeTime = 2f;

        [FuseId(true)]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "COOLING_EFFECT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId, true)
        };
    }
}
