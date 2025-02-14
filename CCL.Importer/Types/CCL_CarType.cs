using CCL.Types;
using CCL.Types.Catalog;
using DV;
using DV.ThingTypes;
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
        public CargoSetup? CargoSetup;
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
                if ((CargoSetup == null) || CargoSetup.IsEmpty) yield break;

                foreach (var cargo in CargoSetup.Entries)
                {
                    if (cargo.Models == null) continue;

                    foreach (var model in cargo.Models)
                    {
                        yield return model;
                    }
                }
            }
        }

        private Dictionary<string, float> GenerateCargoAmounts()
        {
            Dictionary<string, float> dict = new();

            if (CargoSetup == null || CargoSetup.IsEmpty)
            {
                return dict;
            }

            foreach (var item in CargoSetup.Entries)
            {
                if (!Globals.G.types.TryGetCargo(item.CargoId, out _)) continue;

                if (!dict.ContainsKey(item.CargoId))
                {
                    dict.Add(item.CargoId, item.AmountPerCar);
                }
            }

            return dict;
        }
    }
}
