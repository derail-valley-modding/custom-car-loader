using CCL_GameScripts.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomLocoAudioSteam : CustomLocoAudio<CustomLocoControllerSteam, CustomLocoSimEventsSteam, CustomDamageControllerSteam>
    {
		protected CustomChuffController chuffController;

		// Cylinders
		[ProxyField]
		public Transform playLeftCylAt;
		[ProxyField]
		public Transform playRightCylAt;
		[ProxyField]
		public AudioClip[] cylClipsSlow;

		// Chimney
		[ProxyField]
		public Transform playChimneyAt;
		[ProxyField]
		public AudioClip[] chimneyClipsSlow;

		// Chuff
		[ProxyField]
		public LayeredAudio valveGearLayered;
		[ProxyField]
		public LayeredAudio steamChuffsLayered;
		[ProxyField]
		public AnimationCurve individualToFastLoopTransition;

		// Injector
		[ProxyField]
		public LayeredAudio waterInFlowLayered;
		[ProxyField]
		public AnimationCurve waterInVolumeEdgeMultiplier;

		// Water Dump
		[ProxyField]
		public LayeredAudio waterDumpFlowLayered;
		[ProxyField]
		public AnimationCurve waterDumpVolumeEdgeMultiplier;

		// Air
		[ProxyField]
		public LayeredAudio blowerLayered;
		[ProxyField]
		public LayeredAudio draftLayered;

		// Steam Release
		[ProxyField]
		public LayeredAudio steamReleaseLayeredLeft;
		[ProxyField]
		public LayeredAudio steamReleaseLayeredRight;

		[ProxyField]
		public LayeredAudio steamSafetyReleaseLayered;
		[ProxyField]
		public LayeredAudio pressureLeakLayered;
		[ProxyField]
		public AnimationCurve steamReleaseVolumeEdgeMultiplier;

		// Whistle
		[ProxyField]
		public LayeredAudio whistleAudio;
		[ProxyField]
		public AnimationCurve whistleVolumeEdgeMultiplier;

		// Misc
		[ProxyField]
		public LayeredAudio fireLayered;
		[ProxyField]
		public LayeredAudio sandAudio;


		private int lastPlayedChuff;
		private bool loopsPlaying;

		#region Setup & Teardown

		protected override void SetupLocoLogic(TrainCar car)
		{
			base.SetupLocoLogic(car);
			chuffController = car.GetComponent<CustomChuffController>();
		}

		protected override void UnsetLocoLogic()
		{
			base.UnsetLocoLogic();
			chuffController = null;
		}

		protected override void ResetAllAudio()
		{
			base.ResetAllAudio();

			if (valveGearLayered) valveGearLayered.Reset();
			if (steamChuffsLayered) steamChuffsLayered.Reset();

			if (waterInFlowLayered) waterInFlowLayered.Reset();
			if (waterDumpFlowLayered) waterDumpFlowLayered.Reset();

			if (blowerLayered) blowerLayered.Reset();
			if (draftLayered) draftLayered.Reset();

			if (steamReleaseLayeredLeft) steamReleaseLayeredLeft.Reset();
			if (steamReleaseLayeredRight) steamReleaseLayeredRight.Reset();
			if (steamSafetyReleaseLayered) steamSafetyReleaseLayered.Reset();
			
			if (pressureLeakLayered) pressureLeakLayered.Reset();
			if (whistleAudio) whistleAudio.Reset();
			if (fireLayered) fireLayered.Reset();
		}

		protected void StopLoops()
        {
			valveGearLayered.Stop();
			steamChuffsLayered.Stop();
			loopsPlaying = false;
        }

		protected void StartLoops()
        {
			StopLoops();
			valveGearLayered.Play();
			steamChuffsLayered.Play();
			loopsPlaying = true;
        }

        #endregion

        #region Updates

        protected override void Update()
        {
            base.Update();

			if (!timeFlow || !Car) return;

			if (chuffController)
			{
				ChuffUpdate(chuffController.currentChuff, chuffController.chuffPower, chuffController.chuffKmh);
			}

			// Injector
			if (waterInFlowLayered)
            {
				if ((customLocoController.GetInjector() > 0) && (customLocoController.BoilerWater < customLocoController.MaxBoilerWater) && (customLocoController.TenderWater > 0))
                {
					float volume = customLocoController.GetInjector() * waterInVolumeEdgeMultiplier.Evaluate(customLocoController.BoilerWaterPercent);
					waterInFlowLayered.Set(volume);
                }
				else
                {
					waterInFlowLayered.Set(0);
                }
            }

			// Water Dump
			if (waterDumpFlowLayered)
            {
				if ((customLocoController.GetWaterDump() > 0) && (customLocoController.BoilerWater > 0))
                {
					waterDumpFlowLayered.Set(customLocoController.GetWaterDump() * waterDumpVolumeEdgeMultiplier.Evaluate(customLocoController.BoilerWaterPercent));
                }
				else
                {
					waterDumpFlowLayered.Set(0);
                }
            }

			// Blower
			if (blowerLayered)
            {
				blowerLayered.Set(customLocoController.BlowerFlow);
            }

			// Draft
			if (draftLayered)
            {
				draftLayered.Set(customLocoController.DraftFlow);
            }

			// Steam Dump
			if (steamReleaseLayeredLeft && steamReleaseLayeredRight)
            {
				if ((customLocoController.GetSteamDump() > 0) && (customLocoController.BoilerPressure > 0))
				{
					float volume = customLocoController.GetSteamDump() * steamReleaseVolumeEdgeMultiplier.Evaluate(customLocoController.BoilerPressurePercent);
					steamReleaseLayeredLeft.Set(volume);
					steamReleaseLayeredRight.Set(volume);
				}
				else
                {
					steamReleaseLayeredLeft.Set(0);
					steamReleaseLayeredRight.Set(0);
                }
            }

			// Safety Release
			if (steamSafetyReleaseLayered)
            {
				steamSafetyReleaseLayered.Set(customLocoController.SafetyValve);
            }

			// Pressure Leak
			if (pressureLeakLayered)
            {
				if (customLocoController.BoilerPressure > 0)
				{
					pressureLeakLayered.Set(customLocoController.PressureLeak * steamReleaseVolumeEdgeMultiplier.Evaluate(customLocoController.BoilerPressurePercent));
				}
				else
                {
					pressureLeakLayered.Set(0);
                }
            }

			// Whistle
			if (whistleAudio)
            {
				if (customLocoController.BoilerPressure > 0)
                {
					whistleAudio.Set(customLocoController.GetWhistle() * whistleVolumeEdgeMultiplier.Evaluate(customLocoController.BoilerPressurePercent));
                }
				else
                {
					whistleAudio.Set(0);
                }
            }

			// Fire
			if (fireLayered)
            {
				float volume = customLocoController.IsFireOn ? customLocoController.FireTempPercent : 0;
				fireLayered.Set(volume);
            }

			// Sand
			if (sandAudio)
            {
				sandAudio.Set(customLocoController.GetSandersFlow());
            }
        }

		protected void ChuffUpdate(int currentChuff, float power, float wheelKmh)
        {
			float individualLvl = individualToFastLoopTransition.Evaluate(wheelKmh);

			if ((individualLvl > 0.01f) && (currentChuff != lastPlayedChuff))
            {
				lastPlayedChuff = currentChuff;
				Transform playLocation = (currentChuff % 2 == 0) ? playLeftCylAt : playRightCylAt;
				
				float volume = Mathf.Clamp01(power * 2) * individualLvl;
				cylClipsSlow.Play(playLocation.position, volume, 1, 0, 15, 500, default, AudioManager.e.cabGroup, playLocation);

				volume = Mathf.Clamp01(power) * individualLvl;
				chimneyClipsSlow.Play(playChimneyAt.position, volume, 1, 0, 20, 500, default, AudioManager.e.cabGroup, playChimneyAt);
			}

			float continuousLvl = 1 - individualLvl;
			if (continuousLvl > 0.02f)
            {
				if (!loopsPlaying)
                {
					StartLoops();
                }

				valveGearLayered.Set(wheelKmh);
				valveGearLayered.masterVolume = Mathf.Clamp01(continuousLvl);

				steamChuffsLayered.Set(wheelKmh);
				steamChuffsLayered.masterVolume = Mathf.Clamp01(continuousLvl * power * 2);
            }
			else if (loopsPlaying && (continuousLvl < 0.01f))
            {
				StopLoops();
            }
        }

        #endregion
    }
}
