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

        public LocoSimulationEvents.Amount MULampState
		{
			get
			{
				bool IsConnectedEngineOn(CouplingHoseMultipleUnitAdapter adapter)
				{
					LocoControllerBase ctrl = adapter.muCable.connectedTo.muModule.loco;
					if (ctrl is CustomLocoControllerDiesel clcd) return clcd.EngineRunning;
					if (ctrl is LocoControllerDiesel lcd) return lcd.GetEngineRunning();
					return false;
				}

				if (!muModule) return LocoSimulationEvents.Amount.Depleted;

				bool frontConnected = muModule.frontCableAdapter.IsConnected;
				bool rearConnected = muModule.rearCableAdapter.IsConnected;
				
				if (!(frontConnected || rearConnected))
				{
					return LocoSimulationEvents.Amount.Depleted;
				}
				else
				{
					if ((frontConnected && !IsConnectedEngineOn(muModule.frontCableAdapter)) ||
						(rearConnected && !IsConnectedEngineOn(muModule.rearCableAdapter)))
					{
						return LocoSimulationEvents.Amount.Full;
					}
					return LocoSimulationEvents.Amount.Mid;
				}
			}
		}

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
		public bool CanEngineStart => (FuelLevel > 0);
		public bool AutoStart => autostart;
		public bool MasterPower { get; set; }

        protected override float AccessoryPowerLevel => 
			MasterPower ? (0.6f + sim.engineRPM.value * 0.4f) : 0;

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
		public override float GetCompressorSpeed() => train.brakeSystem.compressorRunning ? 1 : 0;

        public float FuelLevel => sim.fuel.value;
		public float OilLevel => sim.oil.value;
		public float EngineTemp => sim.engineTemp.value;

		protected bool FanOn;
		public void SetFanControl(float value)
		{
			EventManager.UpdateValueDispatchOnChange(this, ref FanOn, value > 0.5f, SimEventType.Fan);
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

        public override void UpdateHorn(float value)
        {
			if (BrakeResPressure < 2) value = 0;
            base.UpdateHorn(value);
        }

        public override float GetTractionForce()
		{
			float num = (sim.engineRPM.value > 0f) ? tractionTorqueCurve.Evaluate(GetSpeedKmH() / sim.engineRPM.value) : 0f;
			float num2 = (Mathf.Sign(GetForwardSpeed() * reverser) > 0f) ? num : 1f;
			return sim.engineRPM.value * num2 * tractionTorqueMult;
		}

        #region Events

        protected override void Awake()
		{
			muModule = GetComponent<MultipleUnitModule>();

			base.Awake();

			var simParams = GetComponent<CCL_GameScripts.SimParamsDiesel>();
			if (simParams)
			{
				tractionTorqueCurve = sim.simParams.TractionTorqueCurve;
			}

            _watchables.AddNew(this, SimEventType.Fuel, sim.fuel);
            _watchables.AddNew(this, SimEventType.Oil, sim.oil);
            _watchables.AddNew(this, SimEventType.Sand, sim.sand);
            _watchables.AddNew(this, SimEventType.EngineTemp, sim.engineTemp);
            _watchables.AddNew(this, SimEventType.EngineRPMGauge, GetEngineRPMGauge);
            _watchables.AddNew(this, SimEventType.Amperage, GetAmperage);
            //_watchables.AddNew(this, SimEventType.Fan, () => FanOn);
            _watchables.AddNew(this, SimEventType.MUConnected, () => MULampState);
			_watchables.AddNew(this, SimEventType.MUConnected, () => (MULampState == LocoSimulationEvents.Amount.Mid) ? 1 : 0);
        }

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

			if (HasCompressorControl)
			{
				SetCompressorControl(0f);
			}
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
			base.Update();

            train.brakeSystem.compressorRunning = (!HasCompressorControl || (_CompressorControl >= 0.5f)) && EngineRunning;
			
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
