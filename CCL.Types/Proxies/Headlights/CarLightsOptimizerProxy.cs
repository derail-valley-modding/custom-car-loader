using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [AddComponentMenu("CCL/Proxies/Headlights/Car Lights Optimizer Proxy")]
    public class CarLightsOptimizerProxy : MonoBehaviour
    {
        public GameObject[] cabLights = new GameObject[0];
        public GameObject[] headLightGlass = new GameObject[0];

        public float cabLightDisableDistance = 50f;
        public float headLightGlassDisableDistance = 50f;
        public float headlightsDisableDistance = 1000f;
        public float glaresDisableDistance = 2000f;
        public float beamsDisableDistance = 500f;

        public HeadlightBeamControllerProxy beamController = null!;
        public float checkPeriod = 0.3f;
        public Transform positionCheckTransform = null!;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(ClosedCabDefaults), "Recommended Closed Cab Values")]
        [MethodButton(nameof(OpenCabDefaults), "Recommended Open Cab Values")]
        private bool _buttons;

        private void ClosedCabDefaults()
        {
            cabLightDisableDistance = 50f;
            headLightGlassDisableDistance = 200f;
            headlightsDisableDistance = 1000f;
            glaresDisableDistance = 2000f;
            beamsDisableDistance = 500f;
        }

        private void OpenCabDefaults()
        {
            cabLightDisableDistance = 100f;
            headLightGlassDisableDistance = 200f;
            headlightsDisableDistance = 1000f;
            glaresDisableDistance = 2000f;
            beamsDisableDistance = 500f;
        }
    }
}
