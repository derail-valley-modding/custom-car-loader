using LocoSim.Definitions;
using LocoSim.Implementations;

using CCL.Importer.Implementations;

namespace CCL.Importer.Components.Simulation.Electric
{
    internal class PantographDefinitionInternal : PowerCollectorCommonPortsDefinitionInternal
    {
        public float maximumRaise;
        public float headMovementSpeed;

        public string powerFuseId = string.Empty;

        public readonly PortDefinition pantographRaise = new(PortType.READONLY_OUT, PortValueType.GENERIC, "PANTOGRAPH_RAISE");
        public readonly PortDefinition pantographRaiseNormalized = new(PortType.READONLY_OUT, PortValueType.GENERIC, "PANTOGRAPH_RAISE_NORMALIZED");

        public readonly PortReferenceDefinition toggle = new(PortValueType.CONTROL, "TOGGLE");

        public override SimComponent InstantiateImplementation() => new Pantograph(this);
    }
}
