using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class SanderDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
    {
        public float sandConsumptionRate = 5f;

        [Min(1f)]
        public float sandCoeficientMax = 1.5f;

        [Header("Optional")]
        [FuseId]
        public string powerFuseId;

        [MethodButton(nameof(ApplyDM3Defaults), "Apply DM3 Defaults")]
        [RenderMethodButtons]
        public bool renderButtons;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "SAND_COEF"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.SAND, "SAND_FLOW"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.SAND, "SAND"),
            new PortReferenceDefinition(DVPortValueType.SAND, "SAND_CONSUMPTION", true),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };

        public void ApplyDM3Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 2.25f;
        }
    }
}
