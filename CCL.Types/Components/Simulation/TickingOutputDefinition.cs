using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    public class TickingOutputDefinition : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        [Header("0 is treated as instant change"), Min(0)]
        public float TickingTime = 1.0f;
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
