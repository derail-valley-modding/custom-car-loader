using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Components.Simulation
{
    public class CombinedThrottleDynamicBrakeDefinition : SimComponentDefinitionProxy
    {
        public bool UseToggleMode = false;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "THROTTLE_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "DYNAMIC_BRAKE_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "COMBINED_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "SELECTOR_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CURRENT_MODE")
        };
    }
}
