﻿using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Cargo Setup")]
    public class CargoSetup : ScriptableObject, IAssetLoadCallback
    {
        public CargoEntry[] Entries = new CargoEntry[0];

        [SerializeField, HideInInspector]
        private GameObject[]? _models;
        [SerializeField, HideInInspector]
        private int[]? _counts;
        [SerializeField, HideInInspector]
        private string? _json;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(SortEntries), "Sort Entries")]
        private bool _buttons;

        private void OnValidate()
        {
            var models = new List<GameObject>();
            var counts = new List<int>();

            foreach (var item in Entries)
            {
                models.AddRange(item.Models);
                counts.Add(item.Models.Length);
            }

            _models = models.ToArray();
            _counts = counts.ToArray();

            _json = JSONObject.ToJson(Entries);
        }

        public void AfterAssetLoad(AssetBundle bundle)
        {
            Entries = JSONObject.FromJson(_json, () => new CargoEntry[0]);

            if (_counts == null || _models == null) return;

            int index = 0;
            int count;

            for (int i = 0; i < Entries.Length; i++)
            {
                count = _counts[i];

                Entries[i].Models = new GameObject[count];
                Array.Copy(_models, index, Entries[i].Models, 0, count);

                index += count;
            }
        }

        private void SortEntries()
        {
            Entries = Entries
                .OrderBy(x => x.CargoId)
                .OrderBy(x => !IdV2.Cargos.Any(y => y == x.CargoId))
                .OrderBy(x => string.IsNullOrEmpty(x.CargoId))
                .ToArray();
        }
    }

    [Serializable]
    public class CargoEntry
    {
        [CargoField]
        public string CargoId = string.Empty;
        public float AmountPerCar = 1f;
        [JsonIgnore]
        public GameObject[] Models = new GameObject[0];
    }
}
