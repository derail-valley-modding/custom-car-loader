﻿using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation
{
    public class TractionDefinitionProxy : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.IN, DVPortValueType.TORQUE, "TORQUE_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "FORWARD_SPEED_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.RPM, "WHEEL_RPM_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "WHEEL_SPEED_KMH_EXT_IN"),
        };
    }
}