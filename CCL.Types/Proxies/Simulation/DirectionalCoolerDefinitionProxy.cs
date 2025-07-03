using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Directional Cooler Definition Proxy")]
    public class DirectionalCoolerDefinitionProxy : SimComponentDefinitionProxy, IDM3Defaults
    {
        public float coolingRate = 12500f;
        public float minCoolingSpeed = 2f;
        public float maxCoolingSpeed = 25f;
        public bool coolingInForwardDirection = true;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "SPEED"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE"),
        };

        #region Defaults

        public void ApplyDM3Defaults()
        {
            coolingRate = 2000.0f;
            minCoolingSpeed = 1.0f;
            maxCoolingSpeed = 20.0f;
            coolingInForwardDirection = true;
        }

        #endregion
    }
}