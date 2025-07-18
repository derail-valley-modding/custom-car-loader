﻿using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Multiple Port Sum Definition Proxy")]
    public class MultiplePortSumDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized, IRecommendedDebugPorts
    {
        public PortReferenceDefinition[] inputs = new PortReferenceDefinition[0];

        public PortDefinition output = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT");

        [SerializeField, HideInInspector]
        private string? _jsonIn;
        [SerializeField, HideInInspector]
        private string? _jsonOut;

        public override IEnumerable<PortDefinition> ExposedPorts => new[] { output };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => inputs;

        public void OnValidate()
        {
            _jsonIn = JSONObject.ToJson(inputs);
            _jsonOut = JSONObject.ToJson(output);
        }

        public void AfterImport()
        {
            inputs = JSONObject.FromJson(_jsonIn, () => new PortReferenceDefinition[0]);
            output = JSONObject.FromJson(_jsonOut, () => new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT"));
        }

        public IEnumerable<string> GetDebugPorts() => new[]
        {
            output.ID
        };
    }
}
