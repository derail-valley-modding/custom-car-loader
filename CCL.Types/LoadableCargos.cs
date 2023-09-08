using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    [Serializable]
    public class LoadableCargo : IAssetLoadCallback
    {
        public List<LoadableCargoEntry> Entries = new List<LoadableCargoEntry>();

        public bool IsEmpty => Entries == null || Entries.Count == 0;

        public void AfterAssetLoad(AssetBundle bundle)
        {
            foreach (var entry in Entries)
            {
                entry.AfterAssetLoad(bundle);
            }
        }

        public string ToJson()
        {
            return JSONObject.CreateFromObject(Entries).ToString();
        }

        public static LoadableCargo FromJson(string json)
        {
            var parsed = JSONObject.Create(json);
            var entries = parsed.ToObject<List<LoadableCargoEntry>>();
            return new LoadableCargo()
            {
                Entries = entries
            };
        }
    }

    [Serializable]
    public class LoadableCargoEntry : IAssetLoadCallback
    {
        public float AmountPerCar = 1f;
        public BaseCargoType CargoType;

        [JsonIgnore]
        public GameObject[]? ModelVariants;
        [HideInInspector]
        public string[]? ModelPaths;

        public void AfterAssetLoad(AssetBundle bundle)
        {
            ModelVariants = ModelPaths?.Select(p => bundle.LoadAsset<GameObject>(p)).ToArray();
        }
    }
}
