using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    [AddComponentMenu("CCL/Proxies/Simulation/Electric/Electric Bell Definition Proxy")]
    public class ElectricBellDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float smoothDownTime = 2f;
        [FuseId]
        public string powerFuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[] { new FuseIdField(this, nameof(powerFuseId), powerFuseId) };

        public override IEnumerable<PortDefinition> ExposedPorts => new[] { new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "BELL_NORMALIZED") };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[] { new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL") };
    }
}
