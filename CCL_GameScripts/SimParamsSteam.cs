using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL_GameScripts
{
    public class SimParamsSteam : SimParamsBase
    {
        public override LocoParamsType SimType => LocoParamsType.Steam;

        public override LocoAudioBasis AudioType => LocoAudioBasis.Steam;

        public int ChuffsPerRevolution = 2;

        [Header("Firebox")]
        public float BaseAirMultiplier = 0.1f;
        public float BlowerAirMultiplier = 0.4f;
        public float DraftAirMultiplier = 0.5f;

        [Tooltip("Max fuel usage per second")]
        public float MaxBurnRate = 40;

        public float FireboxCapacity = 90;
        public float AutoFuelMaxPerS = 0;

        [Header("Boiler")]
        public float BoilerWaterCapacityL = 20000;
        public float BoilerMaxPressure = 24;
        [Tooltip("Temperature to steam generation factor")]
        public float VaporizationRate = 0.17f;
        public float SafetyValvePressure = 20;
        public float InjectorMaxFlowLPS = 3000;

        [Header("Steam Chest")]
        [Tooltip("Flow rate of steam to the cylinders at max power")]
        public float SteamPipeMultiplier = 20;

        [Header("Fuel")]
        public SteamFuelType FuelType;
        public bool IsTankLoco = false;
        [Tooltip("(Tank loco) Internal water capacity")]
        public float BunkerWaterCapacity = 0;
        [Tooltip("(Tank loco) Internal fuel capacity")]
        public float BunkerFuelCapacity = 0;

        public bool AllowTenderFuel = true;
        public bool AllowTenderWater = true;

        public bool AllowAnyTenderConnection => AllowTenderFuel || AllowTenderWater;

        [Header("Other Equipment")]
        [Tooltip("Generation = (boiler pressure - reservoir pressure) * rate")]
        public float AirCompressorRate = 0.25f;

        public enum SteamFuelType
        {
            Coal = 21,
            Oil = 10
        }

        public void ApplySH282Defaults()
        {
            // Base
            RequiredLicense = LocoRequiredLicense.Steam;
            SandCapacity = 1200;
            SandMaxFlow = 40;
            tractionTorqueMultiplier = 240000;

            SandCoefficient = 1.5f;
            SlopeCoefficientMultiplier = 10;

            BrakePowerCurve =
                new AnimationCurve(
                    new Keyframe(0, 0, 0.274394065f, 0.274394065f, 0.3333f, 0.05606759f),
                    new Keyframe(1, 1, 1.64810324f, 1.64810324f, 0.0627f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };

            WheelslipToFrictionModifier =
                new AnimationCurve(
                    new Keyframe(0, 0.45f, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(0.8f, 0.35f, -0.224133611f, -0.224133611f, 0.0740166754f, 0.133908823f),
                    new Keyframe(1, 0.005f, -0.008f, -0.008f, 0.3333f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };

            // Steam
            BaseAirMultiplier = 0.1f;
            BlowerAirMultiplier = 0.4f;
            DraftAirMultiplier = 0.5f;
            MaxBurnRate = 40;

            FireboxCapacity = 90;
            AutoFuelMaxPerS = 0;

            BoilerWaterCapacityL = 20000;
            BoilerMaxPressure = 24;
            SafetyValvePressure = 20;
            InjectorMaxFlowLPS = 3000;

            FuelType = SteamFuelType.Coal;
            IsTankLoco = false;
            BunkerWaterCapacity = 0;
            BunkerFuelCapacity = 0;

            AirCompressorRate = 0.25f;
        }
    }
}
