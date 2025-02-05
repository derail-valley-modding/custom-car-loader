using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class SanderDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields,
        IDE2Defaults, IDE6Defaults, IDH4Defaults, IDM3Defaults, IDM1UDefaults, IBE2Defaults, IS060Defaults, IS282Defaults
    {
        public float sandConsumptionRate = 5f;
        [Min(1f)]
        public float sandCoeficientMax = 1.5f;

        [Header("Optional")]
        [FuseId]
        public string powerFuseId;

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

        #region Defaults

        public void ApplyDE2Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 2.25f;
        }

        public void ApplyDE6Defaults()
        {
            sandConsumptionRate = 1.7f;
            sandCoeficientMax = 2.25f;
        }

        public void ApplyDH4Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 1.5f;
        }

        public void ApplyDM3Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 2.25f;
        }

        public void ApplyDM1UDefaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 2.25f;
        }

        public void ApplyBE2Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 2.25f;
        }

        public void ApplyS060Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 1.5f;
        }

        public void ApplyS282Defaults()
        {
            sandConsumptionRate = 0.5f;
            sandCoeficientMax = 1.5f;
        }

        #endregion
    }
}
