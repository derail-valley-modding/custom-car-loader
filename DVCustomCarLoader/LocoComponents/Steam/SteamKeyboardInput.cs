using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class SteamKeyboardInput : LocoKeyboardInputSteam
    {
		public CustomLocoControllerSteam Ctrl = null;
		protected InputDescriptor[] Inputs = null;

		private void Start()
        {
			var baseStart = AccessTools.Method(typeof(LocoKeyboardInputSteam), "Start");
			baseStart.Invoke(this, new object[0]);

			Inputs = new InputDescriptor[]
			{
				new InputDescriptor(KeyBindings.increaseSandKeys, KeyBindings.decreaseSandKeys, 1, Ctrl.GetSanderValve, Ctrl.SetSanders),
				new InputDescriptor(KeyBindings.openFireDoorKeys, KeyBindings.closeFireDoorKeys, 1, Ctrl.GetFireDoor, Ctrl.SetFireDoor),
				new InputDescriptor(KeyBindings.openInjectorKeys, KeyBindings.closeInjectorKeys, 1, Ctrl.GetInjector, Ctrl.SetInjector),
				new InputDescriptor(KeyBindings.increaseDraftKeys, KeyBindings.decreaseDraftKeys, 1, Ctrl.GetDamper, Ctrl.SetDamper),
				new InputDescriptor(KeyBindings.increaseBlowerKeys, KeyBindings.decreaseBlowerKeys, 1, Ctrl.GetBlower, Ctrl.SetBlower),
				new InputDescriptor(KeyBindings.increaseWaterDumpKeys, KeyBindings.decreaseWaterDumpKeys, 1, Ctrl.GetWaterDump, Ctrl.SetWaterDump),
				new InputDescriptor(KeyBindings.increaseSteamReleaseKeys, KeyBindings.decreaseSteamReleaseKeys, 1, Ctrl.GetSteamDump, Ctrl.SetSteamDump),
			};
        }

        protected override void Update()
        {
            base.Update();

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


		private void CheckInput(InputDescriptor input)
        {
			TrySmoothInputSpeed(
				input.IncreaseKeys, 
				input.DecreaseKeys, 
				ref input.CurrentChangeSpeed, 
				input.MaxChangeSpeed, 
				ref input.CurrentChangeDerivative);
		}

		private void ApplyInput(InputDescriptor input)
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