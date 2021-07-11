using System.Collections;
using UnityEngine;

namespace CCL_GameScripts
{
    public abstract class SimParamsBase : MonoBehaviour
    {
        // default values from diesel
        [Header("Basic")]
        public float MaxSpeed = 120f;
        public float SandCapacity = 200f;
        public float SandValveSpeed = 10f;
        public float SandMaxFlow = 5f;
    }
}