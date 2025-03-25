using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class TickingOutputDefinitionInternal : SimComponentDefinition
    {
        public float TickingTime = 1.0f;
        public float AbsoluteValueDifference = -1.0f;
        public string PowerFuseId = string.Empty;

        public readonly PortReferenceDefinition Input = new(PortValueType.GENERIC, "INPUT");
        public readonly PortDefinition OutReadOut = new(PortType.READONLY_OUT, PortValueType.GENERIC, "OUTPUT");
        public readonly PortDefinition TickReadOut = new(PortType.READONLY_OUT, PortValueType.STATE, "TICKING");

        public override SimComponent InstantiateImplementation()
        {
            return new TickingOutput(this);
        }
    }
}
