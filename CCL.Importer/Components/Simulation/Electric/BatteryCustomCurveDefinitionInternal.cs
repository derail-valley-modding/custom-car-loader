using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Components.Simulation.Electric
{
    internal class BatteryCustomCurveDefinitionInternal : SimComponentDefinition
    {
        public int numSeriesCells = 36;
        public float internalResistance = 0.005f;
        public float baseConsumptionMultiplier = 4f;
        public AnimationCurve chargeToVoltageCurve = null!;
        public string powerFuseId = string.Empty;

        public readonly PortDefinition voltageReadOut = new(PortType.READONLY_OUT, PortValueType.VOLTS, "VOLTAGE");
        public readonly PortDefinition voltageNormalizedReadOut = new(PortType.READONLY_OUT, PortValueType.VOLTS, "VOLTAGE_NORMALIZED");

        public readonly PortReferenceDefinition chargeNormalized = new(PortValueType.ELECTRIC_CHARGE, "NORMALIZED_CHARGE", false);
        public readonly PortReferenceDefinition chargeConsumption = new(PortValueType.ELECTRIC_CHARGE, "CHARGE_CONSUMPTION", true);
        public readonly PortReferenceDefinition powerReader = new(PortValueType.POWER, "POWER", false);

        public override SimComponent InstantiateImplementation()
        {
            return new BatteryCustomCurve(this);
        }
    }
}
