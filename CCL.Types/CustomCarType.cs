using DV.ThingTypes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public class CustomCarType : TrainCarType_v2, IAssetLoadCallback
    {
        [NonSerialized]
        public Version ExporterVersion = ExporterConstants.ExporterVersion;

        [Header("CCL Extended Properties")]
        public DVTrainCarKind KindSelection = DVTrainCarKind.Car;

        public string Version = "1.0.0";
        public string Author = "";

        [SerializeField, HideInInspector]
        public string? NameTranslationJson = null;
        public TranslationData NameTranslations;

        [SerializeField, HideInInspector]
        public string? CargoTypeJson = null;
        public LoadableCargo CargoTypes;

        public IEnumerable<CustomLivery> CustomLiveries => liveries.Cast<CustomLivery>();

        public CustomCarType() : base()
        {
            NameTranslations = new TranslationData();
            CargoTypes = new LoadableCargo();
        }

        [RenderMethodButtons]
        [MethodButton("CCL.Creator.TrainCarValidator:ValidateExport", "Export Car")]
        public bool buttonRender;

        private void OnValidate()
        {
            localizationKey = $"ccl/car/{id}";

            if (liveries.Any(l => l && !(l is CustomLivery)))
            {
                liveries.RemoveAll(l => l && !(l is CustomLivery));
            }

            if (NameTranslations == null || NameTranslations.Items == null || NameTranslations.Items.Count == 0)
            {
                NameTranslations = new TranslationData()
                {
                    Items = new List<TranslationItem>() { new TranslationItem() }
                };
            }

            NameTranslationJson = JsonConvert.SerializeObject(NameTranslations);
            CargoTypeJson = JsonConvert.SerializeObject(CargoTypes);
        }

        public void ForceValidation()
        {
            OnValidate();
            foreach (var child in CustomLiveries)
            {
                child.ForceValidation();
            }
        }

        public void AfterAssetLoad(AssetBundle bundle)
        {
            if (!string.IsNullOrEmpty(NameTranslationJson))
            {
                NameTranslations = JsonConvert.DeserializeObject<TranslationData>(NameTranslationJson!)!;
            }
            else
            {
                NameTranslations = new TranslationData()
                {
                    Items = new List<TranslationItem>()
                };
            }

            if (!string.IsNullOrEmpty(CargoTypeJson))
            {
                CargoTypes = JsonConvert.DeserializeObject<LoadableCargo>(CargoTypeJson!)!;
                CargoTypes.AfterAssetLoad(bundle);
            }
            else
            {
                CargoTypes = new LoadableCargo()
                {
                    Entries = new List<LoadableCargoEntry>()
                };
            }

            foreach (var livery in CustomLiveries)
            {
                livery.AfterAssetLoad();
            }
        }
    }

    public interface IAssetLoadCallback
    {
        void AfterAssetLoad(AssetBundle bundle);
    }
}
