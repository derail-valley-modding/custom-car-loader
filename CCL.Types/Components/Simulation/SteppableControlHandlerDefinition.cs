using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Steppable Control Handler Definition")]
    public class SteppableControlHandlerDefinition : SimComponentDefinitionProxy
    {
        public float Step = 0.1f;
        public float FastStepTime = 0.2f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EXT_IN"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "OTHER_CONTROL_EXT_IN", true)
        };
    }
}
