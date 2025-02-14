using System;
using System.Collections.Generic;
using UnityEngine;
using DVLangHelper.Data;
using CCL.Types.Json;
using CCL.Types.HUD;
using CCL.Types.Catalog;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Custom Car Type")]
    public class CustomCarType : ScriptableObject, IAssetLoadCallback
    {
        public enum UnusedCarDeletePreventionMode
        {
            None = 0,
            TimeBasedCarVisit = 10,
            TimeBasedCarVisitPropagatedToFrontCar = 11,
            TimeBasedCarVisitPropagatedToRearCar = 12,
            TimeBasedCarVisitPropagatedToFrontAndRearCar = 13,
            OnlyManualDeletePossible = 20
        }

        public const float ROLLING_RESISTANCE_COEFFICIENT = 0.002f;
        public const float WHEELSLIDE_FRICTION_COEFFICIENT = 0.13f;
        public const float WHEELSLIP_FRICTION_COEFFICIENT = 0.2f;

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

        [Header("Physics")]
        public float mass;
        public float bogieSuspensionMultiplier = 1;
        public float rollingResistanceCoefficient = ROLLING_RESISTANCE_COEFFICIENT;
        public float wheelSlidingFrictionCoefficient = WHEELSLIDE_FRICTION_COEFFICIENT;
        public float wheelslipFrictionCoefficient = WHEELSLIP_FRICTION_COEFFICIENT;

        [Header("Wheels")]
        public float wheelRadius;
        public bool useDefaultWheelRotation = true;

        [Header("Cargo")]
        public CargoSetup? CargoSetup;

        public BrakesSetup brakes;
        public DamageSetup damage;

        public UnusedCarDeletePreventionMode unusedCarDeletePreventionMode;

        [Space]
        [Tooltip("Any extra prefab that has scripts on it should be added here")]
        public GameObject[] ExtraModels = new GameObject[0];

        [Header("Audio - optional")]
        public GameObject SimAudioPrefab;

        [Header("HUD - optional")]
        public VanillaHUDLayout HUDLayout = null!;

        [Header("Catalog - optional")]
        public CatalogPage CatalogPage = null!;

        [Header("License - optional")]
        [Tooltip("Only general license IDs are supported (not job licenses)")]
        [GeneralLicenseField]
        public string LicenseID = "";

        [Header("Paints - optional")]
        public PaintSubstitutions[] PaintSubstitutions = new PaintSubstitutions[0];

        [SerializeField, HideInInspector]
        private string? brakesJson;
        [SerializeField, HideInInspector]
        private string? damageJson;

        public CustomCarType()
        {
            NameTranslations = new TranslationData();
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

            if (CargoSetup != null)
            {
                CargoSetup.AfterImport();
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

            foreach (var item in PaintSubstitutions)
            {
                item.AfterImport();
            }
        }

        public IEnumerable<GameObject> AllPrefabs
        {
            get
            {
                if (CargoSetup != null)
                {
                    foreach (var cargo in CargoSetup.Entries)
                    {
                        if (cargo.Models == null) continue;

                        foreach (var model in cargo.Models)
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
            public enum TrainBrakeType
            {
                None,
                SelfLap,
                ManualLap
            }

            public enum BrakeCylinderPressureCalculation
            {
                Regular,
                CopyFront,
                CopyRear,
                CopyMax
            }

            public bool hasCompressor;
            public bool hasMainResConnections;
            public TrainBrakeType brakeValveType;
            public bool hasIndependentBrake;
            public bool hasHandbrake = true;
            public bool ignoreOverheating;
            //[Header("Leave brake curve data blank for linear behaviour")]
            //public BrakesCurve trainBrakeCurveData;
            //public BrakesCurve indBrakeCurveData;
            public BrakeCylinderPressureCalculation brakeCylinderPressureCalculation;
            public float brakingForcePerBogieMultiplier = 1;
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
}
