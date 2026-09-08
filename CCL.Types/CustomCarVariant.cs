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
        [Tooltip("This is used to tell if this livery is part of a set of vehicles, " +
            "such as a locomotive and her tender (S282A + S282B)\n" +
            "Order is important")]
        public string[] TrainsetLiveries = new string[0];

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

        [Header("Mod Compatibility - optional")]
        [Tooltip("Used by other mods to limit or enable repetitive spawning\n" +
            "Leave at 0 to ignore")]
        public int MaxRepeatedSpawn = 0;
        [Space]
        [Tooltip("Spawn this livery in regional routes in Passenger Jobs")]
        public bool AllowOnRegionalRoutes = true;
        [Tooltip("Spawn this livery in express routes in Passenger Jobs")]
        public bool AllowOnExpressRoutes = true;
        [Space]
        [Tooltip("Cost to order replacement parts for this vehicle during a demonstrator quest"), Min(0)]
        public float DemonstratorPartsOrderCost = 15000.0f;
        [Tooltip("Cost to install replacement parts for this vehicle during a demonstrator quest"), Min(0)]
        public float DemonstratorPartsInstallationCost = 10000.0f;
        [Tooltip("The name of the demonstrator parts cargo")]
        public TranslationData DemonstratorPartName = new TranslationData();
        [Tooltip("The shortened name of the demonstrator parts cargo")]
        public TranslationData DemonstratorPartNameShort = new TranslationData();
        [Tooltip("Texture to use in the museum posters for this vehicle")]
        public Texture2D? DemonstratorPoster;
        [Tooltip("Livery icon for the demonstrator paint")]
        public Sprite? DemonstratorIcon;
        [Tooltip("Livery icon for the rusty demonstrator paint")]
        public Sprite? DemonstratorRustedIcon;
        [Tooltip("The model to use for the demonstrator parts")]
        public PartsCargoModel PartsModel = PartsCargoModel.GenericBox;
        [Tooltip("The prefab for the parts cargo, when loaded on the DM1U"), EnableIf(nameof(UseCustomPartsModel))]
        public GameObject? PartsCargoPrefabDM1U;
        [Tooltip("The prefab for the parts cargo, when loaded on the Utility Flatbed"), EnableIf(nameof(UseCustomPartsModel))]
        public GameObject? PartsCargoPrefabFlatbed;
        [Tooltip("The mass of the parts cargo")]
        public float PartsCargoMass = 10000.0f;
        [SerializeField, HideInInspector]
        private string? _demoPartNameJson = string.Empty;
        [SerializeField, HideInInspector]
        private string? _demoPartNameShortJson = string.Empty;

        [RenderMethodButtons, SerializeField]
        [MethodButton("CCL.Creator.Wizards.CarPrefabManipulators:AlignBogieColliders", "Align Bogie Colliders")]
        [MethodButton("CCL.Creator.Wizards.CarPrefabManipulators:ResetCouplers", "Reset Coupler Children")]
        private bool _buttons;

        public bool UseCustomFrontBogie => FrontBogie == BogieType.Custom;
        public bool UseCustomRearBogie => RearBogie == BogieType.Custom;
        public bool UseCustomBuffers => BufferType == BufferType.Custom;
        public bool UseCustomPartsModel => PartsModel == PartsCargoModel.Custom;

        public IEnumerable<GameObject> AllPrefabs
        {
            get
            {
                if (prefab != null) yield return prefab;

                if (interiorPrefab != null) yield return interiorPrefab;
                if (explodedInteriorPrefab != null) yield return explodedInteriorPrefab;

                if (externalInteractablesPrefab != null) yield return externalInteractablesPrefab;
                if (explodedExternalInteractablesPrefab != null) yield return explodedExternalInteractablesPrefab;

                if (PartsCargoPrefabDM1U != null) yield return PartsCargoPrefabDM1U;
                if (PartsCargoPrefabFlatbed != null) yield return PartsCargoPrefabFlatbed;
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
            _demoPartNameJson = JSONObject.ToJson(DemonstratorPartName);
            _demoPartNameShortJson = JSONObject.ToJson(DemonstratorPartNameShort);
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
            DemonstratorPartName = JSONObject.FromJson(_demoPartNameJson, () => TranslationData.Default());
            DemonstratorPartNameShort = JSONObject.FromJson(_demoPartNameShortJson, () => TranslationData.Default());
        }
    }
}
