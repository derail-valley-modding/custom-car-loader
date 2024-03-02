using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation
{
    public class TransmissionFixedGearDefinitionProxy : SimComponentDefinitionProxy, IDE2Defaults, IDE6Defaults, IBE2Defaults
    {
        public float gearRatio = 5.18f;
        public float transmissionEfficiency = 1f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.IN, DVPortValueType.TORQUE, "TORQUE_IN"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "GEAR_RATIO"),
        };

        #region Defaults

        public void ApplyDE2Defaults()
        {
            gearRatio = 4.0f;
            transmissionEfficiency = 1.0f;
        }

        public void ApplyDE6Defaults()
        {
            gearRatio = 4.133333f;
            transmissionEfficiency = 1.0f;
        }

        public void ApplyBE2Defaults()
        {
            gearRatio = 4.133333f;
            transmissionEfficiency = 1.0f;
        }

        #endregion
    }
}