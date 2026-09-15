using System.Collections.Generic;
using UnityEngine;

using CCL.Types.Proxies.Ports;

namespace CCL.Types.Components.Simulation.Electric
{
    [AddComponentMenu("CCL/Components/Simulation/Electric/Pantograph Definition")]
    public class PantographDefinition : PowerCollectorCommonPortsDefinition, IHasFuseIdFields
    {
        [Min(0.01f), Tooltip("Pantograph head movement speed in m/s")]
        public float headMovementSpeed = 1.0f;

        [Min(0.0f), Tooltip("Maximum reach height above a rail head. The minimum height is taken from initial position. Must match the reach from pantograph animation")]
        public float maximumRaise;

        [FuseId(true)]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new List<PortDefinition>(base.ExposedPorts)
        {
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
