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
        [EnableIf(nameof(UseCustomBuffers))]
        public bool UseCustomHosePositions = false;
        [EnableIf(nameof(UseCustomBuffers))]
        public bool HideHookPlates = false;

        public bool HideFrontCoupler = false;
        public bool HideBackCoupler = false;

        [Header("Trainset - optional")]
        [Tooltip("This is used to tell if this vehicle is part of a set of vehicles\n" +
            "Examples are a locomotive and her tender (S282A + S282B)\n" +
            "Order is important")]
        public string[] TrainsetLiveries = new string[0];
        [Tooltip("Used by other mods to limit or enable repetitive spawning\n" +
            "Leave at 0 to ignore")]
        public int MaxRepeatedSpawn = 0;
        [Tooltip("Only affects Passenger Jobs")]
        public bool AllowOnRegionalRoutes = true;
        [Tooltip("Only affects Passenger Jobs")]
        public bool AllowOnExpressRoutes = true;

        [Header("Spawning - optional")]
        public LocoSpawnGroup[] LocoSpawnGroups = new LocoSpawnGroup[0];
        [SerializeField, HideInInspector]
        private string? _spawnGroupJson = string.Empty;
        
        [Header("Work Train - optional")]
        public bool UnlockableAsWorkTrain = false;
        public float UnlockPrice = 30000.0f;
        public float SummonPrice = 5000.0f;

        [Header("Catalog - optional")]
        public CatalogPage? CatalogPage = null;

        [RenderMethodButtons, SerializeField]
        [MethodButton("CCL.Creator.Wizards.CarPrefabManipulators:AlignBogieColliders", "Align Bogie Colliders")]
        [MethodButton("CCL.Creator.Wizards.CarPrefabManipulators:ResetCouplers", "Reset Coupler Children")]
        private bool _buttons;

        public bool UseCustomFrontBogie => FrontBogie == BogieType.Custom;
        public bool UseCustomRearBogie => RearBogie == BogieType.Custom;
        public bool UseCustomBuffers => BufferType == BufferType.Custom;

        public IEnumerable<GameObject> AllPrefabs
        {
            get
            {
                if (prefab != null) yield return prefab;

                if (interiorPrefab != null) yield return interiorPrefab;
                if (explodedInteriorPrefab != null) yield return explodedInteriorPrefab;

                if (externalInteractablesPrefab != null) yield return externalInteractablesPrefab;
                if (explodedExternalInteractablesPrefab != null) yield return explodedExternalInteractablesPrefab;
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
