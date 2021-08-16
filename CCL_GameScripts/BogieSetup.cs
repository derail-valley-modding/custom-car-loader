using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL_GameScripts
{
    public class BogieSetup : MonoBehaviour
    {
        [Header("Axles")]
        [Tooltip("Number of axles, used for calculating joint sounds")]
        [Range(0, 10)]
        public int AxleCount = 2;

        [Tooltip("Distance between 2 consecutive axles on a bogie")]
        [Range(0, 10)]
        public float AxleSeparation = 1.5f;

        [Header("Physics")]
        [Range(0, 100000)]
        public float BrakingForcePerBar = 10000f;

        [Range(0, 0.02f)]
        public float RollingResistanceCoefficient = 0.004f;

        public JSONObject GetJSON()
        {
            var repr = new JSONObject();
            repr.AddField("axleCount", AxleCount);
            repr.AddField("axleSeparation", AxleSeparation);
            repr.AddField("brakingForcePerBar", BrakingForcePerBar);
            repr.AddField("rollingResistance", RollingResistanceCoefficient);
            return repr;
        }
    }
}