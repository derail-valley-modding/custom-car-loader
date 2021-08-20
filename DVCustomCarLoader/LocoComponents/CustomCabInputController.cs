using System;
using System.Collections;
using CCL_GameScripts.CabControls;
using DV.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomCabInputController : CabInput
    {
		protected CustomLocoController locoController;
		public CabInputRelay[] Relays;

		protected virtual void OnEnable()
		{
			var car = TrainCar.Resolve(gameObject);
			if( car == null || !car )
			{
				Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
				return;
			}

			locoController = car.gameObject.GetComponent<CustomLocoController>();
			if( !locoController )
			{
				Main.Error("Couldn't find custom lococontroller for cab input");
				return;
			}

			Relays = GetComponentsInChildren<CabInputRelay>(true);
			Main.Log($"CustomCabInput Start - {Relays.Length} controls");
			foreach( CabInputRelay relay in Relays )
			{
				relay.ConnectToController(locoController);
			}
		}
	}

	public class CabInputRelay : MonoBehaviour
    {
		public CabInputType InputBinding;

		private ControlImplBase __control;
		public ControlImplBase Control
        {
			get => __control;
			set
            {
				if( __control ) __control.ValueChanged -= OnValueChanged;
				__control = value;
				if( __control )
				{
					__control.ValueChanged += OnValueChanged;
				}
            }
        }

		public bool Initialized = false;
		protected Action<float> SetNewValue = null;
		protected Func<float> GetTargetValue = null;
		private float lastValue;

		public void ConnectToController( CustomLocoController locoController )
        {
			(SetNewValue, GetTargetValue) = locoController.GetCabControlActions(InputBinding);

			if( SetNewValue == null || GetTargetValue == null )
			{
				// can't run if not connected to anything
				enabled = false;
			}
			else
			{
				Initialized = true;
			}
        }

		private void OnValueChanged( ValueChangedEventArgs e )
        {
            SetNewValue?.Invoke(e.newValue);
        }

		public void Update()
        {
			if( !Initialized ) return;
			if( !Control )
            {
				Control = gameObject.GetComponentInChildren<ControlImplBase>();
            }

			// handle feedback from the loco controller (eg keyboard controls)
			float target = GetTargetValue();
			if( (target != lastValue) && !Control.IsGrabbedOrHoverScrolled() )
			{
				Control.SetValue(target);
				lastValue = target;
			}
		}
    }
}