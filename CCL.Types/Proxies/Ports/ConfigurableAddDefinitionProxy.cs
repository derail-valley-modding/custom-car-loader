using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Configurable Add Definition Proxy")]
    public class ConfigurableAddDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        [Header("Leave as generic to show all options")]
        public PortReferenceDefinition aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "A");
        public PortReferenceDefinition bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "B");
        public PortDefinition addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ADD_OUT");
        
        public bool negativeA;
        public bool negativeB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            addReadOut,
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            aReader,
            bReader,
        };

        [SerializeField, HideInInspector]
        private string? aJson;
        [SerializeField, HideInInspector]
        private string? bJson;
        [SerializeField, HideInInspector]
        private string? outJson;

        public void OnValidate()
        {
            aJson = JSONObject.ToJson(aReader);
            bJson = JSONObject.ToJson(bReader);
            outJson = JSONObject.ToJson(addReadOut);
        }

        public void AfterImport()
        {
            aReader = JSONObject.FromJson(aJson, () => new PortReferenceDefinition(DVPortValueType.GENERIC, "A"));
            bReader = JSONObject.FromJson(bJson, () => new PortReferenceDefinition(DVPortValueType.GENERIC, "B"));
            addReadOut = JSONObject.FromJson(outJson, () => new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ADD_OUT"));
        }
    }
}
