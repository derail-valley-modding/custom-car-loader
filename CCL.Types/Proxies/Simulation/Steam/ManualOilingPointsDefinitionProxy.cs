using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class ManualOilingPointsDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized, IS060Defaults, IS282Defaults
    {
        [Serializable]
        public class OilingPointDefinition
        {
            public PortDefinition oilLevelReadOut;
            public PortDefinition oilLevelNormalizedReadOut;
            public PortDefinition pointDoorExtIn;
            public PortDefinition refillExtIn;
            public PortDefinition refillingFlowNormalizedReadOut;

            public PortDefinition[] AllPorts => new[]
            {
                oilLevelReadOut,
                oilLevelNormalizedReadOut,
                pointDoorExtIn,
                refillExtIn,
                refillingFlowNormalizedReadOut
            };
        }

        [Header("Per oiling point")]
        public float capacity = 5f;
        public float consumptionPerRev = 0.001f;
        public float pointOpenConsumptionMultiplier = 100f;
        public float refillRate = 2.5f;
        public float damagePerRevWhenEmpty = 1f;

        [Space, Delayed, Min(0)]
        public int OilingPointCount = 0;
        [HideInInspector]
        public OilingPointDefinition[] oilingPoints = new OilingPointDefinition[0];
        [SerializeField, HideInInspector]
        private string? _json;

        public override IEnumerable<PortDefinition> ExposedPorts => s_ports.Concat(oilingPoints.SelectMany(x => x.AllPorts));

        private IEnumerable<PortDefinition> s_ports = new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "MECHANICAL_DAMAGE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "MECHANICAL_PT_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "LOWEST_OIL_LEVEL_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "LOWEST_OIL_LEVEL_AUDIO")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_STORAGE", false),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM", false)
        };

        public void OnValidate()
        {
            oilingPoints = new OilingPointDefinition[OilingPointCount];

            for (int i = 0; i < OilingPointCount; i++)
            {
                oilingPoints[i] = new OilingPointDefinition
                {
                    oilLevelReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OIL, $"OIL_LEVEL_{i}"),
                    oilLevelNormalizedReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OIL, $"OIL_LEVEL_NORMALIZED_{i}"),
                    pointDoorExtIn = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, $"POINT_DOOR_EXT_IN_{i}"),
                    refillExtIn = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, $"REFILL_EXT_IN_{i}"),
                    refillingFlowNormalizedReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, $"REFILLING_FLOW_NORMALIZED_{i}")
                };
            }

            _json = JSONObject.ToJson(oilingPoints);
        }

        public void AfterImport()
        {
            oilingPoints = JSONObject.FromJson(_json, () => new OilingPointDefinition[0]);
        }

        #region Defaults

        public void ApplyS060Defaults()
        {
            capacity = 0.5f;
            consumptionPerRev = 2e-05f;
            pointOpenConsumptionMultiplier = 1000f;
            refillRate = 0.2f;
            damagePerRevWhenEmpty = 1f;

            OilingPointCount = 6;
            OnValidate();
        }

        public void ApplyS282Defaults()
        {
            capacity = 0.5f;
            consumptionPerRev = 2e-05f;
            pointOpenConsumptionMultiplier = 1000f;
            refillRate = 0.2f;
            damagePerRevWhenEmpty = 1f;

            OilingPointCount = 6;
            OnValidate();
        }

        #endregion
    }
}
