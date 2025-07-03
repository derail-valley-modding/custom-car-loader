using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Throttle Gamma Power Conversion Definition Proxy")]
    public class ThrottleGammaPowerConversionDefinitionProxy : SimComponentDefinitionProxy, IDE2Defaults, IDE6Defaults
    {
        public int numberOfNotches = 8;
        public float gamma = 1.2f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "GOAL_POWER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "GOAL_RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NOTCH")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "IDLE_RPM_NORMALIZED", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "MAX_POWER_RPM_NORMALIZED", false),
            new PortReferenceDefinition(DVPortValueType.POWER, "MAX_POWER", false)
        };

        public void ApplyDE2Defaults()
        {
            numberOfNotches = 12;
            gamma = 1.2f;
        }

        public void ApplyDE6Defaults()
        {
            numberOfNotches = 11;
            gamma = 1.2f;
        }
    }
}
