using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class PowerFunctionDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized, IBE2Defaults
    {
        public float multiplier = 1f;
        public float exponent = 1.2f;

        public PortReferenceDefinition input = new PortReferenceDefinition(DVPortValueType.GENERIC, "IN");
        public PortDefinition output = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT");

        [SerializeField, HideInInspector]
        private string? _inJson;
        [SerializeField, HideInInspector]
        private string? _outJson;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            output
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            input
        };

        public void OnValidate()
        {
            _inJson = JSONObject.ToJson(input);
            _outJson = JSONObject.ToJson(output);
        }

        public void AfterImport()
        {
            input = JSONObject.FromJson(_inJson, () => new PortReferenceDefinition(DVPortValueType.GENERIC, "IN"));
            output = JSONObject.FromJson(_outJson, () => new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT"));
        }

        #region Defaults

        public void ApplyBE2Defaults()
        {
            multiplier = 1.0f;
            exponent = 1.0f;
        }

        #endregion
    }
}
