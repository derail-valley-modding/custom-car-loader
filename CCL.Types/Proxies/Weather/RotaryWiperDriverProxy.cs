using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class RotaryWiperDriverProxy : WiperDriverProxy
    {
        private const int Segments = 10;

        public Transform[] rotationaryTransforms = new Transform[0];
        public Transform stationaryTransform = null!;
        public float maxAngle;
        public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private void OnDrawGizmos()
        {
            if (wiper == null) return;

            Gizmos.color = Color.blue;
            var pos = transform.position;
            var end = wiper.transform.position;
            var dif = end - pos;
            var rot = Quaternion.AngleAxis(maxAngle / Segments, transform.forward);

            Gizmos.DrawLine(pos, end);

            for (int i = 0; i < Segments; i++)
            {
                Gizmos.DrawLine(end, end = pos + (dif = rot * dif));
            }

            Gizmos.DrawLine(pos, end);
        }
    }
}
