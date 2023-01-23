using System.Collections;
using UnityEngine;
using UnityEditor;

namespace CCL_GameScripts
{
    public class SimParamsDiesel : SimParamsBase
    {
        public override LocoParamsType SimType => LocoParamsType.DieselElectric;

        [Header("Remote Control")]
        public bool allowRemoteControl = true;

        public override bool AllowRemoteControl => allowRemoteControl;

        [Header("Audio")]
        public bool UseBigDieselAudio = false;
        public override LocoAudioBasis AudioType => UseBigDieselAudio ? LocoAudioBasis.DE6 : LocoAudioBasis.DE2;

        [Header("Throttle")]
        public float ThrottleUpRate = 2f;
        public float ThrottleDownRate = 2f;
        [Tooltip("Percent of max tractive effort available to drivers based on speed")]
        public AnimationCurve TractionTorqueCurve;

        [Header("Heat Management (°C)")]
        [Tooltip("Power multiplier when below Max Power Temp")]
        public float ColdEnginePowerFactor = 0.8f;

        public float PassiveTempLoss = 5.5f;
        public bool HasForwardRadiator = false;
        [Tooltip("Extra cooling per second at max speed")]
        public float ForwardMovementTempLoss = 0;
        public float IdleTempGain = 5;
        public float IdleMaxTemp = 52;

        [Tooltip("Extra heating per second per %(RPM > idle)")]
        public float TempGainPerRpm = 8;
        //public float MinTemp = 30;
        //public float MaxTemp = 120;
        [Tooltip("Temp where cold running penalty drops off")]
        public float MaxPowerTemp = 75;

        [Header("Fuel (L)")]
        public float FuelCapacity = 6000;
        [Tooltip("Base fuel use per second")]
        public float FuelConsumptionBase = 35;
        [Tooltip("Fuel use multiplier at max RPM")]
        public float FuelConsumptionMax = 1;
        [Tooltip("Fuel use multiplier at min RPM")]
        public float FuelConsumptionMin = 0.025f;

        [Tooltip("Damage % where engine begins seizing up")]
        public float PerformanceDropDamageLevel = 0.5f;

        [Header("Lubrication (L)")]
        public float OilCapacity = 500;
        [Tooltip("Oil consumption per second at max RPM")]
        public float OilConsumptionEngineRpm = 1;
        //public float OilConsumptionWheels = 0.12f;

        [Header("Other Equipment")]
        [Tooltip("Air production per second when running")]
        public float AirCompressorRate = 0.8f;
        [Tooltip("Sand valve response speed")]
        public float SandValveSpeed = 10f;

        public SimParamsDiesel()
        {
            ApplyDE6Defaults();
        }

        public void ApplyDE6Defaults()
        {
            RequiredLicense = LocoRequiredLicense.DE6;
            SandCapacity = 200;
            SandMaxFlow = 5;
            tractionTorqueMultiplier = 250000;

            SandCoefficient = 1.5f;
            SlopeCoefficientMultiplier = 10;

            ThrottleUpRate = 2;
            ThrottleDownRate = 2;
            ColdEnginePowerFactor = 0.8f;
            PassiveTempLoss = -5.5f;
            HasForwardRadiator = false;
            ForwardMovementTempLoss = 0;
            IdleTempGain = 5;
            IdleMaxTemp = 52;
            TempGainPerRpm = 8;
            MaxPowerTemp = 75;

            FuelCapacity = 6000;
            FuelConsumptionBase = 35;
            FuelConsumptionMax = 1;
            FuelConsumptionMin = 0.025f;
            PerformanceDropDamageLevel = 0.5f;
            OilCapacity = 500;
            OilConsumptionEngineRpm = 1;

            AirCompressorRate = 0.8f;
            SandValveSpeed = 10;

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

        public void ApplyShunterDefaults()
        {
            RequiredLicense = LocoRequiredLicense.DE2;
            SandCapacity = 200;
            SandMaxFlow = 5;
            tractionTorqueMultiplier = 150000;

            SandCoefficient = 1.5f;
            SlopeCoefficientMultiplier = 10;

            ThrottleUpRate = 2;
            ThrottleDownRate = 2;
            ColdEnginePowerFactor = 0.8f;
            PassiveTempLoss = -4;
            HasForwardRadiator = true;
            ForwardMovementTempLoss = -3;
            IdleTempGain = 5;
            IdleMaxTemp = 52;
            TempGainPerRpm = 12;
            MaxPowerTemp = 75;

            FuelCapacity = 2000;
            FuelConsumptionBase = 15;
            FuelConsumptionMax = 1;
            FuelConsumptionMin = 0.025f;
            PerformanceDropDamageLevel = 0.5f;
            OilCapacity = 100;
            OilConsumptionEngineRpm = 0.3f;

            AirCompressorRate = 0.8f;
            SandValveSpeed = 10;

            BrakePowerCurve =
                new AnimationCurve(
                    new Keyframe(0, 0, 0.2744f, 0.2744f, 0.3333f, 0.0561f),
                    new Keyframe(1, 1, 1.6481f, 1.6481f, 0.0607f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };

            TractionTorqueCurve =
                new AnimationCurve(
                    new Keyframe(0, 1, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(35, 1, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(80, 0, 0, 0, 0.3333f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };

            WheelslipToFrictionModifier =
                new AnimationCurve(
                    new Keyframe(0, 0.4f, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(0.565f, 0.262f, -0.516f, -0.516f, 0.105f, 0.267f),
                    new Keyframe(1, 0.005f, -0.008f, -0.008f, 0.3333f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };
        }
    }
}