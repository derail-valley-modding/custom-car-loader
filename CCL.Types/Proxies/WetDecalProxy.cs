using System;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class WetDecalProxy : MonoBehaviour
    {
        public enum LayerMode
        {
            None,
            Single,
            Triplanar
        }

        public enum ProjectionMode
        {
            Local,
            World
        }

        public enum WetDecalMode
        {
            Wet,
            Dry
        }

        public enum DecalShape
        {
            Cube,
            Sphere,
            Mesh
        }

        [Serializable]
        public class DecalSettingsProxy
        {
            [SerializeField]
            [Tooltip("Should this affect upside-down surfaces as well?")]
            private bool _affectCeiling;

            [SerializeField]
            [Tooltip("The distance from the edge of the shape from which saturation begins to fade.")]
            [Range(0f, 1f)]
            private float _edgeFadeoff;

            [SerializeField]
            [Tooltip("Jitter sample positions of detail layer textures")]
            private bool _enableJitter;

            [SerializeField]
            [Tooltip("How sharply the decal fades around faces facing in different directions")]
            [Range(0f, 10f)]
            private float _faceSharpness;

            [SerializeField]
            [Tooltip("The detail layer mode")]
            private LayerMode _layerMode;

            [SerializeField]
            [Tooltip("The layer projection mode")]
            private ProjectionMode _layerProjection;

            [SerializeField]
            [Tooltip("Determines if the decal projects wetness, or if it drys other wet decals")]
            private WetDecalMode _mode;

            [SerializeField]
            [Tooltip("Maximum jitter in texels to the detail layer sampling coordinates")]
            [Range(0f, 10f)]
            private float _sampleJitter;

            [SerializeField]
            [Tooltip("How wet the decal appears to be")]
            [Range(0f, 1f)]
            private float _saturation;

            [SerializeField]
            [Tooltip("The shape of the decal")]
            private DecalShape _shape;

            [SerializeField]
            [Tooltip("Per pixel saturation projected down the decal's x axis")]
            private DecalLayerProxy _xLayer;

            [SerializeField]
            [Tooltip("Per pixel saturation projected down the decal's y axis")]
            private DecalLayerProxy _yLayer;

            [SerializeField]
            [Tooltip("Per pixel saturation projected down the decal's z axis")]
            private DecalLayerProxy _zLayer;

            public DecalShape Shape => _shape;

            public DecalSettingsProxy()
            {
                _saturation = 0.5f;
                _edgeFadeoff = 0.1f;
                _layerProjection = ProjectionMode.Local;
                _faceSharpness = 1f;
                _layerMode = LayerMode.None;
                _xLayer = new DecalLayerProxy();
                _yLayer = new DecalLayerProxy();
                _zLayer = new DecalLayerProxy();
            }

            public void Validate()
            {
                _xLayer.Validate();
                _yLayer.Validate();
                _zLayer.Validate();
            }
        }

        [Serializable]
        public class DecalLayerProxy
        {
            [SerializeField]
            private DecalLayerChannelProxy _channel2;
            [SerializeField]
            private DecalLayerChannelProxy _channel3;
            [SerializeField]
            private DecalLayerChannelProxy _channel4;
            [SerializeField]
            private DecalLayerChannelProxy _channel1;
            [SerializeField]
            [Tooltip("Texture with 4 channels (RGBA) of saturation")]
            private Texture2D _layerMask;
            [SerializeField]
            [Tooltip("Offset the position of the layer mask image")]
            private Vector2 _layerMaskOffset;
            [SerializeField]
            [Tooltip("Scale the layer mask image")]
            private Vector2 _layerMaskScale;
            [SerializeField]
            private bool _editorSectionFoldout;

            public DecalLayerProxy()
            {
                _layerMaskScale = new Vector2(1f, 1f);
                _layerMaskOffset = new Vector2(0f, 0f);

                _channel1 = new DecalLayerChannelProxy(DecalLayerChannelProxy.DecalChannelMode.SimpleRangeRemap);

                _channel2 = new DecalLayerChannelProxy();
                _channel3 = new DecalLayerChannelProxy();
                _channel4 = new DecalLayerChannelProxy();
            }

            public void Validate()
            {
                _channel1.Validate();
                _channel2.Validate();
                _channel3.Validate();
                _channel4.Validate();
            }
        }

        [Serializable]
        public class DecalLayerChannelProxy
        {
            public enum DecalChannelMode
            {
                Disabled,
                Passthrough,
                SimpleRangeRemap,
                AdvancedRangeRemap
            }

            public const float MinSoftness = 0.0196078438f;

            public const float MaxSoftness = 1f;

            [SerializeField]
            [Range(MinSoftness, MaxSoftness)]
            [Tooltip("How steep the transition is from wet to dry")]
            private float _inputRangeSoftness;

            [SerializeField]
            [Range(0f, 1f)]
            [Tooltip("Threshold for which values are considered wet")]
            private float _inputRangeThreshold;

            [SerializeField]
            [Tooltip("How the input texture data is transformed into wetness values")]
            private DecalChannelMode _mode;

            [SerializeField]
            [Tooltip("Limit the minimum and maximum wetness values of the output")]
            private Vector2 _outputRange;

            public DecalLayerChannelProxy() { }

            public DecalLayerChannelProxy(DecalChannelMode mode)
            {
                _mode = mode;
            }

            public void Validate()
            {
                _outputRange = new Vector2(Mathf.Clamp01(_outputRange.x), Mathf.Clamp01(_outputRange.y));
            }
        }

        [SerializeField]
        private DecalSettingsProxy _settings = new DecalSettingsProxy();

        public DecalSettingsProxy Settings => _settings;

        private void OnValidate()
        {
            _settings.Validate();
        }
    }
}
