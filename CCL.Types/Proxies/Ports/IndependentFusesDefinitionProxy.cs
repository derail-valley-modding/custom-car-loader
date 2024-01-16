using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class IndependentFusesDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public FuseDefinition[] fuses;

        [SerializeField]
        private string fusesJson;

        public bool saveState = true;

        public void AfterImport()
        {
            fuses = JSONObject.FromJson<FuseDefinition[]>(fusesJson);
        }

        public void OnValidate()
        {
            fusesJson = JSONObject.ToJson(fuses);
        }
    }
}
