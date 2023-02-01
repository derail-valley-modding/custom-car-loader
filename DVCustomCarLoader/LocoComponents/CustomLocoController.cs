using System;
using System.Collections.Generic;
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

        protected List<WatchableValue> _watchables = new List<WatchableValue>();
        public IEnumerable<WatchableValue> Watchables => _watchables;

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
            SetReverser((position * 2f) - 1f);
        }
        public float GetReverserCabPosition() => (reverser + 1f) / 2f;

        protected float _AccessoryPowerLevel;
        protected abstract float AccessoryPowerLevel { get; }

        // Headlights
        protected float _HeadlightControlLevel;
        protected float _Headlights;
        public float Headlights => _Headlights;

        protected float _ForwardLights;
        public float ForwardLights => _ForwardLights;

        protected float _RearLights;
        public float RearLights => _RearLights;

        public float GetHeadlightControl() => _HeadlightControlLevel;
        public void SetHeadlight( float value )
        {
            if( value != _HeadlightControlLevel )
            {
                _HeadlightControlLevel = value;
                value = _HeadlightControlLevel * AccessoryPowerLevel;
                EventManager.UpdateValueDispatchOnChange(this, ref _Headlights, value, SimEventType.Headlights);
            }
        }

        // Cab Lights
        protected float _CabLightControlLevel;
        protected float _CabLights;
        public float CabLights => _CabLights;
        public float GetCabLightControl() => _CabLightControlLevel;
        public void SetCabLight( float value )
        {
            if (value != _CabLightControlLevel)
            {
                _CabLightControlLevel = value;
                value = _CabLightControlLevel * AccessoryPowerLevel;
                EventManager.UpdateValueDispatchOnChange(this, ref _CabLights, value, SimEventType.CabLights);
            }
        }

        // Compressor
        protected float _CompressorControl = 1;
        public float GetCompressorControl() => _CompressorControl;
        public void SetCompressorControl(float value) => _CompressorControl = value;
        public abstract float GetCompressorSpeed();

        public float BrakePipePressure => GetBrakePipePressure();

        public float BrakeResPressure => GetBrakeResPressure();

        public float IndependentPressure => GetIndependentPressure();

        protected override void Awake()
        {
            base.Awake();

            _watchables.AddNew(this, SimEventType.Speed, GetSpeedKmH);
            _watchables.AddNew(this, SimEventType.BrakePipe, GetBrakePipePressure);
            _watchables.AddNew(this, SimEventType.BrakeReservoir, GetBrakeResPressure);
            _watchables.AddNew(this, SimEventType.IndependentPipe, GetIndependentPressure);
            _watchables.AddNew(this, SimEventType.CompressorSpeed, GetCompressorSpeed);

            _watchables.AddNew(this, SimEventType.AccessoryPower, () => AccessoryPowerLevel);
            _watchables.AddNew(this, SimEventType.Headlights, () => _HeadlightControlLevel * AccessoryPowerLevel);
            _watchables.AddNew(this, SimEventType.CabLights, () => _CabLightControlLevel * AccessoryPowerLevel);
            _watchables.AddNew(this, SimEventType.LightsForward, () => Mathf.Lerp(0, _Headlights, Mathf.InverseLerp(0, 1, reverser)));
            _watchables.AddNew(this, SimEventType.LightsReverse, () => Mathf.Lerp(0, _Headlights, Mathf.InverseLerp(0, -1, reverser)));
        }

        public virtual void ForceDispatchAll() { }

        #region ICabControlAcceptor
        public bool HasCompressorControl { get; protected set; } = false;

        public virtual void RegisterControl(CabInputRelay inputRelay)
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
                    inputRelay.SetIOHandlers(SetHeadlight, GetHeadlightControl);
                    break;

                case CabInputType.CabLights:
                    inputRelay.SetIOHandlers(SetCabLight, GetCabLightControl);
                    break;

                case CabInputType.Compressor:
                    HasCompressorControl = true;
                    inputRelay.SetIOHandlers(SetCompressorControl, GetCompressorControl);
                    break;

                default:
                    break;
            }
        }

        public virtual bool AcceptsControlOfType(CabInputType inputType)
        {
            return inputType.EqualsOneOf(
                CabInputType.TrainBrake,
                CabInputType.IndependentBrake,
                CabInputType.Throttle,
                CabInputType.Reverser,
                CabInputType.Headlights,
                CabInputType.CabLights,
                CabInputType.Compressor
            );
        }

        #endregion
    }

    public abstract class CustomLocoController<TSim, TDmg, TEvents, TSave> : CustomLocoController
        where TSim : CustomLocoSimulation
        where TDmg : DamageControllerCustomLoco
        where TEvents : CustomLocoSimEvents<TSim, TDmg>
        where TSave : CustomLocoSaveState
    {
        protected TSim sim;
        protected TDmg damageController;
        protected TEvents eventController;
        protected TSave saveState;

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
                tractionTorqueMult = simParams.tractionTorqueMultiplier;
                allowRemoteControl = simParams.AllowRemoteControl;
            }
            else
            {
                Main.Error($"Sim parameters not found for this loco {train?.ID}");
            }

            carVisitChecker = gameObject.AddComponent<CarVisitChecker>();
            carVisitChecker.Initialize(train);

            saveState = gameObject.AddComponent<TSave>();
            saveState.Initialize(carVisitChecker);

            train.LogicCarInitialized += OnLogicCarInitialized;
        }

        protected virtual void OnLogicCarInitialized()
        {
            train.LogicCarInitialized -= OnLogicCarInitialized;

            if (!train.playerSpawnedCar || Main.Settings.FeesForCCLLocos)
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
            if ((!train.playerSpawnedCar || Main.Settings.FeesForCCLLocos) && (locoDebt != null))
            {
                SingletonBehaviour<LocoDebtController>.Instance.StageLocoDebtOnLocoDestroy(locoDebt);
            }
        }
    }
}