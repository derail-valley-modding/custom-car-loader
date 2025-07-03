using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Smoothed Output Definition Proxy")]
    public class SmoothedOutputDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        [Header("0 is treated as instant change")]
        public float smoothTime = 1f;
        [FuseId]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
{
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUTPUT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId)
        };
    }
}
