using System.Collections.Generic;
using UnityEngine;

using CCL.Types.Proxies.Ports;

namespace CCL.Types.Components.Simulation.Electric
{
    [AddComponentMenu("CCL/Components/Simulation/Electric/Pantograph Definition")]
    public class PantographDefinition : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        [Min(0.01f), Tooltip("Pantograph head movement speed in m/s")]
        public float headMovementSpeed = 1.0f;

        [Min(0.0f), Tooltip("Maximum reach height. The minimum height is taken from initial position. Must match the reach from pantograph animation")]
        public float maximumRaise;

        [FuseId(true)]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "WIRE_HEIGHT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "INITIAL_HEAD_HEIGHT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "HEAD_HEIGHT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, "WIRE_VOLTAGE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "PANTOGRAPH_IN_CONTACT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "VOLTAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "PANTOGRAPH_RAISE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "PANTOGRAPH_RAISE_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "TOGGLE")
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId, true)
        };
    }
}
