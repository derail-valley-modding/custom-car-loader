using System.Collections;
using UnityEngine;
using UnityEditor;

namespace CCL_GameScripts
{
    public class SimParamsDiesel : SimParamsBase
    {
        public override LocoParamsType SimType => LocoParamsType.DieselElectric;

        [Header("Throttle")]
        public float ThrottleUpRate = 2f;
        public float ThrottleDownRate = 2f;

        [Header("Heat Management (°C)")]
        public float ColdEnginePowerFactor = 0.8f;

        public float PassiveTempLoss = 5.5f;
        public bool HasForwardRadiator = false;
        public float ForwardMovementTempLoss = 0;
        public float IdleTempGain = 5;
        public float IdleMaxTemp = 52;

        public float TempGainPerRpm = 8;
        //public float MinTemp = 30;
        //public float MaxTemp = 120;
        public float MaxPowerTemp = 75;

        [Header("Fuel (L)")]
        public float FuelCapacity = 6000;
        public float FuelConsumptionBase = 35;
        public float FuelConsumptionMax = 1;
        public float FuelConsumptionMin = 0.025f;

        public float PerformanceDropDamageLevel = 0.5f;

        [Header("Lubrication (L)")]
        public float OilCapacity = 500;
        public float OilConsumptionEngineRpm = 1;
        //public float OilConsumptionWheels = 0.12f;

        [Header("Other Equipment")]
        public float AirCompressorRate = 0.8f;

        public SimParamsDiesel()
        {
            ApplyDE6Defaults();
        }

        public void ApplyDE6Defaults()
        {
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