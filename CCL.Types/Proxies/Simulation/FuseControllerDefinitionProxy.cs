using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Fuse Controller Definition Proxy")]
    public class FuseControllerDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields, ICustomSerialized
    {
        public float setThreshold = 0.5f;
        public bool isActiveWhenOverThreshold = true;
        [FuseId]
        public string fuseId = string.Empty;
        public PortReferenceDefinition controllingPort = new PortReferenceDefinition(DVPortValueType.STATE, "CONTROLLING_PORT", false);

        [SerializeField, HideInInspector]
        private string? _json;

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            controllingPort
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId)
        };

        public void OnValidate()
        {
            _json = JSONObject.ToJson(controllingPort);
        }

        public void AfterImport()
        {
            controllingPort = JSONObject.FromJson(_json, () => new PortReferenceDefinition(DVPortValueType.STATE, "CONTROLLING_PORT", false));
        }
    }
}
