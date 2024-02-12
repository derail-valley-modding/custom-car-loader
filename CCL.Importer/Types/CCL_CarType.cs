using CCL.Types;
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
    }
}
