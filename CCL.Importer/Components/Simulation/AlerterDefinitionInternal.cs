using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class AlerterDefinitionInternal : SimComponentDefinition
    {
        public float MinimumSpeed = 1;
        public float VigilanceTime = 30;
        public float WarningTime = 5;
        public string PowerFuseId = string.Empty;

        public readonly PortDefinition AcknowledgeControl = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "ACKNOWLEDGE_CONTROL");
        public readonly PortDefinition StateReadout = new(PortType.READONLY_OUT, PortValueType.STATE, "STATE");
        public readonly PortDefinition TimeLeftReadout = new(PortType.READONLY_OUT, PortValueType.STATE, "TIME_LEFT");
        public readonly PortDefinition ControllerActive = new(PortType.EXTERNAL_IN, PortValueType.STATE, "ACTIVE");

        public readonly PortReferenceDefinition WheelSpeedReader = new(PortValueType.GENERIC, "WHEEL_SPEED_KMH");

        public override SimComponent InstantiateImplementation()
        {
            return new Alerter(this);
        }
    }
}
