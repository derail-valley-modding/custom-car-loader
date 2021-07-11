using System.Collections;
using UnityEngine;
using UnityEditor;

namespace CCL_GameScripts
{
    public class SimParamsDiesel : SimParamsBase
    {
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
        }
    }
}