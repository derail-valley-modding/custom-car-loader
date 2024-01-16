using CCL.Types;
using DV.ThingTypes;
using DVLangHelper.Data;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Types
{
    public class CCL_CarVariant : TrainCarLivery
    {
        public TrainCarType BaseCarType;
        public TranslationData NameTranslations = new TranslationData();

        public bool UseCustomFrontBogie;
        public bool UseCustomRearBogie;

        public bool UseCustomBuffers;
        public bool UseCustomHosePositions;

        public bool HideFrontCoupler;
        public bool HideBackCoupler;

        public IEnumerable<GameObject> AllPrefabs
        {
            get
            {
                yield return prefab;

                if (interiorPrefab) yield return interiorPrefab;
                if (explodedInteriorPrefab) yield return explodedInteriorPrefab;
                
                if (externalInteractablesPrefab) yield return externalInteractablesPrefab;
                if (explodedExternalInteractablesPrefab) yield return explodedExternalInteractablesPrefab;
            }
        }
    }
}
