using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Components.Simulation
{
    public class TickingOutputDefinition : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float TickingTime = 1.0f;
        public float AbsoluteValueDifference = -1.0f;
        [FuseId]
        public string PowerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUTPUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "TICKING")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "INPUT", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(PowerFuseId), PowerFuseId)
        };
    }
}
