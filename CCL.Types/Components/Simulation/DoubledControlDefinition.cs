using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Doubled Control Definition")]
    public class DoubledControlDefinition : SimComponentDefinitionProxy
    {
        public float DefaultValue;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_A_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_B_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "COMBINED_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "EFFECT_A_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "EFFECT_B_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "EFFECT")
        };
    }
}
