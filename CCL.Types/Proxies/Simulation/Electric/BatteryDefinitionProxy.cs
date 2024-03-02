using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class BatteryDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields, IBE2Defaults
    {
        public readonly BatteryChemistry chemistry = BatteryChemistry.LeadAcid;

        public int numSeriesCells = 36;
        public float internalResistance = 0.005f;
        public float baseConsumptionMultiplier = 4f;

        [FuseId]
        public string powerFuseId;

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

        #region Defaults

        public void ApplyBE2Defaults()
        {
            numSeriesCells = 56;
            internalResistance = 0.005f;
            baseConsumptionMultiplier = 2f;
        }

        #endregion
    }

    public enum BatteryChemistry
    {
        Unknown,
        LeadAcid
    }
}
