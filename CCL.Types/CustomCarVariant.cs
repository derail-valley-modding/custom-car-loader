using System.Collections.Generic;
using UnityEngine;
using DVLangHelper.Data;
using CCL.Types.Json;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Car Variant")]
    public class CustomCarVariant : ScriptableObject
    {
        [Header("Basic Properties")]
        public CustomCarType? parentType;
        public string? id;
        public BaseTrainCarType BaseCarType;

        [SerializeField, HideInInspector]
        public string? NameTranslationJson = null;
        public TranslationData NameTranslations = new TranslationData();

        public Sprite? icon;

        [HideInInspector]
        public string? localizationKey = null;

        [Header("Models")]
        public GameObject? prefab;
        [Header("Optional Models")]
        public GameObject? interiorPrefab;
        public GameObject? explodedInteriorPrefab;
        [Space]
        public GameObject? externalInteractablesPrefab;
        public GameObject? explodedExternalInteractablesPrefab;

        [Header("Bogies")]
        public bool UseCustomFrontBogie = false;
        public bool UseCustomRearBogie = false;

        [Header("Buffers")]
        public bool UseCustomBuffers = false;
        public bool UseCustomHosePositions = false;

        public bool HideFrontCoupler = false;
        public bool HideBackCoupler = false;

        [RenderMethodButtons]
        [MethodButton("CCL.Creator.Wizards.CarPrefabManipulators:AlignBogieColliders", "Align Bogie Colliders")]
        public bool buttonRender;

        public IEnumerable<GameObject> AllPrefabs
        {
            get
            {
                if (prefab) yield return prefab!;

                if (interiorPrefab) yield return interiorPrefab!;
                if (explodedInteriorPrefab) yield return explodedInteriorPrefab!;

                if (externalInteractablesPrefab) yield return externalInteractablesPrefab!;
                if (explodedExternalInteractablesPrefab) yield return explodedExternalInteractablesPrefab!;
            }
        }

        private void OnValidate()
        {
            localizationKey = $"ccl/livery/{id}";

            if (NameTranslations == null || NameTranslations.Items == null || NameTranslations.Items.Count == 0)
            {
                NameTranslations = new TranslationData()
                {
                    Items = new List<TranslationItem>() { new TranslationItem() }
                };
            }

            NameTranslationJson = JSONObject.ToJson(NameTranslations.Items);
        }

        public void ForceValidation()
        {
            OnValidate();

            if (prefab) ForceValidatePrefab(prefab!);
            if (interiorPrefab) ForceValidatePrefab(interiorPrefab!);
            if (explodedInteriorPrefab) ForceValidatePrefab(explodedInteriorPrefab!);

            if (externalInteractablesPrefab) ForceValidatePrefab(externalInteractablesPrefab!);
            if (explodedExternalInteractablesPrefab) ForceValidatePrefab(explodedExternalInteractablesPrefab!);
        }

        private void ForceValidatePrefab(GameObject prefab)
        {
            foreach (var customSerialized in prefab.GetComponentsInChildren<ICustomSerialized>())
            {
                customSerialized.OnValidate();
            }
        }

        public void AfterAssetLoad()
        {
            if (!string.IsNullOrEmpty(NameTranslationJson))
            {
                var items = JSONObject.FromJson<List<TranslationItem>>(NameTranslationJson);
                NameTranslations = new TranslationData() { Items = items! };
            }
            else
            {
                NameTranslations = TranslationData.Default();
            }
        }
    }
}