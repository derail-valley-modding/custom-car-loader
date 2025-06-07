using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class SteamMechanicalStokerDefinitionInternal : SimComponentDefinition
    {
        public float MaxTransferRate = 10f;
        public float MaxSteamConsumption = 1f;
        public float MaxWorkingPressure = 6f;
        public float FireboxCoalConsumptionMultiplier = 1f;
        public float SmoothTime = 5f;

        public readonly PortDefinition SteamConsumptionReadOut = new(PortType.READONLY_OUT, PortValueType.MASS_RATE, "STEAM_CONSUMPTION");
        public readonly PortDefinition StokingNormalizedReadOut = new(PortType.READONLY_OUT, PortValueType.STATE, "STOKING_NORMALIZED");

        public readonly PortReferenceDefinition Control = new(PortValueType.CONTROL, "CONTROL", false);
        public readonly PortReferenceDefinition SteamPressure = new(PortValueType.PRESSURE, "STEAM_PRESSURE", false);
        public readonly PortReferenceDefinition FireboxCoalLevel = new(PortValueType.COAL, "FIREBOX_COAL_LEVEL", false);
        public readonly PortReferenceDefinition FireboxCoalCapacity = new(PortValueType.COAL, "FIREBOX_COAL_CAPACITY", false);
        public readonly PortReferenceDefinition FireboxCoalControl = new(PortValueType.CONTROL, "FIREBOX_COAL_CONTROL", true);
        public readonly PortReferenceDefinition CoalAmount = new(PortValueType.COAL, "COAL_AMOUNT", false);
        public readonly PortReferenceDefinition CoalConsume = new(PortValueType.COAL, "COAL_CONSUMPTION", true);

        public override SimComponent InstantiateImplementation()
        {
            return new SteamMechanicalStoker(this);
        }
    }
}
