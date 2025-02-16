using UnityEngine;

namespace CCL.Importer.Components.Indicators
{
    internal class IndicatorShaderCustomValueInternal : Indicator
    {
        public MeshRenderer Renderer = null!;
        public string PropertyId = string.Empty;

        private MaterialPropertyBlock _propertyBlock = null!;
        private int _id;

        private void Awake()
        {
            CheckInitialized();
        }

        private void CheckInitialized()
        {
            if (_propertyBlock != null) return;

            _propertyBlock = new MaterialPropertyBlock();
            _id = Shader.PropertyToID(PropertyId);
        }

        protected override void OnValueSet()
        {
            CheckInitialized();
            _propertyBlock.SetFloat(_id, GetNormalizedValue(true));
            Renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
