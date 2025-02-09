using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public class CarLightsOptimizerProxy : MonoBehaviour
    {
        public GameObject[] cabLights;
        public GameObject[] headLightGlass;

        public float cabLightDisableDistance = 50f;
        public float headLightGlassDisableDistance = 50f;
        public float headlightsDisableDistance = 1000f;
        public float glaresDisableDistance = 2000f;
        public float beamsDisableDistance = 500f;

        public HeadlightBeamControllerProxy beamController;
        public float checkPeriod = 0.3f;
        public Transform positionCheckTransform;
    }
}
