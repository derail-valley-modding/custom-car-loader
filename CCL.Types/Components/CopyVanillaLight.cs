using CCL.Types.Proxies.VFX;
using UnityEngine;
using UnityEngine.Rendering;
using static CCL.Types.Proxies.VFX.LightShadowQualityProxy;

namespace CCL.Types.Components
{
    [RequireComponent(typeof(Light))]
    public class CopyVanillaLight : MonoBehaviour
    {
        public VanillaLight LightToCopy;

        private void Start()
        {
            Debug.Log($"CopyVanillaLight was not deleted from '{name}'!");
            Destroy(this);
        }

        public void ApplyProperties()
        {
            ApplyProperties(GetComponent<Light>(), LightToCopy);
        }

        public static void ApplyProperties(Light light, VanillaLight source)
        {
            LightShadowQualityProxy quality = null!;

            switch (source)
            {
                case VanillaLight.DE2HeadlightsHigh:
                    light.type = LightType.Spot;
                    light.range = 250.0f;
                    light.spotAngle = 20.0f;
                    light.color = new Color32(241, 229, 210, 255);

                    // Mode missing...
                    light.intensity = 3.0f;

                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 1.0f;
                    light.shadowResolution = LightShadowResolution.FromQualitySettings;
                    light.shadowBias = 0.05f;
                    light.shadowNormalBias = 0.4f;
                    light.shadowNearPlane = 0.2f;

                    light.cullingMask = ~0;

                    if (!light.TryGetComponent(out quality))
                    {
                        quality = light.gameObject.AddComponent<LightShadowQualityProxy>();
                    }

                    quality.settings = LightShadowQualitySettings.HeadlightConfig;
                    break;
                case VanillaLight.DE2HeadlightsLow:
                    light.type = LightType.Spot;
                    light.range = 120.0f;
                    light.spotAngle = 32.0f;
                    light.color = new Color32(241, 229, 210, 255);

                    // Mode missing...
                    light.intensity = 3.0f;

                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 1.0f;
                    light.shadowResolution = LightShadowResolution.FromQualitySettings;
                    light.shadowBias = 0.05f;
                    light.shadowNormalBias = 0.4f;
                    light.shadowNearPlane = 0.2f;

                    light.cullingMask = ~0;

                    if (!light.TryGetComponent(out quality))
                    {
                        quality = light.gameObject.AddComponent<LightShadowQualityProxy>();
                    }

                    quality.settings = LightShadowQualitySettings.HeadlightConfig;
                    break;

                case VanillaLight.DE2Cablights:
                    light.type = LightType.Point;
                    light.range = 2.5f;
                    light.color = new Color32(229, 208, 176, 255);

                    // Mode missing...
                    light.intensity = 2.5f;

                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 1.0f;
                    light.shadowResolution = LightShadowResolution.Low;
                    light.shadowBias = 0.05f;
                    light.shadowNormalBias = 0.4f;
                    light.shadowNearPlane = 0.2f;

                    light.cullingMask = ~0;
                    break;

                case VanillaLight.S060Fire:
                case VanillaLight.S282Fire:
                    light.type = LightType.Point;
                    light.range = 3.0f;
                    light.color = new Color32(255, 124, 50, 255);

                    // Mode missing...
                    light.intensity = 0.0f;

                    light.shadows = LightShadows.Hard;
                    light.shadowStrength = 1.0f;
                    light.shadowResolution = LightShadowResolution.FromQualitySettings;
                    light.shadowBias = 0.05f;
                    light.shadowNormalBias = 0.4f;
                    light.shadowNearPlane = 0.1f;

                    light.cullingMask = ~0;

                    if (!light.TryGetComponent(out quality))
                    {
                        quality = light.gameObject.AddComponent<LightShadowQualityProxy>();
                    }

                    quality.settings = LightShadowQualitySettings.DefaultConfig;
                    break;

                default:
                    Debug.LogError("Unsupported light source!");
                    break;
            }
        }
    }

    public enum VanillaLight
    {
        DE2HeadlightsHigh = 0,
        DE2HeadlightsLow,

        DE2Cablights = 100,

        S060Fire = 200,
        S282Fire,
    }
}
