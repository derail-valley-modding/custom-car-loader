using System;
using System.Reflection;
using CCL_GameScripts.CabControls;
using DV.CabControls;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CabInputRelay : MonoBehaviour
    {
		public CabInputType Binding;
		public float MapMin = 0;
		public float MapMax = 1;
		public bool AbsPosition = false;

		protected float ControlPositionToBoundValue( float pos )
        {
			float mapped = Extensions.Mapf(0, 1, MapMin, MapMax, pos);
			return AbsPosition ? Mathf.Abs(mapped) : mapped;
		}

		protected float BoundValueToControlPosition( float value )
		{
			return Extensions.Mapf(MapMin, MapMax, 0, 1, value);
		}

		private ControlImplBase __control;
		public ControlImplBase Control
        {
			get => __control;
			set
            {
				if( __control )
				{
					__control.ValueChanged -= OnValueChanged;
					__control.Used -= OnUsed;
				}

				__control = value;

				if( __control )
				{
					initializedField = AccessTools.Field(__control.GetType(), "isInitialized");
					__control.ValueChanged += OnValueChanged;
					__control.Used += OnUsed;
				}
            }
        }

		public bool Initialized { get; protected set; } = false;
		protected Action<float> SetNewValue = null;
		protected Func<float> GetTargetValue = null;
		private float lastValue;

		private FieldInfo initializedField = null;
		protected bool IsControlInitialized
        {
			get
            {
				if( __control )
                {
					if( initializedField != null )
					{
						if( initializedField.GetValue(__control) is bool b )
						{
							return b;
						}
						else
						{
							Main.Warning($"Failed to get initialized field from control {__control.GetType()}");
						}
					}
                }
				return true;
            }
        }

		public float Value
		{
			get
			{
				if( Control )
				{
					return ControlPositionToBoundValue(Control.Value);
				}
				else return MapMin;
			}
			set
			{
				if( Control )
				{
					Control.SetValue(BoundValueToControlPosition(value));
				}
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
			SetNewValue?.Invoke(ControlPositionToBoundValue(e.newValue));
        }

		private void OnUsed()
        {
			if( __control is ButtonBase button )
            {
				SetNewValue?.Invoke(button.IsOn ? MapMin : MapMax);
            }
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
			if( (GetTargetValue != null) && IsControlInitialized )
			{
				float target = GetTargetValue();
				if( (target != lastValue) && !Control.IsGrabbedOrHoverScrolled() )
				{
					Control.SetValue(BoundValueToControlPosition(target));
					lastValue = target;
				}
			}
        }
    }
}