using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class ManualLapBrakeSplitterDefinitionInternal : SimComponentDefinition
    {
        public readonly PortDefinition RegularIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "REGULAR_EXT_IN");
        public readonly PortDefinition EmergencyIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "EMERGENCY_EXT_IN");
        public readonly PortDefinition CombinedIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "COMBINED_EXT_IN");

        public override SimComponent InstantiateImplementation()
        {
            return new ManualLapBrakeSplitter(this);
        }
    }
}
