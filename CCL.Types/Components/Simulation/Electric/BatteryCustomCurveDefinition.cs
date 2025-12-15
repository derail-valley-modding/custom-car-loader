using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation.Electric
{
    [AddComponentMenu("CCL/Components/Simulation/Electric/Battery Custom Curve Definition")]
    public class BatteryCustomCurveDefinition : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public int numSeriesCells = 36;
        public float internalResistance = 0.005f;
        public float baseConsumptionMultiplier = 4f;
        public AnimationCurve chargeToVoltageCurve = null!;

        [FuseId(true)]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "VOLTAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "VOLTAGE_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.ELECTRIC_CHARGE, "NORMALIZED_CHARGE", false),
            new PortReferenceDefinition(DVPortValueType.ELECTRIC_CHARGE, "CHARGE_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.POWER, "POWER", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };
    }
}
