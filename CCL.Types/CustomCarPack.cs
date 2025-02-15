using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Custom Car Pack")]
    public class CustomCarPack : ScriptableObject, IAssetLoadCallback
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
        public ExtraTranslations? ExtraTranslations;

        [RenderMethodButtons, SerializeField]
        [MethodButton("CCL.Creator.Validators.CarPackValidator:ValidateExport", "Export Car")]
        private bool _buttons;

        private void OnValidate()
        {
            if (ExtraTranslations != null)
            {
                ExtraTranslations.OnValidate();
            }
        }

        public void AfterAssetLoad(AssetBundle bundle)
        {
            foreach (var car in Cars)
            {
                car.AfterAssetLoad(bundle);
            }

            foreach (var paint in PaintSubstitutions)
            {
                paint.AfterImport();
            }

            if (ExtraTranslations != null)
            {
                ExtraTranslations.AfterImport();
            }
        }
    }
}
