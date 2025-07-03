using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    [AddComponentMenu("CCL/Proxies/Simulation/Electric/Voltage Regulator Definition Proxy")]
    public class VoltageRegulatorDefinitionProxy : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "OUTPUT_VOLTAGE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "EXTERNAL_CURRENT_LIMIT_ACTIVE")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE", false),
            new PortReferenceDefinition(DVPortValueType.VOLTS, "SUPPLY_VOLTAGE", false),
            new PortReferenceDefinition(DVPortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE", false)
        };
    }
}
