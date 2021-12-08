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
    public abstract class CustomLocoController : LocoControllerBase, ILocoEventProvider, ICabControlAcceptor
    {
        public AnimationCurve tractionTorqueCurve;
        public LocoEventManager EventManager { get; set; }

        protected DebtTrackerCustomLoco locoDebt;
        protected CarVisitChecker carVisitChecker;

        private float GetBrakePipePressure() => train.brakeSystem.brakePipePressure;
        private float GetBrakeResPressure() => train.brakeSystem.mainReservoirPressure;

        private static readonly FieldInfo independentPipeField = AccessTools.Field(typeof(DV.Simulation.Brake.BrakeSystem), "independentPipePressure");
        private float GetIndependentPressure()
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
        protected float _Headlights;
        public float Headlights => _Headlights;
        public void SetHeadlight( float value )
        {
            if( value != Headlights )
            {
                //headlights.SetActive(HeadlightsOn);
                EventManager.UpdateValueDispatchOnChange(this, ref _Headlights, value, SimEventType.Headlights);
            }
        }

        // Cab Lights
        protected float _CabLights;
        public float CabLights => _CabLights;
        public void SetCabLight( float value )
        {
            if (value != CabLights)
            {
                EventManager.UpdateValueDispatchOnChange(this, ref _CabLights, value, SimEventType.CabLights);
            }
        }

        protected float _BrakePipePressure;
        public float BrakePipePressure => _BrakePipePressure;

        protected float _BrakeResPressure;
        public float BrakeResPressure => _BrakeResPressure;

        protected float _IndependentPressure;
        public float IndependentPressure => _IndependentPressure;

        protected float Speed;

        public override void Update()
        {
            EventManager.UpdateValueDispatchOnChange(this, ref Speed, GetSpeedKmH(), SimEventType.Speed);
            EventManager.UpdateValueDispatchOnChange(this, ref _BrakePipePressure, GetBrakePipePressure(), SimEventType.BrakePipe);
            EventManager.UpdateValueDispatchOnChange(this, ref _BrakeResPressure, GetBrakeResPressure(), SimEventType.BrakeReservoir);
            EventManager.UpdateValueDispatchOnChange(this, ref _IndependentPressure, GetIndependentPressure(), SimEventType.IndependentPipe);

            base.Update();
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