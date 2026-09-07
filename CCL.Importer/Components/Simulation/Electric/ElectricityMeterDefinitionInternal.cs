using CCL.Importer.Implementations;

using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation.Electric
{
    public class ElectricityMeterDefinitionInternal : SimComponentDefinition
    {
        public float electricChargeConsumptionFactor;

        public readonly PortDefinition electricChargeConsumed = new(PortType.READONLY_OUT, PortValueType.ELECTRIC_CHARGE, "ENERGY_CONSUMED");

        public readonly PortReferenceDefinition supplyVoltage = new(PortValueType.VOLTS, "SUPPLY_VOLTAGE", false);
        public readonly PortReferenceDefinition currentDraw = new(PortValueType.AMPS,"CURRENT_DRAW", false);

        public override SimComponent InstantiateImplementation() => new ElectricityMeter(this);
    }
}
