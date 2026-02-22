using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Combined Control Definition")]
    public class CombinedControlDefinition : SimComponentDefinitionProxy
    {
        public float NeutralValue = 0.5f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_A_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_B_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "COMBINED_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CURRENT_MODE")
        };
    }
}
