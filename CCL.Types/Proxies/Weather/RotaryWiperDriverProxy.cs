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

            var pos = transform.position;
            var end = wiper.transform.position;
            var dif = end - pos;
            var rot = Quaternion.AngleAxis(maxAngle / Segments, transform.forward);

            Vector3? difS = null;
            Vector3? difE = null;
            if (wiper.start != null) difS = wiper.start.position - end;
            if (wiper.end != null) difE = wiper.end.position - end;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, end);

            if (difS.HasValue && difE.HasValue)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(end + difS.Value, end + difE.Value);
            }

            for (int i = 0; i < Segments; i++)
            {
                var temp = pos + (dif = rot * dif);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(end, temp);

                if (difS.HasValue)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(end + difS.Value, temp + difS.Value);
                }

                if (difE.HasValue)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(end + difE.Value, temp + difE.Value);
                }

                end = temp;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, end);

            if (difS.HasValue && difE.HasValue)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(end + difS.Value, end + difE.Value);
            }
        }
    }
}
