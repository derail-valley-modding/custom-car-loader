using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class TimeReaderDefinitionInternal : SimComponentDefinition
    {
        public readonly PortDefinition HoursReadOut = new(PortType.READONLY_OUT, PortValueType.GENERIC, "HOURS");
        public readonly PortDefinition MinutesReadOut = new(PortType.READONLY_OUT, PortValueType.GENERIC, "MINUTES");
        public readonly PortDefinition NormalizedReadOut = new(PortType.READONLY_OUT, PortValueType.GENERIC, "NORMALIZED");

        public override SimComponent InstantiateImplementation()
        {
            return new TimeReader(this);
        }
    }
}
