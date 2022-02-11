using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.CabControls;
using DV.MultipleUnit;
using DV.ServicePenalty;
using DV.Util.EventWrapper;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.DieselElectric
{
    public class CustomLocoControllerDiesel : 
		CustomLocoController<
			CustomLocoSimDiesel,
			DamageControllerCustomDiesel,
			CustomDieselSimEvents,
			CustomDieselSaveState>,
		IFusedLocoController
    {
		protected List<CabInputRelay> HornRelays = new List<CabInputRelay>();

		public MultipleUnitModule muModule;

        public float EngineRPM => sim.engineRPM.value;

		public bool EngineRunning
		{
			get => sim.engineOn;
			set
            {
				if( value != sim.engineOn )
				{
					sim.engineOn = value;
					if( train.brakeSystem )
					{
						train.brakeSystem.compressorRunning = value;
					}
					eventController.EngineRunningChanged.Invoke(value);
				}
			}
		}
		public bool CanEngineStart => !EngineRunning && (FuelLevel > 0);
		public bool AutoStart => autostart;


		private float GetFuel() => sim.fuel.value;
		private float GetOil() => sim.oil.value;
		private float GetSandLvl() => sim.sand.value;
		private bool GetSandersOn() => sim.sandOn;
		private float GetEngineTemp() => sim.engineTemp.value;
		private float GetEngineRPMGauge() => sim.engineOn ? (12.5f + sim.engineRPM.value * 100f) : 0f;
		private float GetAmperage()
		{
			float traction = (reverser == 0) ? 0 : GetTractionForce();
			return 0.00387571f * traction;
		}

		protected float _FuelLevel;
		public float FuelLevel => _FuelLevel;

		protected float _OilLevel;
		public float OilLevel => _OilLevel;

		protected float _SandLevel;
		public float SandLevel => _SandLevel;

		protected bool _SandersOn;
		public bool SandersOn => _SandersOn;

		protected float _EngineTemp;
		public float EngineTemp => _EngineTemp;

		protected float _EngineRPMGauge;
		public float EngineRPMGauge => _EngineRPMGauge;

		protected float _Amperage;
		public float Amperage => _Amperage;


		protected bool FanOn;
		public void SetFanControl(float value)
		{
			EventManager.UpdateValueDispatchOnChange(this, ref FanOn, value > 0.5f, SimEventType.Fan);
		}

        private void UpdateWatchables()
        {
			EventManager.UpdateValueDispatchOnChange(this, ref _FuelLevel, GetFuel(), SimEventType.Fuel);
			EventManager.UpdateValueDispatchOnChange(this, ref _OilLevel, GetOil(), SimEventType.Oil);
			EventManager.UpdateValueDispatchOnChange(this, ref _SandLevel, GetSandLvl(), SimEventType.Sand);
			EventManager.UpdateValueDispatchOnChange(this, ref _EngineTemp, GetEngineTemp(), SimEventType.EngineTemp);
			EventManager.UpdateValueDispatchOnChange(this, ref _EngineRPMGauge, GetEngineRPMGauge(), SimEventType.EngineRPMGauge);
			EventManager.UpdateValueDispatchOnChange(this, ref _Amperage, GetAmperage(), SimEventType.Amperage);
        }

        public override void RegisterControl( CabInputRelay inputRelay )
        {
			switch( inputRelay.Binding )
            {
				case CabInputType.Horn:
					inputRelay.SetIOHandlers(UpdateHorn);
					HornRelays.Add(inputRelay);
					break;

				case CabInputType.Sand:
					inputRelay.SetIOHandlers(SetSanders, GetSanders);
					break;

				case CabInputType.Fan:
					inputRelay.SetIOHandlers(SetFanControl, null);
					break;

				default:
					base.RegisterControl(inputRelay);
					break;
			}
        }

        public override bool AcceptsControlOfType( CabInputType inputType )
        {
            return inputType.EqualsOneOf(
				CabInputType.Horn,
				CabInputType.Sand,
				CabInputType.Fan
			) || base.AcceptsControlOfType(inputType);
        }

        public override float GetSandersFlow()
		{
			if( sim.sand.value <= 0f )
			{
				return 0f;
			}
			return sim.sandFlow.value;
		}

		public void SetSandersFlow( float value )
		{
			sim.sandFlow.SetValue(value);
		}

		public override void SetSanders( float value )
		{
			if( sim.sandOn ^ (value > 0.5f) )
			{
				sim.sandOn = (value > 0.5f);
				base.SetSanders(value);
				EventManager.Dispatch(this, SimEventType.SandDeploy, sim.sandOn);
			}
		}

		public float GetSanders()
        {
			return sim.sandOn ? 1 : 0;
        }

		public override void SetReverser( float position )
		{
			if( targetThrottle > 0.05f )
			{
				return;
			}
			if( position < 0f )
			{
				position = -1f;
			}
			else if( position > 0f )
			{
				position = 1f;
			}
			else
			{
				position = 0f;
			}
			base.SetReverser(position);
		}

		public override float GetTractionForce()
		{
			float num = (sim.engineRPM.value > 0f) ? tractionTorqueCurve.Evaluate(GetSpeedKmH() / sim.engineRPM.value) : 0f;
			float num2 = (Mathf.Sign(GetForwardSpeed() * reverser) > 0f) ? num : 1f;
			return sim.engineRPM.value * num2 * tractionTorqueMult;
		}

        #region Events

        //protected override void Awake()
        //{
        //	base.Awake();

        //	// TODO: MU, save state
        //	//MultipleUnitModule component = base.GetComponent<MultipleUnitModule>();
        //	//base.gameObject.AddComponent<LocoStateSaveDiesel>().Initialize(sim, damageController, this, carVisitChecker, component);
        //}

        private void OnDisable()
		{
			SetupListeners(false);
		}

		private void OnEnable()
		{
			SetupListeners(true);
		}

		private void OnEngineTempChanged( LocoSimulationEvents.Amount amount )
		{
			if( amount == LocoSimulationEvents.Amount.Full )
			{
				EngineRunning = false;
			}
		}

		private void OnFuelChanged( LocoSimulationEvents.Amount amount )
		{
			if( amount == LocoSimulationEvents.Amount.Depleted )
			{
				EngineRunning = false;
			}
		}

		public override void SetNeutralState()
		{
			EngineRunning = false;
			SetSanders(0f);
			SetSandersFlow(0f);
			SetThrottle(0f);
			SetReverser(0f);
            SetBrake(0f);
            SetIndependentBrake(1f);
		}


		private void SetupListeners( bool on )
		{
			eventController.FuelChanged.Manage(OnFuelChanged, on);
			eventController.EngineTempChanged.Manage(OnEngineTempChanged, on);
		}

        //protected override bool ShouldSwitchToTrainBrakeOnStart()
        //{
        //	return LicenseManager.IsGeneralLicenseAcquired(GeneralLicenseType.DE6) && base.ShouldSwitchToTrainBrakeOnStart();
        //}

        protected override void Start()
		{
			base.Start();
			if( !VRManager.IsVREnabled() )
			{
				var keyboardCtrl = gameObject.AddComponent<LocoKeyboardInputDiesel>();
				keyboardCtrl.control = this;
				Main.LogVerbose("Added keyboard input to car");
			}

			train.brakeSystem.compressorProductionRate = sim.simParams.AirCompressorRate;
		}

        #endregion

        #region Update Loop

        public override void Update()
		{
			UpdateWatchables();
			base.Update();
			UpdateSimSpeed();
			UpdateSimThrottle();
			UpdateControls();
		}

		private void UpdateSimSpeed()
		{
			sim.speed.SetValue(GetSpeedKmH());
			sim.goingForward = (GetForwardSpeed() > 0f);
		}

		private void UpdateSimThrottle()
		{
			sim.throttle.SetValue(sim.engineOn ? throttle : 0f);
			sim.throttleToTargetDiff.SetValue(sim.engineOn ? (throttle - targetThrottle) : 0f);
		}

		private void UpdateControls()
        {
			if( isAcceptingKeyboardInput )
            {
				if( KeyBindings.increaseSandKeys.IsDown() && !sim.sandOn )
                {
					SetSanders(1);
                }
				if( KeyBindings.decreaseSandKeys.IsDown() && sim.sandOn )
                {
					SetSanders(0);
                }
				if( KeyBindings.increaseHornKeys.IsPressed() )
                {
					foreach( var relay in HornRelays )
                    {
						if( relay.Initialized ) relay.DeflectWithSpring(UnityEngine.Random.Range(0.95f, 1f));
					}
                }
            }
        }

        #endregion
    }
}
