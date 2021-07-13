using System.Collections;
using CCL_GameScripts;
using DV.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomCabInput : CabInput
    {
		protected CustomLocoController locoController;
		protected CabInputSetup config;

		//public GameObject brake;
		//public GameObject independentBrake;
		//public GameObject reverser;
		//public GameObject throttle;

		public bool reverserSnap;

		protected ControlImplBase brakeControl;
		protected ControlImplBase independentBrakeControl;
		protected ControlImplBase reverserControl;
		protected ControlImplBase throttleControl;

		protected bool initialized;

		protected float prevControllerBrake;
		protected float prevControllerIndependentBrake;
		protected float prevControllerReverser;
		protected float prevControllerThrottle;

		private IEnumerator Init()
		{
			yield return null;
			yield return null;

			var car = TrainCar.Resolve(gameObject);
			if( car == null || !car )
            {
				Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
				yield break;
            }

			locoController = car.GetComponent<CustomLocoController>();
			config = gameObject.GetComponent<CabInputSetup>();

			if( !locoController )
            {
				Main.Error("Couldn't find custom lococontroller");
				yield break;
            }

			if( !config )
            {
				Main.Error("Couldn't find cab input setup");
				yield break;
            }

			if( config.Brake )
			{
				brakeControl = config.Brake.GetComponent<ControlImplBase>();
				brakeControl.SetValue(locoController.targetBrake);
			}

			if( config.IndependentBrake )
			{
				independentBrakeControl = config.IndependentBrake.GetComponent<ControlImplBase>();
				independentBrakeControl.SetValue(locoController.targetIndependentBrake);
			}

			if( config.Throttle )
			{
				throttleControl = config.Throttle.GetComponent<ControlImplBase>();
				throttleControl.SetValue(locoController.targetThrottle);
			}

			if( config.Reverser ) 
			{
                reverserControl = config.Reverser.GetComponent<ControlImplBase>();
				reverserControl.SetValue((locoController.reverser + 1f) / 2f);
			}

			yield return WaitFor.SecondsRealtime(1f);

			if( brakeControl )
			{
				brakeControl.ValueChanged += (ValueChangedEventArgs e) =>
					locoController.SetBrake(e.newValue);
			}

			if( independentBrakeControl )
			{
				independentBrakeControl.ValueChanged += (ValueChangedEventArgs e) =>
					locoController.SetIndependentBrake(e.newValue);
			}

			if( throttleControl )
			{
				throttleControl.ValueChanged += (ValueChangedEventArgs e) =>
					locoController.SetThrottle(e.newValue);
			}

			if( reverserControl )
			{
				if( reverserSnap )
                {

                }
				reverserControl.ValueChanged += (ValueChangedEventArgs e) => 
					locoController.SetReverser(Mathf.RoundToInt(e.newValue * 2f) - 1);
			}

			initialized = true;
			yield break;
		}

		protected virtual void OnDestroy()
		{
			if( locoController )
			{
				locoController.SetThrottle(throttleControl.Value);
			}
		}

		protected virtual void OnEnable()
		{
			StartCoroutine(Init());
		}

		protected virtual void Update()
		{
			if( !initialized )
			{
				return;
			}

			float targetThrottle = locoController.targetThrottle;
			if( targetThrottle != prevControllerThrottle && !throttleControl.IsGrabbedOrHoverScrolled() )
			{
				throttleControl.SetValue(targetThrottle);
				prevControllerThrottle = targetThrottle;
			}

			float targetBrake = locoController.targetBrake;
			if( targetBrake != prevControllerBrake && !brakeControl.IsGrabbedOrHoverScrolled() )
			{
				brakeControl.SetValue(targetBrake);
				prevControllerBrake = targetBrake;
			}

			float targetIndependentBrake = locoController.targetIndependentBrake;
			if( targetIndependentBrake != prevControllerIndependentBrake && !independentBrakeControl.IsGrabbedOrHoverScrolled() )
			{
				independentBrakeControl.SetValue(targetIndependentBrake);
				prevControllerIndependentBrake = targetIndependentBrake;
			}

			float curReverser = locoController.reverser;
			if( curReverser != prevControllerReverser && !reverserControl.IsGrabbedOrHoverScrolled() )
			{
				reverserControl.SetValue((curReverser + 1f) / 2f);
				prevControllerReverser = curReverser;
			}
		}
	}
}