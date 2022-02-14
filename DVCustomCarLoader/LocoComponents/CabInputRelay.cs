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
					newControl = true;
				}
            }
        }

		public bool Initialized { get; protected set; } = false;
		public event EventHandler<float> ValueChanged;
		protected Func<float> GetTargetValue = null;
		private float lastValue;

		private bool newControl = true;
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

		public void SetIOHandlers(Action<float> onUserInput, Func<float> getFeedback = null)
        {
			ValueChanged += (s, e) => onUserInput(e);
			GetTargetValue = getFeedback;
			if (getFeedback != null)
            {
				lastValue = getFeedback();
            }

			Initialized = true;
        }

		public void SetIOHandlers(EventHandler<float> onUserInput, Func<float> getFeedback = null)
        {
			ValueChanged += onUserInput;
			GetTargetValue = getFeedback;
			if (getFeedback != null)
			{
				lastValue = getFeedback();
			}

			Initialized = true;
        }

		public void AddListener(EventHandler<float> onUserInput)
        {
			ValueChanged += onUserInput;
			Initialized = true;
        }

		private void OnValueChanged( ValueChangedEventArgs e )
        {
			if (newControl) return;
			ValueChanged?.Invoke(this, ControlPositionToBoundValue(e.newValue));
        }

		private void OnUsed()
        {
			if( __control is ButtonBase button )
            {
				ValueChanged?.Invoke(this, button.IsOn ? MapMin : MapMax);
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

			if (newControl && IsControlInitialized)
            {
				Control.SetValue(BoundValueToControlPosition(lastValue));
				newControl = false;
				return;
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

		private static readonly FieldInfo RotaryHingeField = AccessTools.Field(typeof(RotaryBase), "hj");
		private static readonly MethodInfo ControlAcceptValue = AccessTools.Method(typeof(ControlImplBase), "AcceptSetValue");
		private static readonly MethodInfo ControlRequestUpdate = AccessTools.Method(typeof(ControlImplBase), "RequestValueUpdate");

		public void DeflectWithSpring( float value )
        {
			if( !Control ) return;

			if( Control is LeverBase lever )
            {
				lever.MoveLeverAndReset(value);
            }
			else if( (value > 0) && Control is ButtonBase button )
            {
				button.Use();
            }
			else if( Control is RotaryBase rotary )
            {
				HingeJoint hj = RotaryHingeField?.GetValue(rotary) as HingeJoint;
				if( hj )
                {
					JointSpring spring = hj.spring;
					ControlAcceptValue?.Invoke(rotary, new object[] { value });
					ControlRequestUpdate?.Invoke(rotary, new object[] { value });

					spring.targetPosition = 0f;
					hj.spring = spring;
				}
			}
        }
    }
}