using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Throttle Custom Power Conversion Definition Proxy")]
    public class ThrottleCustomPowerConversionDefinitionProxy : SimComponentDefinitionProxy
    {
        public float[] notchPowerPercentages = new float[0];

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "GOAL_POWER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NOTCH"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "PREV_NOTCH")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE", false),
            new PortReferenceDefinition(DVPortValueType.POWER, "MAX_POWER", false)
        };
    }
}
