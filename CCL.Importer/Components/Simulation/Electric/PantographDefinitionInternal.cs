using UnityEngine;

using LocoSim.Definitions;
using LocoSim.Implementations;

using CCL.Importer.Implementations;

namespace CCL.Importer.Components.Simulation.Electric
{
    internal class PantographDefinitionInternal : SimComponentDefinition
    {
        public float nominalVoltage, maximumRaise, headMovementSpeed, contactTolerance;

        public string powerFuseId = string.Empty;

        public readonly PortDefinition wireHeight = new(PortType.EXTERNAL_IN, PortValueType.GENERIC, "WIRE_HEIGHT");
        public readonly PortDefinition initialHeadHeight = new(PortType.EXTERNAL_IN, PortValueType.GENERIC, "INITIAL_HEAD_HEIGHT");
        public readonly PortDefinition headHeight = new(PortType.EXTERNAL_IN, PortValueType.GENERIC, "HEAD_HEIGHT");
        public readonly PortDefinition wireVoltage = new(PortType.EXTERNAL_IN, PortValueType.VOLTS, "WIRE_VOLTAGE");
        public readonly PortDefinition supplyVoltage = new(PortType.READONLY_OUT, PortValueType.VOLTS, "VOLTAGE");
        public readonly PortDefinition supplyVoltageNormalized = new(PortType.READONLY_OUT, PortValueType.VOLTS, "VOLTAGE_NORMALIZED");
        public readonly PortDefinition pantographRaise = new(PortType.READONLY_OUT, PortValueType.GENERIC, "PANTOGRAPH_RAISE");
        public readonly PortDefinition pantographRaiseNormalized = new(PortType.READONLY_OUT, PortValueType.STATE, "PANTOGRAPH_RAISE_NORMALIZED");

        public readonly PortReferenceDefinition toggle = new(PortValueType.CONTROL, "TOGGLE", true );

        public override SimComponent InstantiateImplementation() => new Pantograph(this);
    }
}
