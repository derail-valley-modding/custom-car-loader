using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [AddComponentMenu("CCL/Proxies/Headlights/Volumetric Light Beam Proxy")]
    public class VolumetricLightBeamProxy : MonoBehaviour
    {
        public enum ColorMode
        {
            Flat,
            Gradient
        }

        private enum LocomotiveDefault
        {
            DE2,
            DE6,
            DH4,
            DM3,
            DM1U,
            BE2,
            S060,
            S282
        }

        private enum HeadlightType
        {
            LowBeam,
            HighBeam
        }

        public bool colorFromLight = true;
        public ColorMode colorMode = ColorMode.Flat;
        [ColorUsage(false, true)]
        public Color color = Color.white;
        public Gradient colorGradient = null!;

        public bool intensityFromLight = true;
        public bool intensityModeAdvanced = true;
        public float intensityInside = 0.2f;
        public float intensityOutside = 0.03f;

        public bool spotAngleFromLight = true;
        [Range(0.1f, 179.9f)]
        public float spotAngle = 35f;

        [Range(0f, 1f)]
        public float glareFrontal = 0.8f;

        [SerializeField]
        private LocomotiveDefault _locomotiveDefault;
        [SerializeField]
        private HeadlightType _headlightType;
        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(ApplyLocoDefaults), "Apply Default Values")]
        private bool _renderButton;

        private void ApplyLocoDefaults()
        {
            colorFromLight = true;
            intensityFromLight = true;
            intensityModeAdvanced = true;
            spotAngleFromLight = true;

            switch (_locomotiveDefault)
            {
                case LocomotiveDefault.DE2:
                    switch (_headlightType)
                    {
                        case HeadlightType.LowBeam:
                            intensityInside = 0.1f;
                            intensityOutside = 0.01f;
                            spotAngle = 25.0f;
                            glareFrontal = 0.3f;
                            break;
                        case HeadlightType.HighBeam:
                            intensityInside = 0.2f;
                            intensityOutside = 0.03f;
                            spotAngle = 20.0f;
                            glareFrontal = 0.8f;
                            break;
                        default:
                            break;
                    }
                    break;

                case LocomotiveDefault.DE6:
                case LocomotiveDefault.DH4:
                case LocomotiveDefault.DM3:
                    intensityInside = 0.42f;
                    intensityOutside = 0.03f;
                    spotAngle = 25.0f;
                    glareFrontal = 0.308f;
                    break;

                case LocomotiveDefault.DM1U:
                    intensityInside = 0.42f;
                    intensityOutside = 0.03f;
                    spotAngle = 25.0f;
                    switch (_headlightType)
                    {
                        case HeadlightType.LowBeam:
                            glareFrontal = 0.3f;
                            break;
                        case HeadlightType.HighBeam:
                            glareFrontal = 0.8f;
                            break;
                        default:
                            break;
                    }
                    break;

                case LocomotiveDefault.BE2:
                    intensityInside = 0.2f;
                    intensityOutside = 0.03f;
                    spotAngle = 50.0f;
                    glareFrontal = 0.8f;
                    break;

                case LocomotiveDefault.S060:
                case LocomotiveDefault.S282:
                    intensityInside = 0.0f;
                    intensityOutside = 0.0f;
                    switch (_headlightType)
                    {
                        case HeadlightType.LowBeam:
                            spotAngle = 20.0f;
                            glareFrontal = 0.8f;
                            break;
                        case HeadlightType.HighBeam:
                            spotAngle = 25.0f;
                            glareFrontal = 0.3f;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            switch (_headlightType)
            {
                case HeadlightType.LowBeam:
                    transform.localEulerAngles = new Vector3(8.0f, 0.0f, 0.0f);
                    break;
                case HeadlightType.HighBeam:
                    transform.localEulerAngles = new Vector3(1.0f, 0.0f, 0.0f);
                    break;
                default:
                    break;
            }
        }

        private void OnValidate()
        {
            if (TryGetComponent<Light>(out var light) && light.type == LightType.Spot)
            {
                AssignPropertiesFromSpotLight(light);
            }

            ClampProperties();
        }

        private void AssignPropertiesFromSpotLight(Light lightSpot)
        {
            if (intensityFromLight)
            {
                intensityModeAdvanced = false;
            }

            if (spotAngleFromLight)
            {
                spotAngle = lightSpot.spotAngle;
            }

            if (colorFromLight)
            {
                colorMode = ColorMode.Flat;
                color = lightSpot.color;
            }
        }

        private void ClampProperties()
        {
            intensityInside = Mathf.Max(intensityInside, 0f);
            intensityOutside = Mathf.Max(intensityOutside, 0f);
            spotAngle = Mathf.Clamp(spotAngle, 0.1f, 179.9f);
            glareFrontal = Mathf.Clamp01(glareFrontal);
        }
    }
}
