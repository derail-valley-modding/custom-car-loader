using DV.ThingTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Custom Livery (Car Variant)")]
    public class CustomLivery : TrainCarLivery
    {
        public CustomCarType CustomParentType => (CustomCarType)parentType;

        [Header("CCL Extended Properties")]
        public TrainCarType BaseCarType;

        [SerializeField, HideInInspector]
        public string? NameTranslationJson = null;
        public TranslationData NameTranslations = new TranslationData();

        [Header("Bogies")]
        public bool UseCustomFrontBogie = false;
        public bool UseCustomRearBogie = false;

        [Header("Buffers")]
        public bool UseCustomBuffers = false;
        public bool UseCustomHosePositions = false;

        public bool HideFrontCoupler = false;
        public bool HideBackCoupler = false;

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

            NameTranslationJson = JsonConvert.SerializeObject(NameTranslations);
        }

        public void ForceValidation()
        {
            OnValidate();
        }

        public void AfterAssetLoad()
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
        }
    }
}