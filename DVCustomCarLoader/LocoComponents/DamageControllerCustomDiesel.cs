using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCL_GameScripts;
using DV.JObjectExtstensions;
using DV.ServicePenalty;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class DamageControllerCustomDiesel : DamageControllerCustomLoco<
            CustomLocoControllerDiesel,
            CustomDieselSimEvents,
            DamageConfigDiesel>
    {
        protected const string ENGINE_HP_SAVE_KEY = "engineHP";

        [Header("Additional damage types")]
        public TrainDamage engine;
        protected Coroutine randomShutoffCoroutine = null;

        protected virtual void OnEnable()
        {
            SetupListeners(true);
        }

        protected virtual void OnDisable()
        {
            SetupListeners(false);
        }

        protected virtual void SetupListeners( bool on )
        {
            eventController.EngineRunningChanged.Manage(OnEngineRunningChanged, on);
        }

        public override void RepairAll()
        {
            base.RepairAll();
            engine.RepairDamage(engine.fullHitPoints - engine.currentHitPoints);
        }

        public override void IgnoreDamage( bool set )
        {
            base.IgnoreDamage(set);
            engine.IgnoreDamage(set);
        }

        #region Damage Checks

        protected override void DamagesUpdate()
        {
            base.DamagesUpdate();

            float delta = Time.deltaTime;

            // check cold start damage
            if( (locoController.EngineTemp < config.ColdEngineTempThreshold) && 
                (locoController.EngineRPM > config.ColdEngineRPMThreshold) )
            {
                ApplyDamage(engine, locoController.EngineRPM * config.ColdEngineDPS * delta);
            }

            // running damage
            if( locoController.EngineRunning )
            {
                float curRPM = locoController.EngineRPM;

                // normal wear & tear
                if( curRPM > 0.0001f )
                {
                    ApplyDamage(engine, curRPM * config.EngineRunningDPS * delta);
                }

                // no oil
                if( locoController.OilLevel == 0 )
                {
                    float adjRPM = Mathf.Lerp(0.3f, 1f, curRPM);
                    ApplyDamage(engine, adjRPM * config.EngineNoOilDPS * delta);
                }

                // check for random shutoff
                if( (engine.DamagePercentage >= config.EngineFailureThreshold) && (randomShutoffCoroutine == null) )
                {
                    randomShutoffCoroutine = StartCoroutine(RandomEngineShutoff());
                }
            }
        }

        protected IEnumerator RandomEngineShutoff()
        {
            WaitForSeconds waitTimeout = WaitFor.Seconds(config.RandomShutoffCheckPeriod);

            float threshold = config.EngineFailureThreshold;
            while( locoController.EngineRunning && (engine.DamagePercentage > threshold) )
            {
                if( Random.value <= (config.ShutoffChanceMultiplier * (engine.DamagePercentage - threshold)) )
                {
                    locoController.EngineRunning = false;
                    break;
                }
                yield return waitTimeout;
            }

            randomShutoffCoroutine = null;
            yield break;
        }

        protected override void OnCollisionDamage( float colDamage, Vector3 forceDirection )
        {
            base.OnCollisionDamage(colDamage, forceDirection);

            float engineDamage = bodyDamage.GetModifiedCollisionDamage(colDamage) * config.EngineCollisionMultiplier;

            if( engineDamage > 0 )
            {
                ApplyDamage(engine, engineDamage);

                if( engineDamage > config.CollisionShutoffThreshold )
                {
                    locoController.EngineRunning = false;
                }
            }
        }

        protected override void OnFireDamage( float timeInFire )
        {
            base.OnFireDamage(timeInFire);

            float engineDamage = bodyDamage.GetModifiedFireDamage(timeInFire) * config.EngineFireMultiplier;

            if( engineDamage > 0 )
            {
                ApplyDamage(engine, engineDamage);
            }
        }

        private void OnEngineRunningChanged( bool engineTurnedOn )
        {
            if( engineTurnedOn )
            {
                ApplyDamage(engine, config.EngineStartDamage);
            }
        }

        #endregion

        #region Data Overrides

        protected override void InitializeTrainDamages()
        {
            base.InitializeTrainDamages();

            if( engine == null )
            {
                engine = new TrainDamage(config.EngineHitpoints);
            }
            else
            {
                engine.fullHitPoints = config.EngineHitpoints;
                if( engine.fullHitPoints == 0f )
                {
                    Main.Error("TrainDamage[engine].fullHitPoints is set to invalid value 0! Overriding to 1000");
                    engine.fullHitPoints = 1000f;
                }
                engine.SetCurrentHitPoints(engine.fullHitPoints);
            }
        }

        public override JObject GetDamageSaveData()
        {
            JObject saveData = base.GetDamageSaveData();
            saveData.SetFloat(ENGINE_HP_SAVE_KEY, engine.currentHitPoints);
            return saveData;
        }

        public override void LoadDamagesState( JObject stateData )
        {
            base.LoadDamagesState(stateData);
            float? engineHp = stateData.GetFloat(ENGINE_HP_SAVE_KEY);
            if( engineHp.HasValue )
            {
                engine.SetCurrentHitPoints(engineHp.Value);
            }
            else
            {
                Main.Error("No load data for engineHP found!");
            }
        }

        #endregion

        #region Debt Tracking

        public override IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            return new[]
            {
                new DebtTrackingInfo(this, new DebtComponent(bodyDamage.EffectiveHealthPercentage100Notation, ResourceType.Car_DMG)),
                new DebtTrackingInfo(this, new DebtComponent(wheels.HealthPercentage100Notation, ResourceType.Wheels_DMG)),
                new DebtTrackingInfo(this, new DebtComponent(engine.HealthPercentage100Notation, ResourceType.Engine_DMG))
            };
        }

        public override void ResetDebt( DebtComponent debt )
        {
            switch( debt.type )
            {
                case ResourceType.Car_DMG:
                    debt.ResetComponent(bodyDamage.EffectiveHealthPercentage100Notation);
                    break;

                case ResourceType.Wheels_DMG:
                    debt.ResetComponent(wheels.HealthPercentage100Notation);
                    break;

                case ResourceType.Engine_DMG:
                    debt.ResetComponent(engine.HealthPercentage100Notation);
                    break;
            }
        }

        public override void UpdateDebtValue( DebtComponent debt )
        {
            switch( debt.type )
            {
                case ResourceType.Car_DMG:
                    debt.UpdateEndValue(bodyDamage.EffectiveHealthPercentage100Notation);
                    break;

                case ResourceType.Wheels_DMG:
                    debt.UpdateEndValue(wheels.HealthPercentage100Notation);
                    break;

                case ResourceType.Engine_DMG:
                    debt.UpdateEndValue(engine.HealthPercentage100Notation);
                    break;
            }
        }

        #endregion

        #region Pit Stop Parameters

        public override IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            if( (engine == null) || (wheels == null) || (bodyDamage == null) || !bodyDamage )
            {
                bool eng = (engine != null);
                bool whl = (wheels != null);
                bool body = (bodyDamage != null) && bodyDamage;

                Main.Error($"DamageControllerCustomDiesel: engine={eng}, wheels={whl}, body={body}");
                return Enumerable.Empty<PitStopRefillable>();
            }

            return new[]
            {
                new PitStopRefillable(this, ResourceType.Engine_DMG, engine.HealthPercentage100Notation, 100),
                new PitStopRefillable(this, ResourceType.Car_DMG, bodyDamage.EffectiveHealthPercentage100Notation, 100),
                new PitStopRefillable(this, ResourceType.Wheels_DMG, wheels.HealthPercentage100Notation, 100),
            };
        }

        public override float GetPitStopLevel( ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Car_DMG:
                    return bodyDamage.EffectiveHealthPercentage100Notation;
                case ResourceType.Wheels_DMG:
                    return wheels.HealthPercentage100Notation;
                case ResourceType.Engine_DMG:
                    return engine.HealthPercentage100Notation;
                default:
                    Main.Warning("Tried to get pit stop value this loco damage ctrl doesn't have");
                    return 0;
            }
        }

        public override void ChangePitStopLevel( ResourceType type, float changeAmount )
        {
            switch( type )
            {
                case ResourceType.Car_DMG:
                    bodyDamage.RepairCarEffectivePercentage(changeAmount / 100f);
                    //if( bodyDamage.DamagePercentage < 0.05f )
                    //{
                    //    // Repair windows
                    //}
                    break;

                case ResourceType.Wheels_DMG:
                    wheels.RepairDamagePercentage(changeAmount / 100f);
                    break;

                case ResourceType.Engine_DMG:
                    engine.RepairDamagePercentage(changeAmount / 100f);
                    break;

                default:
                    Main.Warning("Trying to refill/repair something that is not part of this loco");
                    break;
            }
        }

        #endregion
    }
}