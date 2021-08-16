using System.Collections;
using System.Linq;
using UnityEngine;
using CCL_GameScripts;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLocoAudioDiesel : 
        CustomLocoAudio<
            CustomLocoControllerDiesel,
            CustomDieselSimEvents,
            DamageControllerCustomDiesel>
    {
		// Engine/Traction Sounds
		[ProxyField]
		public Transform playEngineAt;

		[ProxyField]
		public LayeredAudio engineAudio;
		[ProxyField]
		public LayeredAudio enginePistonAudio;
		[ProxyField]
		public LayeredAudio electricMotorAudio;

		[ProxyField]
		public LayeredAudio engineDamageAudio;
		[ProxyField]
		public AnimationCurve engineDamageToMasterVolumeCurve;

		[ProxyField]
		public AudioClip engineOnClip;
		[ProxyField]
		public AudioClip engineOffClip;

		[ProxyField]
		public float prevLocoEngineRpm;
		[ProxyField]
		public float enginePistonTargetVolume;
		[ProxyField]
		public float neutralEnginePistonVolume = 0.4f;

		// Reverser Lever
		[ProxyField]
		public AudioClip[] reverserClips;
		[ProxyField]
		public Transform playReverserAt;
		[ProxyField]
		public float reverserVolume = 1f;
		[ProxyField]
		public float reverserPitch = 1f;

		// Sanders
		[ProxyField]
		public Transform playSandAt;
		[ProxyField]
		public LayeredAudio sandAudio;

		// Horn
		[ProxyField]
		public LayeredAudio hornAudio;

		// Engine loop
		protected bool IsEngineVolumeFadeActive;
		protected Coroutine engineAudioCoro;

        #region Setup & Teardown

        protected override void SetupLocoLogic( TrainCar car )
        {
            base.SetupLocoLogic(car);

            simEvents.EngineRunningChanged.Register(OnEngineStateChanged);
			customLocoController.OnReverserChanged += PlayReverser;
        }

        protected override void UnsetLocoLogic()
        {
			if( simEvents != null )
			{
				simEvents.EngineRunningChanged.Unregister(OnEngineStateChanged);
			}

			if( engineAudioCoro != null )
            {
				StopCoroutine(engineAudioCoro);
            }

			base.UnsetLocoLogic();
        }

        public override void SetupForCar( TrainCar car )
        {
			base.SetupForCar(car);

			// Setup Horn
        }

        protected override void ReturnToPool()
        {
			UnsetLocoLogic();
            base.ReturnToPool();
        }

		protected override void ResetAllAudio()
		{
			base.ResetAllAudio();
			float newVolume = customLocoController.EngineRunning ? 1 : 0;

			if( engineAudio )
			{
				engineAudio.Reset();
				engineAudio.masterVolume = newVolume;
			}
			if( enginePistonAudio )
			{
				enginePistonAudio.Reset();
				enginePistonAudio.masterVolume = newVolume;
			}
			if( electricMotorAudio )
			{
				electricMotorAudio.Reset();
				electricMotorAudio.masterVolume = newVolume;
			}
			if( engineDamageAudio )
			{
				engineDamageAudio.Reset();
				engineDamageAudio.masterVolume = 0f;
			}
			if( sandAudio )
			{
				sandAudio.Reset();
			}
			if( hornAudio )
			{
				hornAudio.Reset();
			}
		}

        #endregion

        #region Game Audio Triggers

        protected override void Update()
        {
            base.Update();

			if( !timeFlow || !Car ) return;

			float engineRPM = customLocoController.EngineRPM;
			engineAudio.Set(engineRPM);
			enginePistonAudio.Set(engineRPM);
			electricMotorAudio.Set(locoController.GetSpeedKmH() / 100f);

			if( engineDamageAudio )
            {
				engineDamageAudio.Set(engineRPM);
				engineDamageAudio.masterVolume =
					engineDamageToMasterVolumeCurve.Evaluate(customDmgController.engine.DamagePercentage);
            }

			if( sandAudio )
            {
				sandAudio.Set(locoController.GetSandersFlow());
			}

			if( customLocoController.EngineRunning && !IsEngineVolumeFadeActive )
            {
				float deltaRPM = engineRPM - prevLocoEngineRpm;
				if( deltaRPM > -0.0001f )
				{
					enginePistonTargetVolume = deltaRPM * 1000f;

					if( enginePistonTargetVolume < neutralEnginePistonVolume )
					{
						enginePistonTargetVolume = neutralEnginePistonVolume;
					}
				}
				else
                {
					enginePistonTargetVolume = 0f;
                }

				enginePistonTargetVolume = Mathf.Clamp01(enginePistonTargetVolume);

				// Ramp piston volume up/down to target
				if( enginePistonAudio.masterVolume < enginePistonTargetVolume )
                {
					enginePistonAudio.masterVolume += 0.001f + (enginePistonAudio.masterVolume / 16f);
                }
				else if( enginePistonAudio.masterVolume > enginePistonTargetVolume )
                {
					enginePistonAudio.masterVolume -= enginePistonAudio.masterVolume / 32f;
                }

				enginePistonAudio.masterVolume = Mathf.Clamp01(enginePistonAudio.masterVolume);
				prevLocoEngineRpm = engineRPM;
            }
        }

		protected void PlayReverser()
		{
			if( reverserClips != null && reverserClips.Length != 0 )
			{
				Transform playAt = playReverserAt ? playReverserAt : transform;
				reverserClips.Play(playAt.position, reverserVolume, 1f, 0f, 1f, 500f, default(AudioSourceCurves), AudioManager.e.cabGroup, playAt);
			}
		}

		#endregion

		#region Engine Loop

		private void OnEngineStateChanged( bool state )
		{
			if( engineAudioCoro != null )
			{
				StopCoroutine(engineAudioCoro);
			}
			engineAudioCoro = StartCoroutine(EngineStartupShutdown(state));
		}

		private IEnumerator EngineStartupShutdown( bool engineTurnedOn )
		{
			IsEngineVolumeFadeActive = true;

			if( engineTurnedOn )
			{
				engineOnClip.Play(playEngineAt.position, 1f, 1f, 0f, 1f, 500f, default(AudioSourceCurves), null, playEngineAt);
			}
			else
			{
				engineOffClip.Play(playEngineAt.position, 1f, 1f, 0f, 1f, 500f, default(AudioSourceCurves), null, playEngineAt);
			}

			float onOffDelay = engineTurnedOn ? (engineOnClip.length * 0.15f) : (engineOffClip.length * 0.1f);
			yield return WaitFor.Seconds(onOffDelay);

			float startTime = Time.realtimeSinceStartup;
			float duration = engineTurnedOn ? 2f : 1f;

			float startEngineVolume = engineAudio.masterVolume;
			float startPistonVolume = enginePistonAudio.masterVolume;
			float startElectricVolume = electricMotorAudio.masterVolume;

			int endEngineVolume = engineTurnedOn ? 1 : 0;
			float endPistonVolume = engineTurnedOn ? neutralEnginePistonVolume : 0f;

			// Ramp engine volumes up/down
			while( true )
			{
				float durationPercent = (Time.realtimeSinceStartup - startTime) / duration;
				engineAudio.masterVolume = Mathf.Lerp(startEngineVolume, endEngineVolume, durationPercent);
				enginePistonAudio.masterVolume = Mathf.Lerp(startPistonVolume, endPistonVolume, durationPercent);
				electricMotorAudio.masterVolume = Mathf.Lerp(startElectricVolume, endEngineVolume, durationPercent);

				if( durationPercent >= 1f )
				{
					break;
				}
				yield return null;
			}

			IsEngineVolumeFadeActive = false;
			engineAudioCoro = null;
			yield break;
		}

		#endregion
	}
}