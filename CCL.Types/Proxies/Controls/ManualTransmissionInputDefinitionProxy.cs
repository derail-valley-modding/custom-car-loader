using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Manual Transmission Input Definition Proxy")]
    public class ManualTransmissionInputDefinitionProxy : SimComponentDefinitionProxy
    {
        public bool gear0IsNeutral;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "REVERSER_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "GEAR"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "REVERSER"),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "NUM_OF_GEARS")
        };
    }
}