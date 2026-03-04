using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Components.Simulation
{
    public class RPMDamageCalculatorDefinition : SimComponentDefinitionProxy
    {
        public float MaxRPM;
        public float DamagePerSecond = 0.005f;
        public float OverspeedDamagePerSecond = 0.005f;
        public float ScalingFactor = 50.0f;

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.RPM, "RPM", false)
        };

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "OVERSPEED_SOUND")
        };
    }
}
