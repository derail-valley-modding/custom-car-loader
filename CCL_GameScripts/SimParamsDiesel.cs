using System.Collections;
using UnityEngine;

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
        public float IdleTempGain = 5;
        public float IdleMaxTemp = 52;

        public float TempGainPerRpm = 8;
        public float MinTemp = 30;
        public float MaxTemp = 120;
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
    }
}