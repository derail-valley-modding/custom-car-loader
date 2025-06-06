using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    public class SteamMechanicalStokerDefinition : SimComponentDefinitionProxy, IRecommendedDebugPorts
    {
        [Min(0.0f)]
        public float MaxTransferRate = 10f;
        [Min(0.0f)]
        public float MaxSteamConsumption = 1f;
        [Min(2.0f)]
        public float MaxWorkingPressure = 6f;
        [Min(0.0f)]
        public float SmoothTime = 5f;
        [Min(0.0f), Tooltip("This MUST match the multiplier in the firebox controller")]
        public float FireboxCoalConsumptionMultiplier = 1f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "STEAM_CONSUMPTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "STOKING_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
{
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "STEAM_PRESSURE", false),
            new PortReferenceDefinition(DVPortValueType.COAL, "FIREBOX_COAL_LEVEL", false),
            new PortReferenceDefinition(DVPortValueType.COAL, "FIREBOX_COAL_CAPACITY", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "FIREBOX_COAL_CONTROL", true),
            new PortReferenceDefinition(DVPortValueType.COAL, "COAL_AMOUNT", false),
            new PortReferenceDefinition(DVPortValueType.COAL, "COAL_CONSUMPTION", true)
        };

        public IEnumerable<string> GetDebugPorts() => new[]
        {
            "STOKING_NORMALIZED"
        };
    }
}
