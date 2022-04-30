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
        [Tooltip("Max kg/s to sanders")]
        public float SandMaxFlow = 5f;

        [Header("Physics Curves")]
        public AnimationCurve BrakePowerCurve;
        [Tooltip("Maximum (starting) tractive effort (N)")]
        public float tractionTorqueMultiplier = 250000;

        [Header("Drivers")]
        [Tooltip("Simulate infinite friction with track")]
        public bool PreventWheelslip = false;
        [Tooltip("Friction multiplier at 100 % sand application")]
        public float SandCoefficient = 1.5f;
        [Tooltip("Multiplier of track slope for adhesion calcs")]
        public float SlopeCoefficientMultiplier = 2f;
        [Tooltip("Relationship of % wheelslip -> friction coefficient")]
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