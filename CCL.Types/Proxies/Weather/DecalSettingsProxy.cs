using System;
using UnityEngine;
using static CCL.Types.Proxies.Weather.WetDecalEnums;

namespace CCL.Types.Proxies.Weather
{
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
            _affectCeiling = false;
            _enableJitter = false;

            _edgeFadeoff = 0.1f;
            _faceSharpness = 0.0f;
            _saturation = 1.0f;

            _layerMode = LayerMode.None;
            _layerProjection = ProjectionMode.Local;
            _mode = WetDecalMode.Dry;
            _shape = DecalShape.Cube;

            _xLayer = new DecalLayerProxy();
            _yLayer = new DecalLayerProxy();
            _zLayer = new DecalLayerProxy();
        }
    }
}
