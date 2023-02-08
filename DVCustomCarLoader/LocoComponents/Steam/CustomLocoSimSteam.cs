using CCL_GameScripts;
using DV.JObjectExtstensions;
using DV.ServicePenalty;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomLocoSimSteam : CustomLocoSimulation<SimParamsSteam, CustomDamageControllerSteam>
    {
        public ResourceType FuelType => GetFuelResourceType(simParams);

        [NonSerialized]
        public float fuelConsumptionRate;
        [NonSerialized]
        public float maxFuelConsumptionRate;
        [NonSerialized]
        public float pressureLeakMultiplier = 1f;

        [NonSerialized]
        public float injectorFlowRate;
        [NonSerialized]
        public float stokerFlowRate;

        private float tempRange;

        #region SimComponents

        // Firebox
        public SimComponent fireOn = new SimComponent("FireOn", 0f, 1f, 1f, 0f);
        public SimComponent fireDoorOpen = new SimComponent("FireDoorOpen", 0f, 1f, 0.25f, 0);
        public SimComponent temperature = new SimComponent("Temperature", 25f, 1350f, 1f, 0f);
        public SimComponent damper = new SimComponent("Damper", 0f, 1f, 0.1f, 1f);
        public SimComponent blower = new SimComponent("Blower", 0f, 1f, 0.1f, 0f);

        public SimComponent fireboxFuel;
        public SimComponent autoFuelFeed;

        // Boiler
        public SimComponent steamReleaser = new SimComponent("SteamReleaser", 0f, 1f, 0.25f, 0f);
        public SimComponent safetyPressureValve = new SimComponent("SafetyValve", 0f, 1f, 1f, 0f);
        public SimComponent injector = new SimComponent("Injector", 0f, 1f, 0.25f, 0f);
        public SimComponent waterDump = new SimComponent("WaterDumper", 0f, 1f, 0.25f, 0f);

        public SimComponent boilerWater;
        public SimComponent boilerPressure;

        // Steam
        public SimComponent regulator = new SimComponent("Regulator", 0f, 1f, 0.1f, 0f);
        public SimComponent cutoff = new SimComponent("Cutoff", 0f, 1f, 0.01f, 0.5f);
        public SimComponent exhaust = new SimComponent("Exhaust", 0, 100);

        public SimComponent power = new SimComponent("Power", 0f, 1, 1f, 0f);
        public SimComponent speed;

        public SimComponent sandFlow = new SimComponent("SandFlow", 0f, 1f, 0.25f, 0f);
        public SimComponent sandValve = new SimComponent("SandValve", 0f, 1f, 0.25f, 0f);

        public SimComponent dynamoValve = new SimComponent("DynamoValve", 0, 1, 0.1f, 0);
        public SimComponent dynamoSpeed = new SimComponent("DynamoSpeed", 0, 1, 0.1f, 0);

        public SimComponent sand;

        public SimComponent tenderWater;
        public SimComponent tenderFuel;

        private SimComponent bunkerWater;
        private SimComponent bunkerFuel;

        private const string TOTAL_FUEL_CONSUMED_KEY = "fuelConsumed";

        private float tractionConstant;
        public float GetMaxTractiveEffort(float velocity)
        {
            // TE = TEmax - TEmax/Vmax^1.5 * v^1.5
            return simParams.tractionTorqueMultiplier - tractionConstant * Mathf.Pow(velocity, 1.5f);
        }

        public void RecalculateTractionConstant()
        {
            tractionConstant = simParams.tractionTorqueMultiplier / Mathf.Pow(simParams.MaxSpeed, 1.5f);
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            maxFuelConsumptionRate = (simParams.BaseAirMultiplier + simParams.DraftAirMultiplier + simParams.BlowerAirMultiplier) * simParams.MaxBurnRate;
            tempRange = temperature.max - temperature.min;
            RecalculateTractionConstant();
        }

        #region Component Manipulation

        protected override void InitComponents()
        {
            base.InitComponents();

            speed = new SimComponent("Speed", 0f, simParams.MaxSpeed, 1f, 0f);

            fireboxFuel = new SimComponent("FireboxFuel", 0, simParams.FireboxCapacity, 15, 0);
            autoFuelFeed = new SimComponent("Stoker", 0, simParams.AutoFuelMaxPerS, 15, 0);

            boilerWater = new SimComponent("BoilerWater", 0, simParams.BoilerWaterCapacityL, 2000f, simParams.BoilerWaterCapacityL * 0.7f);
            boilerPressure = new SimComponent("BoilerPressure", 0, simParams.BoilerMaxPressure, 1, 0);
            sand = new SimComponent("Sand", 0, simParams.SandCapacity, simParams.SandCapacity * 0.2f, simParams.SandCapacity);

            bunkerWater = new SimComponent("BunkerWater", 0, simParams.BunkerWaterCapacity, 15, simParams.BunkerWaterCapacity);
            bunkerFuel = new SimComponent("BunkerFuel", 0, simParams.BunkerFuelCapacity, 15, simParams.BunkerFuelCapacity);

            SetInternalFuelComponents();

            components = new SimComponent[]
            {
                fireOn,
                fireDoorOpen,
                temperature,
                fireboxFuel,
                boilerWater,
                tenderWater,
                tenderFuel,
                boilerPressure,
                steamReleaser,
                safetyPressureValve,
                injector,
                waterDump,
                regulator,
                cutoff,
                damper,
                blower,
                power,
                speed,
                sand,
                sandFlow,
                sandValve,
                autoFuelFeed,
                exhaust,
                dynamoValve,
                dynamoSpeed,
            };
        }

        public virtual void SetInternalFuelComponents()
        {
            tenderWater = bunkerWater;
            tenderFuel = bunkerFuel;
        }

        public virtual void UpdateComponentReferences()
        {
            components[5] = tenderWater;
            components[6] = tenderFuel;
        }

        public override JObject GetComponentsSaveData()
        {
            JObject jobject = new JObject();
            SimComponent.SaveComponentState(fireOn, jobject);
            SimComponent.SaveComponentState(temperature, jobject);
            SimComponent.SaveComponentState(fireboxFuel, jobject);
            SimComponent.SaveComponentState(boilerWater, jobject);
            SimComponent.SaveComponentState(boilerPressure, jobject);
            SimComponent.SaveComponentState(sand, jobject);
            SimComponent.SaveComponentState(bunkerWater, jobject);
            SimComponent.SaveComponentState(bunkerFuel, jobject);
            jobject.SetFloat(TOTAL_FUEL_CONSUMED_KEY, TotalFuelConsumed);
            return jobject;
        }

        public override void LoadComponentsState(JObject stateData)
        {
            SimComponent.LoadComponentState(fireOn, stateData);
            SimComponent.LoadComponentState(temperature, stateData);
            SimComponent.LoadComponentState(fireboxFuel, stateData);
            SimComponent.LoadComponentState(boilerWater, stateData);
            SimComponent.LoadComponentState(boilerPressure, stateData);
            SimComponent.LoadComponentState(sand, stateData);
            SimComponent.LoadComponentState(bunkerWater, stateData);
            SimComponent.LoadComponentState(bunkerFuel, stateData);

            float? fuelConsumed = stateData.GetFloat(TOTAL_FUEL_CONSUMED_KEY);
            if (fuelConsumed != null)
            {
                TotalFuelConsumed = fuelConsumed.Value;
                return;
            }
            Main.Error($"No load data for {TOTAL_FUEL_CONSUMED_KEY} found!");
        }

        #endregion

        public override void ResetRefillableSimulationParams()
        {
            sand.SetValue(sand.max);
            bunkerWater.SetValue(bunkerWater.max);
            bunkerFuel.SetValue(bunkerFuel.max);
        }

        public void DebugForceSteamUp()
        {
            boilerPressure.SetValue(boilerPressure.max);
            boilerWater.SetValue(boilerWater.max);
            fireboxFuel.SetValue(fireboxFuel.max);
            fireOn.SetValue(fireOn.max);
            temperature.SetValue(temperature.max);
        }

        public void AddCoalChunk()
        {
            if (GetFuelResourceType(simParams) != ResourceType.Coal) return;

            tenderFuel.PassValueTo(fireboxFuel, SteamLocoSimulation.COAL_PIECE_KG);
            if (fireOn.value == 0 && temperature.value > SteamLocoSimulation.MIN_TEMP_WHEN_FIRE_ON_C)
            {
                fireOn.SetValue(1);
            }
        }

        public void TryExtinguishFire()
        {
            if (temperature.value <= 150)
            {
                fireOn.SetValue(0);
            }
        }

        //================================================================================
        #region Simulation Ticks

        protected override void SimulateTick(float delta)
        {
            InitNextValues();
            SimulateFire(delta);
            SimulateWater(delta);
            SimulateSteam(delta);
            SimulateCylinder(delta);
            SimulateSand(delta);
            SetValuesToNextValues();
        }

        public float GetBlowerFlowPercent() => blower.value * (boilerPressure.value / boilerPressure.max);
        public float GetDraftFlowPercent() => exhaust.value / simParams.SteamPipeMultiplier;

        protected const float TEMP_GAIN_PER_UNIT_FUEL = 20; // 2.5
        protected const float TEMP_RESISTANCE_DIVISOR = 16;

        protected const float AMBIENT_TEMP = 25;
        protected const float AMBIENT_LOSS_MULTIPLIER = 0.2f;
        protected const float DOOR_LOSS_MULTIPLIER = 0.2f;
        protected const float BURN_RATE_INERTIA = 3000;

        protected float burnRateDamp;

        protected void SimulateFire(float delta)
        {
            if (fireOn.value > 0 && fireboxFuel.value == 0)
            {
                fireOn.SetNextValue(0);
            }

            if ((fireOn.value > 0) && (fireboxFuel.value > 0))
            {
                //float boxPercentFill = fireboxFuel.value / fireboxFuel.max;
                //float richMultiplier = (1 - Mathf.Abs(boxPercentFill - MAX_BURN_FILL_LEVEL));

                float damperMult = damper.value < 0.1f ? 0.1f : damper.value;
                float airRate = 
                    (simParams.BaseAirMultiplier + GetDraftFlowPercent() * simParams.DraftAirMultiplier + GetBlowerFlowPercent() * simParams.BlowerAirMultiplier) * damperMult;

                float targetRate = airRate * simParams.MaxBurnRate;
                if (fireboxFuel.value < (0.1f * fireboxFuel.max))
                {
                    targetRate *= 0.1f;
                }
                fuelConsumptionRate = Mathf.SmoothDamp(fuelConsumptionRate, targetRate, ref burnRateDamp, delta * BURN_RATE_INERTIA);
            }
            else
            {
                fuelConsumptionRate = 0;
            }

            float fuelConsumedInTick = fuelConsumptionRate * delta;
            TotalFuelConsumed += fuelConsumedInTick;

            if (GetFuelResourceType(simParams) == ResourceType.Coal)
            {
                fireboxFuel.AddNextValue(-fuelConsumedInTick);
            }
            else
            {
                fireboxFuel.SetNextValue(0);
            }

            if (autoFuelFeed.value > 0 && tenderFuel.value > 0)
            {
                tenderFuel.PassValueToNext(fireboxFuel, autoFuelFeed.value * delta);
            }

            float tempDampFactor = (temperature.max - temperature.value) * 2f / tempRange;
            float tempGain = fuelConsumptionRate * TEMP_GAIN_PER_UNIT_FUEL * tempDampFactor;

            float tempLoss = (temperature.value - AMBIENT_TEMP) * AMBIENT_LOSS_MULTIPLIER;

            float tempDelta = (tempGain - tempLoss) * delta;

            temperature.AddNextValue(tempDelta);
            if (fireOn.value > 0)
            {
                temperature.SetNextValue(Mathf.Max(temperature.nextValue, SteamLocoSimulation.MIN_TEMP_WHEN_FIRE_ON_C));
            }
        }

        protected virtual void SimulateWater(float delta)
        {
            if (injector.value > 0f && tenderWater.value > 0f && boilerWater.value < boilerWater.max)
            {
                injectorFlowRate = simParams.InjectorMaxFlowLPS * injector.value;
                tenderWater.PassValueToNext(boilerWater, injectorFlowRate * delta);
            }
            else
            {
                injectorFlowRate = 0;
            }

            if (waterDump.value > 0f && boilerWater.value > boilerWater.min)
            {
                boilerWater.AddNextValue(-SteamLocoSimulation.WATER_DUMP_STREAM_L * waterDump.value * delta);
            }
        }

        protected const float PRESSURE_CONST = 875E-5f;

        protected const float DYNAMO_INERTIA = 3000f;
        private float _dynamoAccel;

        protected virtual void SimulateSteam(float delta)
        {
            // evaporation
            if ((temperature.value > SteamLocoSimulation.WATER_BOIL_TEMP) && 
                (boilerWater.value > 0) && 
                (boilerPressure.value < boilerPressure.max * 0.999f))
            {
                float evaporatedWater = simParams.VaporizationRate * (temperature.value - SteamLocoSimulation.WATER_BOIL_TEMP) * delta;
                boilerWater.AddNextValue(-evaporatedWater);
                boilerPressure.AddNextValue(evaporatedWater * PRESSURE_CONST);
            }

            // steam dump
            if (steamReleaser.value > 0 && boilerPressure.value > 0)
            {
                boilerPressure.AddNextValue(-SteamLocoSimulation.STEAM_RELEASER_STREAM_KG_PER_SQR_CM_PER_S * steamReleaser.value * delta);
            }

            // safety valve
            if (safetyPressureValve.value == 0 && boilerPressure.value > simParams.SafetyValvePressure)
            {
                safetyPressureValve.SetNextValue(1);
            }
            if (safetyPressureValve.value > 0)
            {
                boilerPressure.AddNextValue(-SteamLocoSimulation.SAFETY_PRESSURE_DUMP_STREAM_KG_PER_SQR_CM_PER_S * delta);

                if (boilerPressure.value < simParams.SafetyValvePressure * 0.975f)
                {
                    safetyPressureValve.SetNextValue(0);
                }
            }

            // leaks
            float damageDegree = Mathf.InverseLerp(SteamLocoSimulation.BODY_DAMAGE_PRESSURE_LEAK_THRESHOLD_PERCENTAGE, 1, dmgController.bodyDamage.DamagePercentage);
            pressureLeakMultiplier = Mathf.Lerp(SteamLocoSimulation.PRESSURE_LEAK_MULTIPLIER_MIN, SteamLocoSimulation.PRESSURE_LEAK_MULTIPLIER_MAX, damageDegree);
            boilerPressure.AddNextValue(-SteamLocoSimulation.PRESSURE_LEAK_L * pressureLeakMultiplier * delta);

            // dynamo
            float dynamoTarget = Mathf.InverseLerp(boilerPressure.min, simParams.SafetyValvePressure / 2, boilerPressure.value) * dynamoValve.value;
            dynamoSpeed.SetNextValue(Mathf.SmoothDamp(dynamoSpeed.value, dynamoTarget, ref _dynamoAccel, delta * DYNAMO_INERTIA));
        }

        private float GetCutoffPowerPercent()
        {
            float c = cutoff.value;
            return Mathf.Sqrt((2 * c) - (c * c));
        }

        private (float, float) GetSteamPipeFlow()
        {
            float speedPercent = Mathf.InverseLerp(0, speed.max / 2, speed.value);
            float availableFlow = (boilerPressure.value / boilerPressure.max) * regulator.value;
            return (
                availableFlow * GetCutoffPowerPercent(),
                Mathf.Lerp(0, availableFlow, speedPercent) * simParams.SteamPipeMultiplier * cutoff.value
            );
        }

        protected virtual void SimulateCylinder(float delta)
        {
            (float availFlowPercent, float actualFlow) = GetSteamPipeFlow();

            power.SetNextValue(availFlowPercent);
            exhaust.SetNextValue(actualFlow * delta);

            if (power.value > 0 && boilerPressure.value > 0)
            {
                boilerPressure.AddNextValue(-actualFlow * delta);
            }
        }

        protected virtual void SimulateSand(float delta)
        {
            if (sandValve.value > 0f && sand.value > 0f && boilerPressure.value > 0f)
            {
                sandFlow.SetNextValue(sandValve.value);
            }
            else
            {
                sandFlow.SetNextValue(0f);
            }
            if (sandFlow.value > 0f)
            {
                sand.AddNextValue(-simParams.SandMaxFlow * sandFlow.value * delta);
                boilerPressure.AddNextValue(SteamLocoSimulation.SAND_PRESSURE_CONSUMPTION_KG_PER_SQR_CM_PER_S * sandFlow.value * delta);
            }
        }

        #endregion

        //================================================================================
        #region Debt Components

        public override IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            var debts = new List<DebtTrackingInfo>()
            {
                new DebtTrackingInfo(this, new DebtComponent(0, ResourceType.EnvironmentDamageCoal)),
                new DebtTrackingInfo(this, new DebtComponent(sand.value, ResourceType.Sand))
            };

            if (bunkerWater.max > 0)
            {
                debts.Add(new DebtTrackingInfo(this, new DebtComponent(bunkerWater.value, ResourceType.Water)));
            }
            if (bunkerFuel.max > 0)
            {
                debts.Add(new DebtTrackingInfo(this, new DebtComponent(bunkerFuel.value, GetFuelResourceType(simParams))));
            }
            return debts;
        }

        public override void ResetDebt(DebtComponent debt)
        {
            if (debt.type == FuelType)
            {
                debt.ResetComponent(bunkerFuel.value);
                return;
            }

            switch (debt.type)
            {
                case ResourceType.EnvironmentDamageCoal:
                    debt.ResetComponent(TotalFuelConsumed);
                    break;

                case ResourceType.Sand:
                    debt.ResetComponent(sand.value);
                    break;

                case ResourceType.Water:
                    debt.ResetComponent(bunkerWater.value);
                    break;

                default:
                    Main.Warning("Tried to reset debt value this loco sim doesn't have");
                    break;
            }
        }

        public override void UpdateDebtValue(DebtComponent debt)
        {
            if (debt.type == FuelType)
            {
                debt.UpdateEndValue(bunkerFuel.value);
                return;
            }

            switch (debt.type)
            {
                case ResourceType.EnvironmentDamageCoal:
                    debt.UpdateStartValue(TotalFuelConsumed);
                    break;

                case ResourceType.Sand:
                    debt.UpdateEndValue(sand.value);
                    break;

                case ResourceType.Water:
                    debt.UpdateEndValue(bunkerWater.value);
                    break;

                default:
                    Main.Warning("Tried to update debt value this loco sim doesn't have");
                    break;
            }
        }

        #endregion

        #region Pit Stop Parameters

        public override IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            var values = new List<PitStopRefillable>() { new PitStopRefillable(this, ResourceType.Sand, sand) };
            
            if (bunkerWater.max > 0)
            {
                values.Add(new PitStopRefillable(this, ResourceType.Water, bunkerWater));
            }
            if (bunkerFuel.max > 0)
            {
                values.Add(new PitStopRefillable(this, ResourceType.Coal, bunkerFuel));
            }
            return values;
        }

        public override float GetPitStopLevel(ResourceType type)
        {
            if (type == FuelType)
            {
                return bunkerFuel.value;
            }

            switch (type)
            {
                case ResourceType.Sand:
                    return sand.value;

                case ResourceType.Water:
                    return bunkerWater.value;

                default:
                    Main.Warning("Tried to get pit stop value this loco sim doesn't have");
                    return 0;
            }
        }

        public override void ChangePitStopLevel(ResourceType type, float changeAmount)
        {
            if (type == FuelType)
            {
                bunkerFuel.AddValue(changeAmount);
                return;
            }

            switch (type)
            {
                case ResourceType.Sand:
                    sand.AddValue(changeAmount);
                    return;

                case ResourceType.Water:
                    bunkerWater.AddValue(changeAmount);
                    return;

                default:
                    Main.Warning("Trying to refill/repair something that is not part of this loco");
                    return;
            }
        }

        #endregion

        internal static ResourceType GetFuelResourceType(SimParamsSteam simParams)
        {
            return (ResourceType)simParams.FuelType;
        }
    }
}
