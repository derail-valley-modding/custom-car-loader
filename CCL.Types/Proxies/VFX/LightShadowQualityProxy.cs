using CCL.Types.Json;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCL.Types.Proxies.VFX
{
    [AddComponentMenu("CCL/Proxies/VFX/Light Shadow Quality Proxy")]
    [RequireComponent(typeof(Light)), DisallowMultipleComponent]
    public class LightShadowQualityProxy : MonoBehaviour, ICustomSerialized
    {
        public LightShadowQualitySettings[] settings = new LightShadowQualitySettings[0];

        private string? _json = null;

        public void AfterImport()
        {
            if (!string.IsNullOrEmpty(_json))
            {
                settings = JSONObject.FromJson<LightShadowQualitySettings[]>(_json);
            }

            OnValidate();
        }

        public void OnValidate()
        {
            if (settings == null || settings.Length == 0)
            {
                settings = LightShadowQualitySettings.DefaultConfig;
            }

            _json = JSONObject.ToJson(settings);
        }

        [Serializable]
        public class LightShadowQualitySettings
        {
            [Tooltip("Quality Index setting at which light inherits these values. Always takes highest one that is less or equal than setting.")]
            [Range(0, 6)]
            public int qualityIndex;
            public LightShadows shadows;
            public LightShadowResolution resolution;

            /// <summary>
            /// Default values.
            /// </summary>
            public static LightShadowQualitySettings[] DefaultConfig => new[]
            {
                new LightShadowQualitySettings
                {
                    qualityIndex = 0,
                    shadows = LightShadows.None,
                    resolution = LightShadowResolution.Low
                },
                new LightShadowQualitySettings
                {
                    qualityIndex = 6,
                    shadows = LightShadows.Hard,
                    resolution = LightShadowResolution.Medium
                }
            };
            /// <summary>
            /// Used on headlights.
            /// </summary>
            public static LightShadowQualitySettings[] HeadlightConfig => new[]
            {
                new LightShadowQualitySettings
                {
                    qualityIndex = 0,
                    shadows = LightShadows.None,
                    resolution = LightShadowResolution.Low
                },
                new LightShadowQualitySettings
                {
                    qualityIndex = 5,
                    shadows = LightShadows.Soft,
                    resolution = LightShadowResolution.Medium
                }
            };
        }
    }
}
