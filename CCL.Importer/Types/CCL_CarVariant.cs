using CCL.Importer.Processing;
using CCL.Types;
using DV.ThingTypes;
using DVLangHelper.Data;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Types
{
    public class CCL_CarVariant : TrainCarLivery
    {
        public TranslationData NameTranslations = new TranslationData();

        public BogieType FrontBogie;
        public BogieType RearBogie;

        public BufferType BufferType;
        public bool HasMUCable;
        public bool UseCustomHosePositions;

        public bool HideFrontCoupler;
        public bool HideBackCoupler;

        public LocoSpawnGroup[] LocoSpawnGroups = new LocoSpawnGroup[0];

        public bool UseCustomFrontBogie => FrontBogie == BogieType.Custom;
        public bool UseCustomRearBogie => RearBogie == BogieType.Custom;
        public bool UseCustomBuffers => BufferType == BufferType.Custom;

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
