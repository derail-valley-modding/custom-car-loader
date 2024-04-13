using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class ElectricBellDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float smoothDownTime = 2f;
        [FuseId]
        public string powerFuseId;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[] { new FuseIdField(this, nameof(powerFuseId), powerFuseId) };

        public override IEnumerable<PortDefinition> ExposedPorts => new[] { new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "BELL_NORMALIZED") };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[] { new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL") };
    }
}
