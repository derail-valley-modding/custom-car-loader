using System;
using System.Collections.Generic;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Utility
{
    public class BrakeCarKeyboardInputRouter : CarKeyboardInputBase
    {
		public BrakeCarController Ctrl = null;
		protected List<InputDescriptor> Inputs = new List<InputDescriptor>();

		protected virtual void Start()
		{
			Inputs.Add(
				new InputDescriptor(KeyBindings.increaseIndependentBrakeKeys, KeyBindings.decreaseIndependentBrakeKeys, 1, Ctrl.GetTargetIndependentBrake, Ctrl.SetIndependentBrake)
			);

			if (Ctrl.HasAutomaticBrakeHandle)
			{
				Inputs.Add(
					new InputDescriptor(KeyBindings.increaseBrakeKeys, KeyBindings.decreaseBrakeKeys, 1, Ctrl.GetTargetTrainBrake, Ctrl.SetTrainBrake)
				);
			}
		}

		protected virtual void Update()
		{
			if ((Time.timeScale <= float.Epsilon) && (Time.deltaTime <= float.Epsilon))
			{
				return;
			}

			if (Inputs == null)
			{
				return;
			}

			if (!InputFocusManager.Instance.hasKeyboardFocus)
			{
				foreach (var input in Inputs)
				{
					CheckInput(input);
				}
			}

			foreach (var input in Inputs)
			{
				ApplyInput(input);
			}
		}


		protected void CheckInput(InputDescriptor input)
		{
			TrySmoothInputSpeed(
				input.IncreaseKeys,
				input.DecreaseKeys,
				ref input.CurrentChangeSpeed,
				input.MaxChangeSpeed,
				ref input.CurrentChangeDerivative);
		}

		protected void ApplyInput(InputDescriptor input)
		{
			SetTargetValue(
				new SetValueDelegate(input.SetValueFunc),
				input.GetValueFunc(),
				ref input.CurrentChangeSpeed,
				input.Min,
				input.Max,
				ref input.CurrentChangeDerivative);
		}
	}
}
