using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Cargo Setup")]
    public class CargoSetup : ScriptableObject, ICustomSerialized
    {
        public List<CargoEntry> Entries = new List<CargoEntry>();

        [SerializeField, HideInInspector]
        private string? _json;
        [SerializeField, HideInInspector]
        private Sprite[]? _sprite;
        [SerializeField, HideInInspector]
        private GameObject[]? _models;
        [SerializeField, HideInInspector]
        private int[]? _counts;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(SortEntries), "Sort Entries")]
        [MethodButton("CCL.Creator.Wizards.CargoWizard:ShowWindowForSetup", "Open Wizard")]
        private bool _buttons;

        public bool IsEmpty => Entries.Count == 0;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Entries);

            var sprite = new List<Sprite>();
            var models = new List<GameObject>();
            var counts = new List<int>();

            foreach (var item in Entries)
            {
                sprite.Add(item.LoadedIcon);
                models.AddRange(item.Models);
                counts.Add(item.Models.Length);
            }

            _sprite = sprite.ToArray();
            _models = models.ToArray();
            _counts = counts.ToArray();
        }

        public void AfterImport()
        {
            Entries = JSONObject.FromJson(_json, () => new List<CargoEntry>());

            if (_sprite != null)
            {
                for (int i = 0; i < Entries.Count; i++)
                {
                    Entries[i].LoadedIcon = _sprite[i];
                }
            }

            if (_counts == null || _models == null) return;

            int index = 0;
            int count;

            for (int i = 0; i < Entries.Count; i++)
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
                .ToList();
        }
    }

    [Serializable]
    public class CargoEntry
    {
        [CargoField]
        public string CargoId = string.Empty;
        public float AmountPerCar = 1f;
        public bool OverrideLoadedIcon = true;
        [JsonIgnore]
        public Sprite LoadedIcon = null!;
        [JsonIgnore]
        public GameObject[] Models = new GameObject[0];
    }
}
