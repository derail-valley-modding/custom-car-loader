using System.Collections;
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