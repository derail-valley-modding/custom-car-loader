using CCL.Types;
using DV;
using DV.Logic.Job;
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
        public LoadableCargo? CargoTypes;
        public GameObject[] ExtraModels = new GameObject[0];
        public ExtraTranslations ExtraTranslations;

        public GameObject SimAudioPrefab;

        private Dictionary<CargoType, float>? _cargoAmounts;
        public Dictionary<CargoType, float> CargoAmounts => Extensions.GetCached(ref _cargoAmounts, GenerateCargoAmounts);

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

        private Dictionary<CargoType, float> GenerateCargoAmounts()
        {
            Dictionary<CargoType, float> dict = new();

            if (CargoTypes == null || CargoTypes.IsEmpty)
            {
                return dict;
            }

            foreach (var item in CargoTypes.Entries)
            {
                CargoType c;

                if (item.IsCustom)
                {
                    if (Globals.G.types.TryGetCargo(item.CustomCargoId, out var cargo))
                    {
                        c = cargo.v1;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    c = (CargoType)item.CargoType;
                }

                if (!dict.ContainsKey(c))
                {
                    dict.Add(c, item.AmountPerCar);
                }
            }

            return dict;
        }
    }
}
