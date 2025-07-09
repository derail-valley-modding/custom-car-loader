using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Proxies.VerletRope
{
    [Serializable]
    public class RopeParams
    {
        public float ropeLength = 1.0f;
        public int numPoints = 14;
        [JsonIgnore]
        public Vector3 gravity = new Vector3(0f, -0.2f, 0f);
        public float friction = 0.96f;
        public float floorLevel = float.NegativeInfinity;
        public float floorFriction = 0.5f;
        public float bendingCorrectionFactor = 0.35f;
        public float floorBendingScale = 0.1f;
        public int solverIterations = 100;
        [JsonIgnore]
        public Transform receiveForcesFrom = null!;

        public RopeParams() { }
    }
}
