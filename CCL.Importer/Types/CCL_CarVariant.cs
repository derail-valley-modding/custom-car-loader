using CCL.Importer.Processing;
using CCL.Types;
using DV.ThingTypes;
using DVLangHelper.Data;
using System;
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

        public void RemakePrefabs()
        {
            prefab = ModelProcessor.CreateModifiablePrefab(prefab);

            if (interiorPrefab) interiorPrefab =  ModelProcessor.CreateModifiablePrefab(interiorPrefab);
            if (explodedInteriorPrefab) explodedInteriorPrefab = ModelProcessor.CreateModifiablePrefab(explodedInteriorPrefab);

            if (externalInteractablesPrefab) externalInteractablesPrefab = ModelProcessor.CreateModifiablePrefab(externalInteractablesPrefab);
            if (explodedExternalInteractablesPrefab) explodedExternalInteractablesPrefab = ModelProcessor.CreateModifiablePrefab(explodedExternalInteractablesPrefab);
        }
    }
}
