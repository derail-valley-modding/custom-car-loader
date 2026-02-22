using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class CombinedControlDefinitionInternal : SimComponentDefinition
    {
        public float NeutralValue = 0.5f;

        public readonly PortDefinition ControlAIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "CONTROL_A_EXT_IN");
        public readonly PortDefinition ControlBIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "CONTROL_B_EXT_IN");
        public readonly PortDefinition CombinedIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "COMBINED_EXT_IN");
        public readonly PortDefinition CurrentMode = new(PortType.READONLY_OUT, PortValueType.STATE, "CURRENT_MODE");

        public override SimComponent InstantiateImplementation()
        {
            return new CombinedControl(this);
        }
    }
}
