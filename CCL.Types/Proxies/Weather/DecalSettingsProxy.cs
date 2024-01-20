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
        private bool _affectCeiling = false;
        [SerializeField]
        [Tooltip("The distance from the edge of the shape from which saturation begins to fade.")]
        [Range(0f, 1f)]
        private float _edgeFadeoff = 0.1f;
        [SerializeField]
        [Tooltip("Jitter sample positions of detail layer textures")]
        private bool _enableJitter = false;
        [SerializeField]
        [Tooltip("How sharply the decal fades around faces facing in different directions")]
        [Range(0f, 10f)]
        private float _faceSharpness = 0.0f;
        [SerializeField]
        [Tooltip("The detail layer mode")]
        private LayerMode _layerMode = LayerMode.None;
        [SerializeField]
        [Tooltip("The layer projection mode")]
        private ProjectionMode _layerProjection = ProjectionMode.Local;
        [SerializeField]
        [Tooltip("Determines if the decal projects wetness, or if it drys other wet decals")]
        private WetDecalMode _mode = WetDecalMode.Dry;
        [SerializeField]
        [Tooltip("Maximum jitter in texels to the detail layer sampling coordinates")]
        [Range(0f, 10f)]
        private float _sampleJitter = 0.0f;
        [SerializeField]
        [Tooltip("How wet the decal appears to be")]
        [Range(0f, 1f)]
        private float _saturation = 1.0f;
        [SerializeField]
        [Tooltip("The shape of the decal")]
        private DecalShape _shape = DecalShape.Cube;
        [SerializeField]
        [Tooltip("Per pixel saturation projected down the decal's x axis")]
        private DecalLayerProxy _xLayer = new DecalLayerProxy();
        [SerializeField]
        [Tooltip("Per pixel saturation projected down the decal's y axis")]
        private DecalLayerProxy _yLayer = new DecalLayerProxy();
        [SerializeField]
        [Tooltip("Per pixel saturation projected down the decal's z axis")]
        private DecalLayerProxy _zLayer = new DecalLayerProxy();

        public DecalShape Shape => _shape;
        public float EdgeFadeoff => _edgeFadeoff;
    }
}
