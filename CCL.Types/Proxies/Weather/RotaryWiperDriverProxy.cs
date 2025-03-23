using System.Linq;
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
            // No rotating transform, cannot wipe.
            if (rotationaryTransforms.Length < 1 || rotationaryTransforms[0] == null) return;

            foreach (var transform in rotationaryTransforms)
            {
                DrawRotatingTransform(transform, maxAngle);
            }

            // No wiper, no cleaning area.
            if (wiper == null || wiper.start == null || wiper.end == null) return;

            DrawWipeArea(rotationaryTransforms[0], stationaryTransform, wiper, maxAngle);
        }

        private static void DrawRotatingTransform(Transform t, float angle)
        {
            if (t == null) return;

            Gizmos.color = Color.blue;

            var pos = t.position;
            var end = pos + Quaternion.AngleAxis(angle / -2.0f, t.forward) * (t.up * -0.5f);
            var dif = end - pos;
            var rot = Quaternion.AngleAxis(angle / Segments, t.forward);

            Gizmos.DrawLine(pos, end);

            for (int i = 0; i < Segments; i++)
            {
                Gizmos.DrawLine(end, end = pos + (dif = rot * dif));
            }

            Gizmos.DrawLine(pos, end);
        }

        private static void DrawWipeArea(Transform rotationary, Transform? stationary, WiperProxy wiper, float maxAngle)
        {
            var isStationary = stationary != null && stationary.GetComponentsInChildren<WiperProxy>().Any(x => x == wiper);
            var pos = rotationary.position;
            var end = stationary != null ? stationary.position : pos + Quaternion.AngleAxis(maxAngle / -2.0f, rotationary.forward) * (rotationary.up * -0.5f);
            var dif = end - pos;
            var rot = Quaternion.AngleAxis(maxAngle / Segments, rotationary.forward);

            var posS = wiper.start.position;
            var posE = wiper.end.position;
            var difS = posS - end;
            var difE = posE - end;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(posS, posE);

            for (int i = 0; i < Segments; i++)
            {
                if (!isStationary)
                {
                    difS = rot * difS;
                    difE = rot * difE;
                }

                end = pos + (dif = rot * dif);
                Gizmos.DrawLine(posS, posS = end + difS);
                Gizmos.DrawLine(posE, posE = end + difE);
            }

            Gizmos.DrawLine(posS, posE);
        }
    }
}
