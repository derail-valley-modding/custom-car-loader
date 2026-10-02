using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Components.Simulation
{
    public class AlerterDefinition : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float MinimumSpeed = 1;
        public float VigilanceTime = 30;
        public float WarningTime = 5;
        [FuseId]
        public string PowerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ACKNOWLEDGE_CONTROL"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "TIME_LEFT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "ACTIVE")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "WHEEL_SPEED_KMH")
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(PowerFuseId), PowerFuseId)
        };
    }
}
