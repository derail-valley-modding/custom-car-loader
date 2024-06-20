using CCL.Types;
using CCL.Types.Catalog;
using DV;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DVLangHelper.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Types
{
    public class CCL_CarType : TrainCarType_v2
    {
        public IEnumerable<CCL_CarVariant> Variants => liveries.OfType<CCL_CarVariant>();

        public DVTrainCarKind KindSelection;
        public TranslationData NameTranslations = new();
        public LoadableCargo? CargoTypes;
        public GameObject[] ExtraModels = new GameObject[0];
        public CatalogPage? CatalogPage;
        public ExtraTranslations ExtraTranslations;

        public GameObject SimAudioPrefab;

        private Dictionary<string, float>? _cargoAmounts;
        public Dictionary<string, float> CargoAmounts => Extensions.GetCached(ref _cargoAmounts, GenerateCargoAmounts);

        public IEnumerable<GameObject> AllCargoModels
        {
            get
            {
                if ((CargoTypes == null) || CargoTypes.IsEmpty) yield break;

                foreach (var cargoType in CargoTypes.Entries)
                {
                    if (cargoType.ModelVariants == null) continue;

                    foreach (var variant in cargoType.ModelVariants)
                    {
                        yield return variant;
                    }
                }
            }
        }

        private Dictionary<string, float> GenerateCargoAmounts()
        {
            Dictionary<string, float> dict = new();

            if (CargoTypes == null || CargoTypes.IsEmpty)
            {
                return dict;
            }

            foreach (var item in CargoTypes.Entries)
            {
                string id;

                if (item.IsCustom)
                {
                    if (Globals.G.types.TryGetCargo(item.CustomCargoId, out _))
                    {
                        id = item.CustomCargoId;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    id = ((CargoType)item.CargoType).ToV2().id;
                }

                if (!dict.ContainsKey(id))
                {
                    dict.Add(id, item.AmountPerCar);
                }
            }

            return dict;
        }
    }
}
