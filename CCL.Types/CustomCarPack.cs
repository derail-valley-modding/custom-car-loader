using System.Collections.Generic;
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

        [Header("Custom Cars")]
        public CustomCarType[] Cars = new CustomCarType[0];

        [Header("Optional")]
        public GameObject[] ExtraModels = new GameObject[0];
        public PaintSubstitutions[] PaintSubstitutions = new PaintSubstitutions[0];
        public ProceduralMaterialDefinitions? ProceduralMaterials;
        public ExtraTranslations? ExtraTranslations;

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
