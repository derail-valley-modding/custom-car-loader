using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.CabControls;
using DV;
using DV.ServicePenalty;
using DV.Util.EventWrapper;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLocoControllerDiesel : 
		CustomLocoController<
			CustomLocoSimDiesel,
			DamageControllerCustomDiesel,
			CustomDieselSimEvents>,
		IFusedLocoController
    {
		private bool fanOn;

		public void SetFan( float value )
        {
			bool lastState = fanOn;
			fanOn = value > 0.5f;
			if( lastState ^ fanOn ) FanChanged.Invoke(fanOn);
        }

		public float GetFan() => fanOn ? 1 : 0;

		protected List<CabInputRelay> HornRelays = new List<CabInputRelay>();

        public float EngineRPM => sim.engineRPM.value;
		public float EngineRPMGauge => (sim.engineOn) ? (12.5f + sim.engineRPM.value * 100f) : 0f;
		public float EngineTemp => sim.engineTemp.value;

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

		public float GetAmperage()
        {
			float traction = (reverser == 0) ? 0 : GetTractionForce();
			return 0.00387571f * traction;
		}

		public float FuelLevel => sim.fuel.value;
		public float OilLevel => sim.oil.value;
		public float SandLevel => sim.sand.value;
		public bool SandersOn => sim.sandOn;

		public event_<bool> SandersChanged;
		public event_<bool> FanChanged;

        public override bool Bind( SimEventType eventType, ILocoEventAcceptor listener )
        {
			switch( eventType )
			{
				case SimEventType.SandDeploy:
					SandersChanged.Register(listener.BoolHandler);
					return true;

				case SimEventType.Fan:
					FanChanged.Register(listener.BoolHandler);
					return true;
			}
            return base.Bind(eventType, listener);
        }

        public override Func<float> GetIndicatorFunc( CabIndicatorType indicatedType )
        {
			switch( indicatedType )
            {
				case CabIndicatorType.Fuel:
					return () => FuelLevel;

				case CabIndicatorType.Oil:
					return () => OilLevel;

				case CabIndicatorType.Sand:
					return () => SandLevel;

				case CabIndicatorType.EngineTemp:
					return () => EngineTemp;

				case CabIndicatorType.EngineRPM:
					return () => EngineRPMGauge;

				case CabIndicatorType.Amperage:
					return GetAmperage;

				default:
					return base.GetIndicatorFunc(indicatedType);
			}
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
					inputRelay.SetIOHandlers(SetFan, GetFan);
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
				SandersChanged.Invoke(sim.sandOn);
				base.SetSanders(value);
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
				Main.Log("Added keyboard input to car");
			}

			train.brakeSystem.compressorProductionRate = sim.simParams.AirCompressorRate;
		}

        #endregion

        #region Update Loop

        public override void Update()
		{
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
				if( KeyBindings.increaseSandKeys.IsDown() && !SandersOn )
                {
					SetSanders(1);
                }
				if( KeyBindings.decreaseSandKeys.IsDown() && SandersOn )
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
