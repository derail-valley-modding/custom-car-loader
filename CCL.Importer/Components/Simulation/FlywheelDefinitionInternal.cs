using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class FlywheelDefinitionInternal : SimComponentDefinition
    {
        public float RotationalInertia = 100;
        public float ViscousDampingFactor = 50;
        public float MaxRPM;

        public readonly PortDefinition RPM = new(PortType.READONLY_OUT, PortValueType.RPM, "RPM");
        public readonly PortDefinition RPMNormalised = new(PortType.READONLY_OUT, PortValueType.RPM, "RPM_NORMALIZED");
        public readonly PortDefinition RPMAbsolute = new(PortType.READONLY_OUT, PortValueType.RPM, "RPM_ABS");
        public readonly PortDefinition RPMAbsoluteNormalised = new(PortType.READONLY_OUT, PortValueType.RPM, "RPM_ABS_NORMALIZED");
        public readonly PortDefinition TorqueIn = new(PortType.IN, PortValueType.TORQUE, "TORQUE_IN");
        public readonly PortReferenceDefinition LoadTorque = new(PortValueType.TORQUE, "LOAD_TORQUE");

        public override SimComponent InstantiateImplementation()
        {
            return new Flywheel(this);
        }
    }
}
