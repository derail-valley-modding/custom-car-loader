using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types
{
    public static class MathHelper
    {
        public static float Bezier(float p0, float p1, float p2, float p3, float t)
        {
            float num = 1f - t;
            return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
        }

        public static Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float num = 1f - t;
            return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
        }

        public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float num = 1f - t;
            return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
        }

        public static float BezierDerivative1(float p0, float p1, float p2, float p3, float t)
        {
            float num = 1f - t;
            return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
        }

        public static Vector2 BezierDerivative1(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float num = 1f - t;
            return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
        }

        public static Vector3 BezierDerivative1(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float num = 1f - t;
            return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
        }

        public static void BezierType2(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, out Vector3 point, out Vector3 normal)
        {
            point = Bezier(p0, p1, p2, p3, t);

            var nflat = BezierDerivative1(new Vector2(p0.x, p0.z), new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z), new Vector2(p3.x, p3.z), t);
            var nHeight = BezierDerivative1(p0.y, p1.y, p2.y, p3.y, t);

            normal = new Vector3(nflat.x, nHeight, nflat.y);
        }

        public static float DistanceSqr(Vector2 a, Vector2 b)
        {
            return (b - a).sqrMagnitude;
        }
    }
}
