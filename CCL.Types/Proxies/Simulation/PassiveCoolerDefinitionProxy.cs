using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Passive Cooler Definition Proxy")]
    public class PassiveCoolerDefinitionProxy : SimComponentDefinitionProxy, IDE2Defaults, IDE6Defaults, IDH4Defaults, IDM3Defaults, IDM1UDefaults, IBE2Defaults
    {
        public float coolingRate = 12500f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE"),
        };

        #region Defaults

        public void ApplyDE2Defaults()
        {
            coolingRate = 100.0f;
        }

        public void ApplyDE6Defaults()
        {
            coolingRate = 250.0f;
        }

        public void ApplyDH4Defaults()
        {
            coolingRate = 3750.0f;
        }

        public void ApplyDM3Defaults()
        {
            coolingRate = 4500.0f;
        }

        public void ApplyDM1UDefaults()
        {
            coolingRate = 1000.0f;
        }

        public void ApplyBE2Defaults()
        {
            coolingRate = 5.0f;
        }

        #endregion
    }
}
