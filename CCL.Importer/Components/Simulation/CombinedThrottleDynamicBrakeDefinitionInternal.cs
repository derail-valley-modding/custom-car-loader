using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class CombinedThrottleDynamicBrakeDefinitionInternal : SimComponentDefinition
    {
        public bool UseToggleMode = false;

        public readonly PortDefinition ThrottleIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "THROTTLE_EXT_IN");
        public readonly PortDefinition DynBrakeIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "DYNAMIC_BRAKE_EXT_IN");
        public readonly PortDefinition CombinedIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "COMBINED_EXT_IN");
        public readonly PortDefinition SelectorIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "SELECTOR_EXT_IN");
        public readonly PortDefinition CurrentMode = new(PortType.READONLY_OUT, PortValueType.STATE, "CURRENT_MODE");

        public override SimComponent InstantiateImplementation()
        {
            return new CombinedThrottleDynamicBrake(this);
        }
    }
}
