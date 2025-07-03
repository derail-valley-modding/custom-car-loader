using CCL.Types.Proxies.VFX;
using UnityEngine;
using UnityEngine.Rendering;
using static CCL.Types.Proxies.VFX.LightShadowQualityProxy;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Copiers/Copy Vanilla Light")]
    [RequireComponent(typeof(Light))]
    public class CopyVanillaLight : MonoBehaviour
    {
        public VanillaLight LightToCopy;

        private void Start()
        {
            Debug.Log($"{nameof(CopyVanillaLight)} was not deleted from '{name}'!");
            Destroy(this);
        }

        public void ApplyProperties()
        {
            ApplyProperties(GetComponent<Light>(), LightToCopy);
        }

        public static void ApplyProperties(Light light, VanillaLight source)
        {
            switch (source)
            {
                case VanillaLight.LocoHeadlightsHigh:
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

                    AddQuality(light, LightShadowQualitySettings.HeadlightConfig);
                    break;
                case VanillaLight.LocoHeadlightsLow:
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

                    AddQuality(light, LightShadowQualitySettings.HeadlightConfig);
                    break;

                case VanillaLight.DE2Cablights:
                case VanillaLight.DE6Cablights:
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

                    RemoveQuality(light);
                    break;
                case VanillaLight.DH4Cablights:
                case VanillaLight.DM3Cablights:
                    light.type = LightType.Point;
                    light.range = 2.5f;
                    light.color = new Color32(233, 218, 195, 255);

                    // Mode missing...
                    light.intensity = 2.5f;

                    light.shadows = LightShadows.None;

                    light.cullingMask = ~0;

                    RemoveQuality(light);
                    break;
                case VanillaLight.BE2Cablights:
                    light.type = LightType.Point;
                    light.range = 2.5f;
                    light.color = new Color32(233, 218, 195, 255);

                    // Mode missing...
                    light.intensity = 2.5f;

                    light.shadows = LightShadows.None;

                    light.cullingMask = ~0;

                    AddQuality(light, new[]
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
                            shadows = LightShadows.Soft,
                            resolution = LightShadowResolution.Medium
                        }
                    });
                    break;
                case VanillaLight.S060Cablights:
                    light.type = LightType.Point;
                    light.range = 2.5f;
                    light.color = new Color32(250, 194, 107, 255);

                    // Mode missing...
                    light.intensity = 2.5f;

                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 1.0f;
                    light.shadowResolution = LightShadowResolution.Low;
                    light.shadowBias = 0.05f;
                    light.shadowNormalBias = 0.4f;
                    light.shadowNearPlane = 0.2f;

                    light.cullingMask = ~0;

                    RemoveQuality(light);
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

                    AddQuality(light, LightShadowQualitySettings.DefaultConfig);
                    break;

                case VanillaLight.S282GearLight:
                    light.type = LightType.Spot;
                    light.range = 8.0f;
                    light.spotAngle = 35.9f;
                    light.color = new Color32(238, 204, 152, 255);

                    // Mode missing...
                    light.intensity = 5.0f;

                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 1.0f;
                    light.shadowResolution = LightShadowResolution.Low;
                    light.shadowBias = 0.05f;
                    light.shadowNormalBias = 0.4f;
                    light.shadowNearPlane = 0.2f;

                    light.cullingMask = ~0;

                    RemoveQuality(light);
                    break;

                default:
                    Debug.LogError("Unsupported light source!");
                    break;
            }
        }

        private static void AddQuality(Light light, LightShadowQualitySettings[] config)
        {
            if (!light.TryGetComponent(out LightShadowQualityProxy quality))
            {
                quality = light.gameObject.AddComponent<LightShadowQualityProxy>();
            }

            quality.settings = config;
        }

        private static void RemoveQuality(Light light)
        {
            if (light.TryGetComponent(out LightShadowQualityProxy quality))
            {
                DestroyImmediate(quality);
            }
        }
    }

    public enum VanillaLight
    {
        LocoHeadlightsHigh = 0,
        LocoHeadlightsLow,

        DE2Cablights = 100,
        DE6Cablights,
        DH4Cablights,
        DM3Cablights,
        BE2Cablights,
        S060Cablights,

        S060Fire = 200,
        S282Fire,

        S282GearLight = 250
    }
}
