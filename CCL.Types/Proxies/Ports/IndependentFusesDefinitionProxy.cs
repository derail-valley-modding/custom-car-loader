using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class IndependentFusesDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public FuseDefinition[] fuses;

        [SerializeField, HideInInspector]
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

        public override IEnumerable<FuseDefinition> ExposedFuses => fuses ?? base.ExposedFuses;
    }
}
