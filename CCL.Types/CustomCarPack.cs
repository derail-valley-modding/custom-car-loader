﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Custom Car Pack")]
    public class CustomCarPack : ScriptableObject, ICustomSerialized
    {
        [Header("Pack Info")]
        public string PackId = string.Empty;
        public string PackName = string.Empty;
        public string Author = string.Empty;
        public string Version = "1.0.0";
        [Tooltip("Optional")]
        public string HomePage = string.Empty;
        [Tooltip("Optional")]
        public string Repository = string.Empty;

        [Header("Custom Cars")]
        public CustomCarType[] Cars = new CustomCarType[0];

        [Header("Optional")]
        public GameObject[] ExtraModels = new GameObject[0];
        public PaintSubstitutions[] PaintSubstitutions = new PaintSubstitutions[0];
        public ProceduralMaterialDefinitions? ProceduralMaterials;
        public ExtraTranslations? ExtraTranslations;
        [Tooltip("Additional mod dependencies go here\n" +
            "CCL will automatically add Custom Cargo, Custom Licenses, and Passenger Jobs if needed")]
        public List<string> AdditionalDependencies = new List<string>();

        [RenderMethodButtons, SerializeField]
        [MethodButton("CCL.Creator.Validators.CarPackValidator:ValidateExport", "Export Pack")]
        private bool _buttons;

        // Data for CCL info.
        [HideInInspector]
        public string ExporterVersion = string.Empty;

        public void OnValidate()
        {
            if (ProceduralMaterials != null)
            {
                ProceduralMaterials.OnValidate();
            }

            if (ExtraTranslations != null)
            {
                ExtraTranslations.OnValidate();
            }

            AdditionalDependencies = AdditionalDependencies.Distinct().ToList();
        }

        public void AfterImport()
        {
            foreach (var car in Cars)
            {
                car.AfterImport();
            }

            foreach (var paint in PaintSubstitutions)
            {
                paint.AfterImport();
            }

            if (ProceduralMaterials != null)
            {
                ProceduralMaterials.AfterImport();
            }

            if (ExtraTranslations != null)
            {
                ExtraTranslations.AfterImport();
            }
        }
    }
}
