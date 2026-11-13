using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class RegenerativeBrakingDefinitionInternal : SimComponentDefinition
    {
        public float maximumRegenPower = 200000f;
        public float maxChargeBatteryLevel = 0.95f;

        public string powerFuseId = string.Empty;

        public readonly PortReferenceDefinition tmPowerInRead =
            new(PortValueType.POWER, "TM_POWER_IN_READ");
        public readonly PortReferenceDefinition tmPowerOutRead =
            new(PortValueType.POWER, "TM_POWER_OUT_READ");
        public readonly PortReferenceDefinition dynamicBrkControl =
            new(PortValueType.CONTROL, "DYNAMIC_BRAKE_CONTROL");
        public readonly PortReferenceDefinition batteryChargeNorm =
            new(PortValueType.ELECTRIC_CHARGE, "BATTERY_CHARGE_NORMALIZED");

        public readonly PortDefinition powerOut =
            new(PortType.READONLY_OUT, PortValueType.POWER, "POWER_OUT");
        public readonly PortDefinition isActive =
            new(PortType.READONLY_OUT, PortValueType.STATE, "REGEN_BRAKE_ACTIVE");

        public override SimComponent InstantiateImplementation()
        {
            return new RegenerativeBraking(this);
        }
    }
}
