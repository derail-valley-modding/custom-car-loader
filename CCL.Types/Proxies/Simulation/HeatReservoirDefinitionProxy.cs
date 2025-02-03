using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class HeatReservoirDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized,
        IDE2Defaults, IDE6Defaults, IDH4Defaults, IDM3Defaults, IDM1UDefaults, IBE2Defaults
    {
        public float heatCapacity = 1f;
        public float overheatingTemperatureThreshold = 120f;
        public float maxTemperature = 300f;

        [Delayed]
        public int inputCount = 1;
        [HideInInspector]
        public PortReferenceDefinition[] inputs;
        [SerializeField, HideInInspector]
        private string? _inputsJson;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => inputs ?? base.ExposedPortReferences;

        public void OnValidate()
        {
            if (inputs == null || inputs.Length != inputCount)
            {
                inputs = Enumerable.Range(0, inputCount)
                    .Select(i => new PortReferenceDefinition(DVPortValueType.HEAT_RATE, $"HEAT_IN_{i}"))
                    .ToArray();
            }
            _inputsJson = JSONObject.ToJson(inputs);
        }

        public void AfterImport()
        {
            if (!string.IsNullOrWhiteSpace(_inputsJson))
            {
                inputs = JSONObject.FromJson<PortReferenceDefinition[]>(_inputsJson);
            }
            else
            {
                inputs = new PortReferenceDefinition[0];
            }
        }

        #region Defaults

        public void ApplyDE2Defaults()
        {
            heatCapacity = 1000.0f;
            overheatingTemperatureThreshold = 120.0f;
            maxTemperature = 300f;
        }

        public void ApplyDE6Defaults()
        {
            heatCapacity = 15000.0f;
            overheatingTemperatureThreshold = 120.0f;
            maxTemperature = 300f;
        }

        public void ApplyDH4Defaults()
        {
            heatCapacity = 500000.0f;
            overheatingTemperatureThreshold = 120.0f;
            maxTemperature = 300f;
        }

        public void ApplyDM3Defaults()
        {
            heatCapacity = 100000.0f;
            overheatingTemperatureThreshold = 120.0f;
            maxTemperature = 300f;
        }

        public void ApplyDM1UDefaults()
        {
            heatCapacity = 250000.0f;
            overheatingTemperatureThreshold = 120.0f;
            maxTemperature = 300f;
        }

        public void ApplyBE2Defaults()
        {
            heatCapacity = 500.0f;
            overheatingTemperatureThreshold = 120.0f;
            maxTemperature = 300f;
        }

        #endregion
    }
}
