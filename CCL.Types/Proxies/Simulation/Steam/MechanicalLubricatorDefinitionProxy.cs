using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class MechanicalLubricatorDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        public float oilCapacity;
        public float oilLeakageRate;
        public float oilConsumptionPerRev;
        public float refillPerRev;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "LUBRICATION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OIL, "LUBRICATION_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OIL, "LUBRICATION_AUDIO_NORMALIZED"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "MECHANICAL_PT_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "SPECIAL_REQUEST")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL", false),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "MANUAL_FILL_RATE_NORMALIZED", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM", false)
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            oilCapacity = 1.0f;
            oilLeakageRate = 0.001f;
            oilConsumptionPerRev = 0.001f;
            refillPerRev = 0.002f;
        }

        public void ApplyS282Defaults()
        {
            oilCapacity = 1.0f;
            oilLeakageRate = 0.001f;
            oilConsumptionPerRev = 0.001f;
            refillPerRev = 0.002f;
        }

        #endregion
    }
}
