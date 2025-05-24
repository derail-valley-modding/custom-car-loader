using System.Collections.Generic;
using UnityEngine;
using DVLangHelper.Data;
using CCL.Types.Json;
using CCL.Types.Catalog;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Car Variant", order = MenuOrdering.CarLivery)]
    public class CustomCarVariant : ScriptableObject
    {
        [Header("Basic Properties")]
        public CustomCarType? parentType;
        public string id = string.Empty;

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
        public BogieType FrontBogie;
        public BogieType RearBogie;

        [Header("Buffers")]
        public BufferType BufferType = BufferType.Buffer09;
        public bool HasMUCable = false;
        public bool UseCustomHosePositions = false;

        public bool HideHookPlates = false;
        public bool HideFrontCoupler = false;
        public bool HideBackCoupler = false;

        [Header("Spawning")]
        public LocoSpawnGroup[] LocoSpawnGroups = new LocoSpawnGroup[0];
        [SerializeField, HideInInspector]
        private string? _spawnGroupJson = string.Empty;

        [Header("Catalog - optional")]
        public CatalogPage? CatalogPage = null;

        [RenderMethodButtons]
        [MethodButton("CCL.Creator.Wizards.CarPrefabManipulators:AlignBogieColliders", "Align Bogie Colliders")]
        public bool buttonRender;

        public bool UseCustomFrontBogie => FrontBogie == BogieType.Custom;
        public bool UseCustomRearBogie => RearBogie == BogieType.Custom;
        public bool UseCustomBuffers => BufferType == BufferType.Custom;

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

            _spawnGroupJson = JSONObject.ToJson(LocoSpawnGroups);
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

        public void AfterImport()
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

            LocoSpawnGroups = JSONObject.FromJson(_spawnGroupJson, () => LocoSpawnGroups);
        }
    }
}
