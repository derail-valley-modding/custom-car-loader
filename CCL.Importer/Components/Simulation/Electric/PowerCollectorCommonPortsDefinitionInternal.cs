using LocoSim.Definitions;

namespace CCL.Importer.Components.Simulation.Electric
{
    internal abstract class PowerCollectorCommonPortsDefinitionInternal : SimComponentDefinition
    {
        public readonly PortDefinition wireHeight = new(PortType.EXTERNAL_IN, PortValueType.GENERIC, "WIRE_HEIGHT");
        public readonly PortDefinition initialHeadHeight = new(PortType.EXTERNAL_IN, PortValueType.GENERIC, "INITIAL_HEAD_HEIGHT");
        public readonly PortDefinition headHeight = new(PortType.EXTERNAL_IN, PortValueType.GENERIC, "HEAD_HEIGHT");
        public readonly PortDefinition wireVoltage = new(PortType.EXTERNAL_IN, PortValueType.VOLTS, "WIRE_VOLTAGE");
        public readonly PortDefinition pantographInContact = new(PortType.EXTERNAL_IN, PortValueType.STATE, "PANTOGRAPH_IN_CONTACT");
        public readonly PortDefinition supplyVoltage = new(PortType.READONLY_OUT, PortValueType.VOLTS, "VOLTAGE");
    }
}
