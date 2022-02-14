using System.Collections;
using UnityEngine;
using CCL_GameScripts.Effects;
using DVCustomCarLoader.LocoComponents;
using DVCustomCarLoader.LocoComponents.DieselElectric;

namespace DVCustomCarLoader.Effects
{
    public class EngineSmokeEmitter : MonoBehaviour, IInitSpecFinalizer<SmokeEmissionSetup>
    {
		protected CustomLocoControllerDiesel locoController;
		protected DamageControllerCustomDiesel dmgController;
		protected CustomLocoSimEvents events;

		// Exhaust Particle Settings
		public ParticleSystem exhaustSmokeParticles;
		public AnimationCurve emissionRatePerThrottleCurve;
		public AnimationCurve startSpeedMultPerThrottleCurve;
		public AnimationCurve maxParticlesPerThrottle;

		public Transform EmissionLocation;
		public bool UseBigDieselParticles = false;
		public float MaxParticlesFalloffSpeed = 0.1f;

		// clapper
		//public float IDLE_FREQUENCY = 3f;
		//public float MAX_FREQUENCY = 5f;
		//public float SMOOTH_TIME_ENG_RPM_MIN = 0.1f;
		//public float SMOOTH_TIME_ENG_RPM_MAX = 0.05f;
		//public float MAX_ANGLE_MAX = -90f;
		//public float MAX_ANGLE_MIN = -20f;
		//public float MIN_ANGLE_MAX = -85f;
		//public float MIN_ANGLE_MIN = 0f;

		// Over temp particle settings
		public ParticleSystem highTemperatureSmokeParticles;
		protected const float UPDATE_HIGH_TEMP_SMOKE_PERIOD = 1f;
		public float OverheatMinTemp = 108f;
		public float OverheatMaxTemp = 120f;

		public float HighTempIntervalMin = 1f;
		public float HighTempIntervalMax = 5f;
		public float HighTempSpeedMin = 2f;
		public float HighTempSpeedMax = 5f;

		// Damage particle settings
		public ParticleSystem damagedEngineSmokeParticles;
		protected const float UPDATE_DAMAGED_ENGINE_SMOKE_PERIOD = 2f;
		public float DamageThreshold = 0.4f;

		public float DamagedParticlesMinSize = 0.5f;
		public float DamagedParticlesMaxSize = 3f;

		protected bool shouldPlayEngineOnParticles;

		#region Initialization & Toggle

		public void FinalizeFromSpec(SmokeEmissionSetup spec)
		{
			EngineSmokeParticles particles;

			if (UseBigDieselParticles)
			{
				particles = ParticleInitializer.AddBigDieselParticles(this, EmissionLocation);
			}
			else
            {
				particles = ParticleInitializer.AddSmallDieselParticles(this, EmissionLocation);
            }

			exhaustSmokeParticles = particles.EngineSmoke;
			highTemperatureSmokeParticles = particles.HighTempSmoke;
			damagedEngineSmokeParticles = particles.DamagedSmoke;

			particles.Root.SetActive(true);
		}

		protected void Awake()
		{
			locoController = GetComponent<CustomLocoControllerDiesel>();
			dmgController = GetComponent<DamageControllerCustomDiesel>();
			events = GetComponent<CustomLocoSimEvents>();

			if (!locoController || !dmgController || !events)
            {
				Main.Warning($"EngineSmokeEmitter on {gameObject.name} couldn't find dependent loco:\n" +
					$"loco - {!!locoController}, dmg - {!!dmgController}, events - {!!events}");
            }
		}

		protected void OnEnable()
		{
			StartCoroutine(HighTemperatureSmokeUpdate(UPDATE_HIGH_TEMP_SMOKE_PERIOD));
			StartCoroutine(DamagedEngineSmokeUpdate(UPDATE_DAMAGED_ENGINE_SMOKE_PERIOD));
			events.EngineRunningChanged.Register(Toggle);
			Toggle(locoController.EngineRunning);
		}

		protected void OnDisable()
		{
			events.EngineRunningChanged.Unregister(Toggle);
			StopAllCoroutines();
		}

		public void Toggle(bool on)
		{
			Main.LogVerbose($"{gameObject.name} Engine Smoke toggle - {on}");

			shouldPlayEngineOnParticles = on;
			if (on)
			{
				exhaustSmokeParticles.Play();
				return;
			}
			exhaustSmokeParticles.Stop();
		}

		#endregion

		#region Updates

		protected void Update()
		{
			if (Time.timeScale <= 1E-45f || Time.deltaTime <= 1E-45f)
			{
				return;
			}

			//float z = clapper.localRotation.eulerAngles.z;
			//if (!shouldPlayEngineOnParticles)
			//{
			//	if (z > 0.0001f)
			//	{
			//		clapper.localRotation = Quaternion.Euler(0f, 0f, Mathf.SmoothDampAngle(z, 0f, ref clapperRefVelo, SMOOTH_TIME_ENG_RPM_MIN));
			//	}
			//	return;
			//}

			float engineRPM = locoController.EngineRPM;

			// clapper clapping update
			//float frequency = Mathf.Lerp(IDLE_FREQUENCY, MAX_FREQUENCY, engineRPM);
			//float wobble = (Mathf.Sin(Time.time * frequency * 2f * Mathf.PI) + 1f) / 2f;
			//float smoothTime = Mathf.Lerp(SMOOTH_TIME_ENG_RPM_MIN, SMOOTH_TIME_ENG_RPM_MAX, engineRPM);
			//float minAngle = Mathf.Lerp(MIN_ANGLE_MIN, MIN_ANGLE_MAX, engineRPM);
			//float maxAngle = Mathf.Lerp(MAX_ANGLE_MIN, MAX_ANGLE_MAX, engineRPM);
			//this.clapper.localRotation = Quaternion.Euler(0f, 0f, Mathf.SmoothDampAngle(z, Mathf.Lerp(minAngle, maxAngle, wobble), ref this.clapperRefVelo, smoothTime));

			float newMaxParticles = maxParticlesPerThrottle.Evaluate(engineRPM);
			if (newMaxParticles < exhaustSmokeParticles.main.maxParticles)
			{
				newMaxParticles = Mathf.Lerp(exhaustSmokeParticles.main.maxParticles, newMaxParticles, MaxParticlesFalloffSpeed);
			}

			ParticleSystem.MainModule main = exhaustSmokeParticles.main;
			main.startSpeed = startSpeedMultPerThrottleCurve.Evaluate(engineRPM);
			main.maxParticles = (int)newMaxParticles;

			var emission = exhaustSmokeParticles.emission;
			emission.rateOverTime = emissionRatePerThrottleCurve.Evaluate(engineRPM);
			//maxParticlesDebug = exhaustSmokeParticles.main.maxParticles;
		}

		private IEnumerator HighTemperatureSmokeUpdate(float updatePeriod)
		{
			WaitForSeconds timeout = WaitFor.Seconds(updatePeriod);
			yield return timeout;
			while (true)
			{
				float overheatLevel = Mathf.InverseLerp(OverheatMinTemp, OverheatMaxTemp, locoController.EngineTemp);
				bool shouldPlayHighTemperatureParticles = overheatLevel > 0f;
				if (shouldPlayHighTemperatureParticles)
				{
					var highTempMain = highTemperatureSmokeParticles.main;
					highTempMain.startSpeed = Mathf.Lerp(HighTempSpeedMin, HighTempSpeedMax, overheatLevel);
				}

				bool correctActiveInHierarchy = highTemperatureSmokeParticles.gameObject.activeInHierarchy == shouldPlayHighTemperatureParticles;
				bool correctPlayingParticles = highTemperatureSmokeParticles.isPlaying == shouldPlayHighTemperatureParticles;
				if (!correctActiveInHierarchy || !correctPlayingParticles)
				{
					yield return new WaitForSeconds(Random.Range(HighTempIntervalMin, HighTempIntervalMax));

					if (!correctActiveInHierarchy && highTemperatureSmokeParticles.particleCount == 0)
					{
						highTemperatureSmokeParticles.gameObject.SetActive(shouldPlayHighTemperatureParticles);
					}

					if (!correctPlayingParticles)
					{
						if (shouldPlayHighTemperatureParticles)
						{
							highTemperatureSmokeParticles.Play();
						}
						else
						{
							highTemperatureSmokeParticles.Stop();
						}
					}
				}
				yield return timeout;
			}
		}

		private IEnumerator DamagedEngineSmokeUpdate(float updatePeriod)
		{
			WaitForSeconds timeout = WaitFor.Seconds(updatePeriod);
			yield return timeout;
			while (true)
			{
				float damageLevel = Mathf.InverseLerp(DamageThreshold, 1f, dmgController.engine.DamagePercentage);
				bool shouldPlayDamageStartupParticles = shouldPlayEngineOnParticles && (damageLevel > 0f);
				if (shouldPlayDamageStartupParticles)
				{
					var damagedMain = damagedEngineSmokeParticles.main;
					damagedMain.startSize = Mathf.Lerp(DamagedParticlesMinSize, DamagedParticlesMaxSize, damageLevel);
				}

				bool correctParticlesActive = damagedEngineSmokeParticles.gameObject.activeInHierarchy == shouldPlayDamageStartupParticles;
				if (!correctParticlesActive && damagedEngineSmokeParticles.particleCount == 0)
				{
					damagedEngineSmokeParticles.gameObject.SetActive(shouldPlayDamageStartupParticles);
				}

				if (damagedEngineSmokeParticles.isPlaying != shouldPlayDamageStartupParticles)
				{
					if (shouldPlayDamageStartupParticles)
					{
						damagedEngineSmokeParticles.Play();
					}
					else
					{
						damagedEngineSmokeParticles.Stop();
					}
				}
				yield return timeout;
			}
		}

        #endregion
    }
}