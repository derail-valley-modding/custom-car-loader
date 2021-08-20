using System;
using System.Collections;
using CCL_GameScripts.CabControls;
using DV;
using DV.ServicePenalty;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoController : LocoControllerBase
    {
        public AnimationCurve tractionTorqueCurve;

        protected DebtTrackerCustomLoco locoDebt;
        protected CarVisitChecker carVisitChecker;

        public float GetBrakePipePressure() => train.brakeSystem.brakePipePressure;
        public float GetBrakeResPressure() => train.brakeSystem.mainReservoirPressure;

        public void SetReverserFromCab( float position )
        {
            reverser = (position * 2f) - 1f;
        }
        public float GetReverserCabPosition() => (reverser + 1f) / 2f;

        public virtual Func<float> GetIndicatorFunc( CabIndicatorType indicatedType )
        {
            switch( indicatedType )
            {
                case CabIndicatorType.BrakePipe:
                    return GetBrakePipePressure;
                    
                case CabIndicatorType.BrakeReservoir:
                    return GetBrakeResPressure;

                case CabIndicatorType.Speed:
                    return GetSpeedKmH;

                default:
                    return () => 0;
            }
        }

        public virtual (Action<float>, Func<float>) GetCabControlActions( CabInputType inputType )
        {
            switch( inputType )
            {
                case CabInputType.TrainBrake:
                    return (SetBrake, GetTargetBrake);

                case CabInputType.IndependentBrake:
                    return (SetIndependentBrake, GetTargetIndependentBrake);

                case CabInputType.Throttle:
                    return (SetThrottle, GetTargetThrottle);

                case CabInputType.Reverser:
                    return (SetReverserFromCab, GetReverserCabPosition);

                default:
                    return (null, null);
            }
        }
    }

    public abstract class CustomLocoController<TSim,TDmg,TEvents> : CustomLocoController
        where TSim : CustomLocoSimulation
        where TDmg : DamageControllerCustomLoco
        where TEvents : CustomLocoSimEvents<TSim, TDmg>
    {
        protected TSim sim;
        protected TDmg damageController;
        protected TEvents eventController;

        protected override void Awake()
        {
            base.Awake();
            sim = GetComponent<TSim>();
            damageController = GetComponent<TDmg>();
            eventController = GetComponent<TEvents>();

            var simParams = GetComponent<CCL_GameScripts.SimParamsBase>();
            if( simParams )
            {
                brakePowerCurve = simParams.BrakePowerCurve;
                tractionTorqueCurve = simParams.TractionTorqueCurve;
            }
            else
            {
                Main.Error($"Sim parameters not found for this loco {train?.ID}");
            }

            carVisitChecker = gameObject.AddComponent<CarVisitChecker>();
            carVisitChecker.Initialize(train);

            train.LogicCarInitialized += OnLogicCarInitialized;
        }

        protected virtual void OnLogicCarInitialized()
        {
            train.LogicCarInitialized -= OnLogicCarInitialized;

            if( !train.playerSpawnedCar )
            {
                locoDebt = new DebtTrackerCustomLoco(train.ID, train.carType, this, damageController, sim);
                SingletonBehaviour<LocoDebtController>.Instance.RegisterLocoDebtTracker(locoDebt);
            }

            train.OnDestroyCar += OnLocoDestroyed;
            gameObject.AddComponent<CustomLocoPitStopParams>().Initialize(sim, damageController);
        }

        protected virtual void OnLocoDestroyed( TrainCar train )
        {
            train.OnDestroyCar -= OnLocoDestroyed;
            if( !train.playerSpawnedCar )
            {
                SingletonBehaviour<LocoDebtController>.Instance.StageLocoDebtOnLocoDestroy(locoDebt);
            }
        }
    }
}