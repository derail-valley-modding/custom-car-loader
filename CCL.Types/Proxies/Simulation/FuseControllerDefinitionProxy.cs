using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation
{
    public class FuseControllerDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float setThreshold = 0.5f;
        public bool isActiveWhenOverThreshold = true;
        [FuseId]
        public string fuseId;
        public PortReferenceDefinition controllingPort = new PortReferenceDefinition(DVPortValueType.STATE, "CONTROLLING_PORT", false);

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[] { controllingPort };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[] { new FuseIdField(this, nameof(fuseId), fuseId) };
    }
}
