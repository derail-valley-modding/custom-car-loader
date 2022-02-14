using System.Collections;
using UnityEngine;

namespace CCL_GameScripts
{
    public enum LocoParamsType
    {
        None = 0,
        DieselElectric = 1,
        Steam = 2,
        Tender = 3,
        Caboose = 4
    }

    public static class LocoParamsExtensions
    {
        public static bool IsLocomotiveType(this LocoParamsType paramsType)
        {
            return
                (paramsType == LocoParamsType.DieselElectric) ||
                (paramsType == LocoParamsType.Steam);
        }
    }

    public enum LocoRequiredLicense
    {
        None = 0,
        DE2 = 1,
        DE6 = 2,
        Steam = 3,
    }

    public enum LocoAudioBasis
    {
        None = 0,
        DE2 = 1,
        DE6 = 2,
        Steam = 3,
    }

    [RequireComponent(typeof(TrainCarSetup))]
    public abstract class SimParamsBase : MonoBehaviour
    {
        [HideInInspector]
        public abstract LocoParamsType SimType { get; }

        [HideInInspector]
        public abstract LocoAudioBasis AudioType { get; }

        // default values from diesel
        [Header("Basic")]
        public LocoRequiredLicense RequiredLicense = LocoRequiredLicense.None;
        public float MaxSpeed = 120f;
        public float SandCapacity = 200f;
        public float SandValveSpeed = 10f;
        public float SandMaxFlow = 5f;

        [Header("Physics Curves")]
        public AnimationCurve BrakePowerCurve;
        public AnimationCurve TractionTorqueCurve;
        public float tractionTorqueMultiplier = 250000;

        [Header("Drivers")]
        public bool PreventWheelslip = false;
        public float FrictionCoefficient = 0.25f;
        [Range(1f, 10f)]
        public float SandCoefficient = 1.5f;
        public float SlopeCoefficientMultiplier = 2f;
        public AnimationCurve WheelslipToFrictionModifier;

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

            WheelslipToFrictionModifier =
                new AnimationCurve(
                    new Keyframe(0, 0.4f, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(0.802f, 0.271f, -0.516f, -0.516f, 0.105f, 0.267f),
                    new Keyframe(1, 0.005f, -0.008f, -0.008f, 0.3333f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };
        }
    }
}