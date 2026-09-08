using LocoSim.Definitions;
using LocoSim.Implementations;

using CCL.Importer.Implementations;

namespace CCL.Importer.Components.Simulation.Electric
{
    public class RoofBusBarDefinitionInternal : SimComponentDefinition
    {
        public float nominalVoltage = 1500.0f;
        public int pantographCount = 1;

        public PortReferenceDefinition[]? allInputs;

        public readonly PortDefinition supplyVoltage = new(PortType.READONLY_OUT, PortValueType.VOLTS, "SUPPLY_VOLTAGE");
        public readonly PortDefinition supplyVoltageNormalized = new(PortType.READONLY_OUT, PortValueType.VOLTS, "SUPPLY_VOLTAGE_NORMALIZED");
        public readonly PortDefinition pantographInputCurrent = new(PortType.READONLY_OUT, PortValueType.AMPS, "PANTOGRAPH_INPUT_CURRENT");
        public readonly PortDefinition raisedPantographsCount = new(PortType.READONLY_OUT, PortValueType.GENERIC, "PANTOGRAPHS_RAISED_COUNT");
        
        public override SimComponent InstantiateImplementation() => new RoofBusBar(this);
    }
}
