using System;
using System.Collections.Generic;
using UnityEngine;
using DVLangHelper.Data;
using CCL.Types.Json;
using CCL.Types.HUD;
using CCL.Types.Catalog;

namespace CCL.Types
{
    public enum DVTrainCarKind
    {
        Car,
        Caboose,
        Loco,
        Tender,
        Slug
    }

    [CreateAssetMenu(menuName = "CCL/Custom Car Type")]
    public class CustomCarType : ScriptableObject, IAssetLoadCallback
    {
        [Header("Basic Properties")]
        public DVTrainCarKind KindSelection = DVTrainCarKind.Car;

        public string id = string.Empty;
        public string carIdPrefix = "-";
        public string version = "1.0.0";
        public string author = string.Empty;

        [HideInInspector]
        public string? localizationKey;

        [SerializeField, HideInInspector]
        public string? NameTranslationJson = null;
        public TranslationData NameTranslations;
        public ExtraTranslations ExtraTranslations;

        public List<CustomCarVariant> liveries = new List<CustomCarVariant>();

        [Header("Audio (Optional)")]
        public GameObject SimAudioPrefab;

        [Header("HUD - optional")]
        public VanillaHUDLayout HUDLayout = null!;

        [Header("Physics")]
        public float mass;
        public float bogieSuspensionMultiplier = 1;
        public float rollingResistanceMultiplier = 1;
        public float wheelSlidingFrictionMultiplier = 1;
        public float wheelslipFrictionMultiplier = 1;

        [Header("Wheels")]
        public float wheelRadius;
        public bool useDefaultWheelRotation = true;

        [Header("Cargo")]
        [SerializeField, HideInInspector]
        public string? CargoTypeJson = null;
        public LoadableCargo CargoTypes;

        public BrakesSetup brakes;
        public DamageSetup damage;

        public UnusedCarDeletePreventionMode unusedCarDeletePreventionMode;

        [Space]
        [Tooltip("Any extra prefab that has scripts on it should be added here")]
        public GameObject[] ExtraModels = new GameObject[0];

        [Space]
        public CatalogPage CatalogPage = null!;

        [SerializeField, HideInInspector]
        private string? brakesJson;
        [SerializeField, HideInInspector]
        private string? damageJson;

        public CustomCarType()
        {
            NameTranslations = new TranslationData();
            CargoTypes = new LoadableCargo();
            brakes = new BrakesSetup();
            damage = new DamageSetup();
        }

        [RenderMethodButtons]
        [MethodButton("CCL.Creator.Validators.TrainCarValidator:ValidateExport", "Export Car")]
        public bool buttonRender;

        private void OnValidate()
        {
            localizationKey = $"ccl/car/{id}";

            if (NameTranslations == null || NameTranslations.Items == null || NameTranslations.Items.Count == 0)
            {
                NameTranslations = TranslationData.Default();
            }

            NameTranslationJson = JSONObject.ToJson(NameTranslations.Items);
            CargoTypeJson = CargoTypes.ToJson();

            brakesJson = JSONObject.ToJson(brakes);
            damageJson = JSONObject.ToJson(damage);

            if (ExtraTranslations)
            {
                ExtraTranslations.OnValidate();
            }
        }

        public void ForceValidation()
        {
            OnValidate();
            if (liveries != null)
            {
                foreach (var child in liveries)
                {
                    child.ForceValidation();
                }
            }
        }

        public void AfterAssetLoad(AssetBundle bundle)
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

            if (!string.IsNullOrEmpty(CargoTypeJson))
            {
                CargoTypes = LoadableCargo.FromJson(CargoTypeJson!);
                CargoTypes.AfterAssetLoad(bundle);
            }
            else
            {
                CargoTypes = new LoadableCargo()
                {
                    Entries = new List<LoadableCargoEntry>()
                };
            }

            brakes = JSONObject.FromJson<BrakesSetup>(brakesJson) ?? new BrakesSetup();
            damage = JSONObject.FromJson<DamageSetup>(damageJson) ?? new DamageSetup();

            if (ExtraTranslations)
            {
                ExtraTranslations.AfterImport();
            }

            if (liveries != null)
            {
                foreach (var livery in liveries)
                {
                    livery.AfterAssetLoad();
                }
            }
        }

        public IEnumerable<GameObject> AllPrefabs
        {
            get
            {
                foreach (var cargo in CargoTypes.Entries)
                {
                    if (cargo.ModelVariants != null)
                    {
                        foreach (var model in cargo.ModelVariants)
                        {
                            if (model != null)
                            {
                                yield return model;
                            }
                        }
                    }
                }

                foreach (var model in ExtraModels)
                {
                    if (model != null)
                    {
                        yield return model;
                    }
                }
            }
        }

        [Serializable]
        public class BrakesSetup
        {
            public bool hasCompressor;
            public TrainBrakeType brakeValveType;
            public bool hasIndependentBrake;
            public bool hasHandbrake = true;
            public bool ignoreOverheating;
            public float brakingForcePerBogieMultiplier = 1;

            public enum TrainBrakeType
            {
                None,
                SelfLap,
                ManualLap
            }
        }

        [Serializable]
        public class DamageSetup
        {
            [Header("HP - leave at 0 if unused")]
            public float wheelsHP;
            public float mechanicalPowertrainHP;
            public float electricalPowertrainHP;

            [Header("Price (cars not using wheels price currently")]
            public float bodyPrice = -1f;
            public float wheelsPrice = -1f;
            public float electricalPowertrainPrice = -1f;
            public float mechanicalPowertrainPrice = -1f;
        }
    }

    public interface IAssetLoadCallback
    {
        void AfterAssetLoad(AssetBundle bundle);
    }

    public enum UnusedCarDeletePreventionMode
    {
        None = 0,
        TimeBasedCarVisit = 10,
        TimeBasedCarVisitPropagatedToFrontCar = 11,
        TimeBasedCarVisitPropagatedToRearCar = 12,
        TimeBasedCarVisitPropagatedToFrontAndRearCar = 13,
        OnlyManualDeletePossible = 20
    }
}
