using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    public class FuseInverterDefinition : SimComponentDefinitionProxy, ICustomSerialized
    {
        [Serializable]
        public class FusePair
        {
            [FuseId, Tooltip("The original fuse")]
            public string SourceFuseId = string.Empty;
            [FuseId, Tooltip("The fuse that will hold the inverted value")]
            public string InvertedFuseId = string.Empty;
        }

        public FusePair[] FusesToInvert = new FusePair[0];

        [HideInInspector, SerializeField]
        private string? _json;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(FusesToInvert);
        }

        public void AfterImport()
        {
            FusesToInvert = JSONObject.FromJson(_json, () => new FusePair[0]);
        }
    }
}
