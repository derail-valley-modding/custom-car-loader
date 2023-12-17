using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DVLangHelper.Data;

namespace CCL.Types
{
    public enum DVTrainCarKind
    {
        Car,
        Caboose,
        Loco,
        Tender,
    }

    [CreateAssetMenu(menuName = "CCL/Custom Car Type")]
    public class CustomCarType : ScriptableObject, IAssetLoadCallback
    {
        [Header("Basic Properties")]
        public DVTrainCarKind KindSelection = DVTrainCarKind.Car;

        public string id = string.Empty;
        public string carInstanceIdGenBase = "-";
        public string version = "1.0.0";
        public string author = string.Empty;

        [HideInInspector]
        public string? localizationKey;

        [SerializeField, HideInInspector]
        public string? NameTranslationJson = null;
        public TranslationData NameTranslations;

        public List<CustomCarVariant> liveries = new List<CustomCarVariant>();

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

        public CustomCarType()
        {
            NameTranslations = new TranslationData();
            CargoTypes = new LoadableCargo();
            brakes = new BrakesSetup();
            damage = new DamageSetup();
        }

        [RenderMethodButtons]
        [MethodButton("CCL.Creator.TrainCarValidator:ValidateExport", "Export Car")]
        public bool buttonRender;

        private void OnValidate()
        {
            localizationKey = $"ccl/car/{id}";

            if (NameTranslations == null || NameTranslations.Items == null || NameTranslations.Items.Count == 0)
            {
                NameTranslations = TranslationData.Default();
            }

            NameTranslationJson = NameTranslations.ToJson();
            CargoTypeJson = CargoTypes.ToJson();
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
                NameTranslations = TranslationDataExtensions.FromJson(NameTranslationJson!);
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

            if (liveries != null)
            {
                foreach (var livery in liveries)
                {
                    livery.AfterAssetLoad();
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
}
