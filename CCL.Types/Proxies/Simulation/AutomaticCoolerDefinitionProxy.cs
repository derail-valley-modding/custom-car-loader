using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation
{
    public class AutomaticCoolerDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields, IDH4Defaults
    {
        public float coolingRate = 12500f;
        public float activationTemperature = 100f;
        public float deactivationTemperature = 90f;
        public float easeTime = 2f;

        [FuseId]
        public string powerFuseId;


        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "COOLING_EFFECT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.STATE, "IS_POWERED"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE")
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[] { new FuseIdField(this, nameof(powerFuseId), powerFuseId) };

        #region Defaults

        public void ApplyDH4Defaults()
        {
            coolingRate = 8000.0f;
            activationTemperature = 85.0f;
            deactivationTemperature = 75.0f;
            easeTime = 0.8f;
        }

        #endregion
    }
}
