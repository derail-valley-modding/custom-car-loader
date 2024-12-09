using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class RotaryWiperDriverProxy : WiperDriverProxy
    {
        public Transform[] rotationaryTransforms;
        public Transform stationaryTransform;
        public float maxAngle;
        public AnimationCurve speedCurve;
    }
}
