using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Manual Lap Brake Splitter Definition")]
    public class ManualLapBrakeSplitterDefinition : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
{
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "REGULAR_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EMERGENCY_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "COMBINED_EXT_IN"),
        };
    }
}
