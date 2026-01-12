using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class ConstantMultiplierOffsetDefinitionInternal : SimComponentDefinition
    {
        public float Multiplier = 1;
        public float Offset = 0;
        public PortReferenceDefinition Input = new(PortValueType.GENERIC, "IN");
        public PortDefinition Output = new(PortType.READONLY_OUT, PortValueType.GENERIC, "OUT");

        public override SimComponent InstantiateImplementation()
        {
            return new ConstantMultiplierOffset(this);
        }
    }
}
