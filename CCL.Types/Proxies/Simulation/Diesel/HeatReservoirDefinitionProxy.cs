﻿using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class HeatReservoirDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public float heatCapacity = 1f;

        public float overheatingTemperatureThreshold = 120f;

        public PortReferenceDefinition[] inputs;
        [SerializeField]
        private string? _inputsJson;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => inputs;

        public void OnValidate()
        {
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
    }
}
