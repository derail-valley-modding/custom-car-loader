﻿using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Controls
{
    public class ManualTransmissionInputDefinitionProxy : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "GEAR"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "NUM_OF_GEARS"),
        };
    }
}