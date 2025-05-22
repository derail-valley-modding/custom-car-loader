using System;
using System.Collections.Generic;
using UnityEngine;
using DVLangHelper.Data;
using CCL.Types.Json;
using CCL.Types.HUD;
using CCL.Types.Catalog;
using CCL.Types.Proxies;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Custom Car Type", order = MenuOrdering.CarType)]
    public class CustomCarType : ScriptableObject, ICustomSerialized
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
        public const float BOGIE_BRAKING_FORCE = 15000.0f;

        [Header("Basic Properties")]
        public DVTrainCarKind KindSelection = DVTrainCarKind.Car;

        public string id = string.Empty;
        public string carIdPrefix = "-";
        [Tooltip("Check if this car should be treated as a steam locomotive")]
        public bool IsSteamLocomotive = false;

        [HideInInspector]
        public string? localizationKey;

        [Space]
        public TranslationData NameTranslations;
        [SerializeField, HideInInspector]
        public string? NameTranslationJson = null;

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

        [Space]
        public UnusedCarDeletePreventionMode unusedCarDeletePreventionMode;

        [Header("Audio - optional")]
        public GameObject? SimAudioPrefab;

        [Header("HUD - optional")]
        public VanillaHUDLayout HUDLayout = null!;

        [Header("License - optional")]
        [Tooltip("Only general license IDs are supported (not job licenses)")]
        [GeneralLicenseField]
        public string LicenseID = "";

        [SerializeField, HideInInspector]
        private string? brakesJson;
        [SerializeField, HideInInspector]
        private string? damageJson;

        public bool IsActualSteamLocomotive => IsSteamLocomotive && KindSelection == DVTrainCarKind.Loco;

        public CustomCarType()
        {
            NameTranslations = new TranslationData();
            brakes = new BrakesSetup();
            damage = new DamageSetup();
        }

        public void OnValidate()
        {
            localizationKey = $"ccl/car/{id}";

            if (NameTranslations == null || NameTranslations.Items == null || NameTranslations.Items.Count == 0)
            {
                NameTranslations = TranslationData.Default();
            }

            NameTranslationJson = JSONObject.ToJson(NameTranslations.Items);

            brakesJson = JSONObject.ToJson(brakes);
            damageJson = JSONObject.ToJson(damage);
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

            if (CargoSetup != null)
            {
                CargoSetup.AfterImport();
            }

            brakes = JSONObject.FromJson<BrakesSetup>(brakesJson) ?? new BrakesSetup();
            damage = JSONObject.FromJson<DamageSetup>(damageJson) ?? new DamageSetup();

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
            }
        }

        [Serializable]
        public class BrakesSetup : IWagonDefaults
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

            public void ApplyWagonDefaults()
            {
                hasCompressor = false;
                hasMainResConnections = false;
                brakeValveType = TrainBrakeType.None;
                hasIndependentBrake = false;
                hasHandbrake = true;
                ignoreOverheating = false;
                brakeCylinderPressureCalculation = BrakeCylinderPressureCalculation.Regular;
                brakingForcePerBogieMultiplier = 1.0f;
            }
        }

        [Serializable]
        public class DamageSetup : IWagonDefaults
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

            public void ApplyWagonDefaults()
            {
                wheelsHP = 0.0f;
                mechanicalPowertrainHP = 0.0f;
                electricalPowertrainHP = 0.0f;

                wheelsPrice = -1.0f;
                electricalPowertrainPrice = -1.0f;
                mechanicalPowertrainPrice = -1.0f;
            }
        }
    }
}
