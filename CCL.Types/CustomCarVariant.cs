using System.Collections.Generic;
using UnityEngine;
using DVLangHelper.Data;

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
        [MethodButton("CCL.Creator.Editor.CarPrefabManipulators:AlignBogieColliders", "Align Bogie Colliders")]
        public bool buttonRender;

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

            NameTranslationJson = NameTranslations.ToJson();
        }

        public void ForceValidation()
        {
            OnValidate();
        }

        public void AfterAssetLoad()
        {
            if (!string.IsNullOrEmpty(NameTranslationJson))
            {
                NameTranslations = TranslationDataExtensions.FromJson(NameTranslationJson!);
            }
            else
            {
                NameTranslations = TranslationData.Default();
            }
        }
    }
}