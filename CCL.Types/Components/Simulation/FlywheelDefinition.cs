using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Flywheel Definition")]
    public class FlywheelDefinition : SimComponentDefinitionProxy
    {
        public float RotationalInertia = 100;
        public float ViscousDampingFactor = 50;
        public float MaxRPM;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_ABS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_ABS_NORMALIZED"),
            new PortDefinition(DVPortType.IN, DVPortValueType.TORQUE, "TORQUE_IN")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE")
        };
    }
}
