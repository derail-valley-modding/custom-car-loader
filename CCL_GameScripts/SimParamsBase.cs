using System.Collections;
using UnityEngine;

namespace CCL_GameScripts
{
    public enum LocoParamsType
    {
        None = 0,
        DieselElectric = 1,
        Steam = 2
    }

    public abstract class SimParamsBase : MonoBehaviour
    {
        [HideInInspector]
        public abstract LocoParamsType SimType { get; }

        // default values from diesel
        [Header("Basic")]
        public float MaxSpeed = 120f;
        public float SandCapacity = 200f;
        public float SandValveSpeed = 10f;
        public float SandMaxFlow = 5f;

        [Header("Physics Curves")]
        public AnimationCurve BrakePowerCurve;
        public AnimationCurve TractionTorqueCurve;

        public SimParamsBase()
        {
            // DE6 defaults
            BrakePowerCurve =
                new AnimationCurve(
                    new Keyframe(0, 0, 0.3512f, 0.3512f, 0.3333f, 0.1033f),
                    new Keyframe(1, 1, 1.7677f, 1.7677f, 0.0627f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };

            TractionTorqueCurve =
                new AnimationCurve(
                    new Keyframe(0, 1, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(15, 1, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(98.8f, 0.4813f, -0.0087f, -0.0087f, 0.1278f, 0.3589f),
                    new Keyframe(120, 0, -0.0247f, -0.0247f, 0.2221f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };
        }
    }
}