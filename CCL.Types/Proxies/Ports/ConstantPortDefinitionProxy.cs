using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConstantPortDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public float value;
        public PortDefinition port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, string.Empty);

        [SerializeField, HideInInspector]
        private string? _json;

        public override IEnumerable<PortDefinition> ExposedPorts => new[] { port };

        public void AfterImport()
        {
            port = JSONObject.FromJson(_json, () => port);
        }

        public void OnValidate()
        {
            _json = JSONObject.ToJson(port);
        }
    }
}