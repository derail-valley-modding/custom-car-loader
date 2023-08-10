using DV.ThingTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    [Serializable]
    public class LoadableCargo : IAssetLoadCallback
    {
        public List<LoadableCargoEntry> Entries;

        public bool IsEmpty => Entries == null || Entries.Count == 0;

        public void AfterAssetLoad(AssetBundle bundle)
        {
            foreach (var entry in Entries)
            {
                entry.AfterAssetLoad(bundle);
            }
        }
    }

    [Serializable]
    public class LoadableCargoEntry : IAssetLoadCallback
    {
        public float AmountPerCar = 1f;
        public CargoType CargoType;

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
