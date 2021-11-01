using System;
using System.Reflection;
using CCL_GameScripts.CabControls;
using DV;
using DV.ServicePenalty;
using DV.Util.EventWrapper;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoController : LocoControllerBase, ILocoEventProvider, ICabControlAcceptor, ILocoValueProvider
    {
        public AnimationCurve tractionTorqueCurve;

        protected DebtTrackerCustomLoco locoDebt;
        protected CarVisitChecker carVisitChecker;

        public float GetBrakePipePressure() => train.brakeSystem.brakePipePressure;
        public float GetBrakeResPressure() => train.brakeSystem.mainReservoirPressure;

        private static readonly FieldInfo independentPipeField = AccessTools.Field(typeof(DV.Simulation.Brake.BrakeSystem), "independentPipePressure");
        public float GetIndependentPressure()
        {
            if( independentPipeField != null )
            {
                return (float)independentPipeField.GetValue(train.brakeSystem);
            }
            return 0;
        }

        public void SetReverserFromCab( float position )
        {
            reverser = (position * 2f) - 1f;
        }
        public float GetReverserCabPosition() => (reverser + 1f) / 2f;

        // Headlights
        public bool HeadlightsOn { get; protected set; } = false;
        public float GetHeadlightValue() => HeadlightsOn ? 1 : 0;
        public void SetHeadlight( float value )
        {
            bool lastState = HeadlightsOn;
            HeadlightsOn = value > 0.5f;
            if( lastState ^ HeadlightsOn )
            {
                //headlights.SetActive(HeadlightsOn);
                HeadlightsChanged.Invoke(HeadlightsOn);
            }
        }

        // Cab Lights
        public bool CabLightsOn { get; protected set; } = false;
        public float GetCabLightValue() => CabLightsOn ? 1 : 0;
        public void SetCabLight( float value )
        {
            bool lastState = CabLightsOn;
            CabLightsOn = value > 0.5f;
            if( lastState ^ CabLightsOn ) CabLightsChanged.Invoke(CabLightsOn);
        }

        #region ICabControlAcceptor

        public virtual void RegisterControl( CabInputRelay inputRelay )
        {
            switch( inputRelay.Binding )
            {
                case CabInputType.TrainBrake:
                    inputRelay.SetIOHandlers(SetBrake, GetTargetBrake);
                    break;

                case CabInputType.IndependentBrake:
                    inputRelay.SetIOHandlers(SetIndependentBrake, GetTargetIndependentBrake);
                    break;

                case CabInputType.Throttle:
                    inputRelay.SetIOHandlers(SetThrottle, GetTargetThrottle);
                    break;

                case CabInputType.Reverser:
                    inputRelay.SetIOHandlers(SetReverserFromCab, GetReverserCabPosition);
                    break;

                case CabInputType.Headlights:
                    inputRelay.SetIOHandlers(SetHeadlight, null);
                    break;

                case CabInputType.CabLights:
                    inputRelay.SetIOHandlers(SetCabLight, null);
                    break;

                default:
                    break;
            }
        }

        public virtual bool AcceptsControlOfType( CabInputType inputType )
        {
            return inputType.EqualsOneOf(
                CabInputType.TrainBrake,
                CabInputType.IndependentBrake,
                CabInputType.Throttle,
                CabInputType.Reverser,
                CabInputType.Headlights,
                CabInputType.CabLights
            );
        }

        #endregion

        #region Events

        public event_<bool> HeadlightsChanged;
        public event_<bool> CabLightsChanged;

        public virtual bool TryBind(ILocoEventAcceptor listener)
        {
            return false;
        }

        public virtual bool TryBind(ILocoValueWatcher watcher)
        {
            switch (watcher.ValueBinding)
            {
                case CabIndicatorType.BrakePipe:
                    watcher.Bind(GetBrakePipePressure);
                    return true;

                case CabIndicatorType.BrakeReservoir:
                    watcher.Bind(GetBrakeResPressure);
                    return true;

                case CabIndicatorType.Speed:
                    watcher.Bind(GetSpeedKmH);
                    return true;

                case CabIndicatorType.IndependentPipe:
                    watcher.Bind(GetTargetIndependentBrake);
                    return true;

                case CabIndicatorType.Headlights:
                    watcher.Bind(GetHeadlightValue);
                    return true;

                case CabIndicatorType.CabLights:
                    watcher.Bind(GetCabLightValue);
                    return true;

                case CabIndicatorType.Reverser:
                    watcher.Bind(() => reverser);
                    return true;

                default:
                    return false;
            }
        }

        #endregion
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