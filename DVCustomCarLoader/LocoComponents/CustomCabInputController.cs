using System;
using System.Collections;
using System.Linq;
using CCL_GameScripts.CabControls;
using DV.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomCabInputController : CabInput
    {
		protected ICabControlAcceptor[] controlAcceptors;
		public CabInputRelay[] Relays;

		protected virtual void OnEnable()
		{
			var car = TrainCar.Resolve(gameObject);
			if( car == null || !car )
			{
				Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
				return;
			}

			controlAcceptors = 
				car.gameObject.GetComponentsByInterface<ICabControlAcceptor>()
				.Concat(gameObject.GetComponentsByInterface<ICabControlAcceptor>())
				.ToArray();

			if( controlAcceptors.Length == 0 )
			{
				Main.Error("Couldn't find any components accepting cab input");
				return;
			}

			Relays = GetComponentsInChildren<CabInputRelay>(true);
			Main.Log($"CustomCabInput Start - {Relays.Length} controls");
			foreach( CabInputRelay relay in Relays )
			{
				foreach( var receiver in controlAcceptors )
                {
					if( receiver.AcceptsControlOfType(relay.Binding) )
					{
						receiver.RegisterControl(relay);
					}
                }
			}
		}
	}

	public class CabInputRelay : MonoBehaviour
    {
		public CabInputType Binding;

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

		public bool Initialized { get; protected set; } = false;
		protected Action<float> SetNewValue = null;
		protected Func<float> GetTargetValue = null;
		private float lastValue;

		public float Value
        {
			get => Control ? Control.Value : 0;
			set
			{
				if( Control ) Control.SetValue(value);
			}
        }

		public void SetIOHandlers( Action<float> onUserInput, Func<float> getFeedback = null )
        {
			SetNewValue = onUserInput;
			GetTargetValue = getFeedback;

			Initialized = true;
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
				if( !Control )
                {
					enabled = false;
					Main.Warning($"Input relay {name} is missing ControlImplBase");
					return;
                }
            }

			// handle feedback from the loco controller (eg keyboard controls)
			if( GetTargetValue != null )
			{
				float target = GetTargetValue();
				if( (target != lastValue) && !Control.IsGrabbedOrHoverScrolled() )
				{
					Control.SetValue(target);
					lastValue = target;
				}
			}
		}
    }
}