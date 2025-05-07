using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableFunctionDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public enum FunctionType
        {
            MAX,
            MIN,
            SUM
        }

        public FunctionType type;
        public PortReferenceDefinition[] readers = new PortReferenceDefinition[0];
        public PortDefinition outReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT");

        [SerializeField, HideInInspector]
        private string? _readers;
        [SerializeField, HideInInspector]
        private string? _out;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            outReadOut,
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => readers;

        public void OnValidate()
        {
            _readers = JSONObject.ToJson(readers);
            _out = JSONObject.ToJson(outReadOut);
        }

        public void AfterImport()
        {
            readers = JSONObject.FromJson(_readers, () => new PortReferenceDefinition[0]);
            outReadOut = JSONObject.FromJson(_out, () => new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT"));
        }
    }
}
