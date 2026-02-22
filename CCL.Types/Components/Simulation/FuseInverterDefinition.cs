using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Fuse Inverter Definition")]
    public class FuseInverterDefinition : SimComponentDefinitionProxy, ICanSetFuses, ICustomSerialized
    {
        [Serializable]
        public class FusePair
        {
            [FuseId(true), Tooltip("The original fuse")]
            public string SourceFuseId = string.Empty;
            [FuseId(true), Tooltip("The fuse that will hold the inverted value")]
            public string InvertedFuseId = string.Empty;
        }

        public FusePair[] FusesToInvert = new FusePair[0];

        public IEnumerable<string> SettableFuses => FusesToInvert.Select(f => f.InvertedFuseId);

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
