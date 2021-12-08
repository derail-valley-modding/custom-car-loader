using System.Collections;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomDieselSimEvents :
        CustomLocoSimEvents<CustomLocoSimDiesel, DamageControllerCustomDiesel>
    {
        protected const float ENGINE_OVERHEAT_CHECK_PERIOD = 1f;
        protected const float ENGINE_TEMP_CHECK_PERIOD = 3f;

        protected const float OVERHEAT_TIME_LIMIT_MAX = 25f;
        protected const float OVERHEAT_TIME_LIMIT_MIN = 15f;

        protected const float LOW_ENGINE_TEMP_THRESHOLD = 90f;
        protected const float MID_ENGINE_TEMP_THRESHOLD = 105f;

        protected float LowFuelThreshold;

        protected float LowOilThreshold;
        protected float MidOilThreshold;

        protected float LowSandThreshold;

        protected float EngineDamageThreshold;

        private Coroutine OverheatCheckCoroutine;

        protected LocoEventWrapper<Amount> FuelEvent;
        protected LocoEventWrapper<Amount> OilEvent;
        protected LocoEventWrapper<bool> EngineRunningEvent;
        protected LocoEventWrapper<Amount> EngineTempEvent;
        protected LocoEventWrapper<Amount> EngineDamageEvent;

        protected CustomDieselSimEvents()
        {
            FuelEvent = LocoEventWrapper<Amount>.Create(ref FuelChanged, this, SimEventType.Fuel);
            OilEvent = LocoEventWrapper<Amount>.Create(ref OilChanged, this, SimEventType.Oil);
            EngineRunningEvent = LocoEventWrapper<bool>.Create(ref EngineRunningChanged, this, SimEventType.EngineOn);
            EngineTempEvent = LocoEventWrapper<Amount>.Create(ref EngineTempChanged, this, SimEventType.EngineTemp);
            EngineDamageEvent = LocoEventWrapper<Amount>.Create(ref EngineDamageChanged, this, SimEventType.EngineDamage);
        }

        protected override void InitThresholds()
        {
            LowFuelThreshold = sim.fuel.max / 4f;

            LowOilThreshold = sim.oil.max / 4f;
            MidOilThreshold = sim.oil.max / 2f;

            LowSandThreshold = sim.sand.max / 5f;

            EngineDamageThreshold = sim.simParams.PerformanceDropDamageLevel;
        }

        protected override void Start()
        {
            base.Start();

            Fuel = GetAmount(sim.fuel.value, LowFuelThreshold);
            Oil = GetAmount(sim.oil.value, LowOilThreshold, MidOilThreshold);
            Sand = GetAmount(sim.sand.value, LowSandThreshold);
            EngineTemp = GetAmount(sim.engineTemp.value, LOW_ENGINE_TEMP_THRESHOLD, MID_ENGINE_TEMP_THRESHOLD);
            EngineDamage = GetAmount(dmgController.engine.DamagePercentage, EngineDamageThreshold);
        }

        protected void OnDisable()
        {
            StopAllCoroutines();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(CheckEngineTemp());
        }

        private IEnumerator CheckEngineTemp()
        {
            WaitForSeconds timeout = WaitFor.Seconds(ENGINE_TEMP_CHECK_PERIOD);
            while( true )
            {
                yield return timeout;

                Amount curTempLvl = GetAmount(sim.engineTemp.value, LOW_ENGINE_TEMP_THRESHOLD, MID_ENGINE_TEMP_THRESHOLD);
                if( EngineTemp != curTempLvl )
                {
                    EngineTemp = curTempLvl;
                    EngineTempChanged.Invoke(curTempLvl);
                }

                if( curTempLvl == Amount.High && OverheatCheckCoroutine == null )
                {
                    OverheatCheckCoroutine = StartCoroutine(OverHeatCheck());
                }
            }

            //yield break;
        }

        private IEnumerator OverHeatCheck()
        {
            WaitForSeconds waitTimeout = WaitFor.Seconds(ENGINE_OVERHEAT_CHECK_PERIOD);

            float timeOnHighTemp = 0f;
            float overheatTimeLimit = Random.Range(OVERHEAT_TIME_LIMIT_MIN, OVERHEAT_TIME_LIMIT_MAX);

            while( timeOnHighTemp < overheatTimeLimit )
            {
                yield return waitTimeout;
                if( EngineTemp < Amount.High )
                {
                    OverheatCheckCoroutine = null;
                    yield break;
                }
                timeOnHighTemp += ENGINE_OVERHEAT_CHECK_PERIOD;
            }

            EngineTempChanged.Invoke(Amount.Full);
            OverheatCheckCoroutine = null;

            yield break;
        }

        protected override void CheckTankAndDamageLevels()
        {
            Amount newFuelLevel = GetAmount(sim.fuel.value, LowFuelThreshold);
            if( Fuel != newFuelLevel )
            {
                Fuel = newFuelLevel;
                if( (newFuelLevel == Amount.Depleted && sim.engineOn) || newFuelLevel != Amount.Depleted )
                {
                    FuelChanged.Invoke(newFuelLevel);
                }
            }

            Amount newOilLevel = GetAmount(sim.oil.value, LowOilThreshold, MidOilThreshold);
            if( Oil != newOilLevel )
            {
                Oil = newOilLevel;
                OilChanged.Invoke(newOilLevel);
            }

            Amount newSandLevel = GetAmount(sim.sand.value, LowSandThreshold);
            if( Sand != newSandLevel )
            {
                Sand = newSandLevel;
                SandChanged.Invoke(newSandLevel);
            }

            Amount newDamageLevel = GetAmount(dmgController.engine.DamagePercentage, EngineDamageThreshold);
            if( EngineDamage != newDamageLevel )
            {
                EngineDamage = newDamageLevel;
                EngineDamageChanged.Invoke(newDamageLevel);
            }
        }
    }
}