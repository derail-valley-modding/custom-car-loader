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
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OIL, "LUBRICATION_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL", false),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "LUBRICATOR_CONTROL", false),
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
