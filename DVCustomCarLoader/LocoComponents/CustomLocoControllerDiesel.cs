using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV;
using DV.ServicePenalty;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLocoControllerDiesel : 
		CustomLocoController<
			CustomLocoSimDiesel,
			DamageControllerCustomDiesel,
			CustomDieselSimEvents>
    {
		public bool Backlight { get; set; }
		public bool FanOn { get; set; }

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

		public float FuelLevel => sim.fuel.value;
		public float OilLevel => sim.oil.value;
		public float SandLevel => sim.sand.value;
		public bool SandersOn => sim.sandOn;

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
			sim.sandOn = (value > 0f);
			base.SetSanders(value);
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

		public override void SetThrottle( float throttleLever )
		{
			base.SetThrottle(throttleLever);
		}


		//protected override void Awake()
		//{
		//	base.Awake();
			
		//	// TODO: MU, save state
		//	//MultipleUnitModule component = base.GetComponent<MultipleUnitModule>();
		//	//base.gameObject.AddComponent<LocoStateSaveDiesel>().Initialize(sim, damageController, this, carVisitChecker, component);
		//}


		public override float GetTractionForce()
		{
			float num = (sim.engineRPM.value > 0f) ? tractionTorqueCurve.Evaluate(GetSpeedKmH() / sim.engineRPM.value) : 0f;
			float num2 = (Mathf.Sign(GetForwardSpeed() * reverser) > 0f) ? num : 1f;
			return sim.engineRPM.value * num2 * tractionTorqueMult;
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
				gameObject.AddComponent<LocoKeyboardInputDiesel>().control = this;
			}
		}

		public override void Update()
		{
			base.Update();
			UpdateSimSpeed();
			UpdateSimThrottle();
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
	}
}
