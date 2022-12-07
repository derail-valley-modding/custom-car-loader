using CCL_GameScripts.Effects;
using DVCustomCarLoader.LocoComponents.Steam;
using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.Effects
{
    public class SteamParticlesController : MonoBehaviour, IInitSpecFinalizer<SteamEmissionSetup>
    {
        protected SteamEmissionSetup spec;

        public ParticleSystem chimneyParticles;
        public ParticleSystem chuffParticlesLeft;
        public ParticleSystem chuffParticlesRight;

        public ParticleSystem whistleMid;
        public ParticleSystem whistleFront;

        public ParticleSystem releaseLeft;
        public ParticleSystem releaseRight;

        public ParticleSystem safetyRelease;

        protected CustomLocoControllerSteam controller;
        protected CustomChuffController chuffController;

        public void FinalizeFromSpec(SteamEmissionSetup spec)
        {
            this.spec = spec;
            var particles = ParticleInitializer.AddSteamParticles(this, spec);

            chimneyParticles = particles.ChimneyParticles;
            chuffParticlesLeft = particles.ChuffParticlesLeft;
            chuffParticlesRight = particles.ChuffParticlesRight;

            whistleMid = particles.WhistleMid;
            whistleFront = particles.WhistleFront;

            releaseLeft = particles.ReleaseLeft;
            releaseRight = particles.ReleaseRight;
            safetyRelease = particles.SafetyRelease;

            if (chimneyParticles)
            {
                chuffOriginalSpeedMultiplier = chimneyParticles.main.startSpeedMultiplier;
                chuffOriginalSizeMultiplier = chimneyParticles.main.startSizeMultiplier;
                chuffOriginalEmissionRate = chimneyParticles.emission.rateOverTime.constant;
            }

            particles.Root.SetActive(true);
        }

        protected void OnEnable()
        {
            var car = TrainCar.Resolve(gameObject);
            controller = car.GetComponent<CustomLocoControllerSteam>();

            if (!controller)
            {
                Main.Error("SteamParticleController: No custom steam controller! Disabling...");
                Destroy(this);
                return;
            }

            var main = chimneyParticles.main;
            main.startSpeedMultiplier *= 2;
            main.startSizeMultiplier *= 1.5f;
            
            var emission = chimneyParticles.emission;
            //emission.rateOverTime = emission.rateOverTime.constant * 2;

            chuffOriginalSpeedMultiplier = chimneyParticles.main.startSpeedMultiplier;
            chuffOriginalSizeMultiplier = chimneyParticles.main.startSizeMultiplier;
            chuffOriginalEmissionRate = chimneyParticles.emission.rateOverTime.constant;

            smoothedColor = smokeColorClear;

            chuffController = car.GetComponent<CustomChuffController>();
            if (!chuffController)
            {
                chuffController = car.gameObject.AddComponent<CustomChuffController>();
            }
            chuffController.OnChuff += Chuff;

            SteamCoro = StartCoroutine(UpdateSteamParticles(STEAM_UPDATE_PERIOD));
            if (chimneyParticles)
            {
                SmokeCoro = StartCoroutine(UpdateSmokeParticles(SMOKE_UPDATE_PERIOD));
            }
            Main.LogVerbose("SteamParticlesController enabled");
        }

        protected void OnDisable()
        {
            if (chuffController)
            {
                chuffController.OnChuff -= Chuff;
            }
            if (SmokeCoro != null) StopCoroutine(SmokeCoro);
            StopCoroutine(SteamCoro);
        }

        #region Chuff Particles

        protected Coroutine SmokeCoro = null;

        private const float SMOKE_UPDATE_PERIOD = 0.2f;

        private const float LIFETIME_MIN = 1f;
        private const float LIFETIME_MAX = 4f;

        private const float MAX_SMOKE_BURN_PERCENT = 0.15f;

        private const float EMISSION_RATE_MIN = 10f;
        private const float EMISSION_RATE_MAX = 100f;

        private float chuffOriginalSpeedMultiplier;
        private float chuffOriginalSizeMultiplier;
        private float chuffOriginalEmissionRate;

        public float chuffSizeMult = 1.5f;
        public float chuffSpeedMult = 1.5f;
        public float chuffEmissionRate = 1000f;

        public Color steamColor = Color.white;
        public Color smokeColorClear = new Color(0.14f, 0.14f, 0.13f, 0.1f);
        public Color smokeColorDark = new Color(0.14f, 0.14f, 0.13f);

        private Color smoothedColor;
        private const float SMOOTHING_RATE = 0.7f;

        protected IEnumerator UpdateSmokeParticles(float period)
        {
            WaitForSeconds wait = WaitFor.Seconds(period);
            while (true)
            {
                yield return wait;

                var main = chimneyParticles.main;
                float burnRatePercent = controller.BurnRatePercent;
                //Main.LogVerbose($"Chimney burn rate: {burnRatePercent} ({controller.FuelConsumptionRate}/{controller.MaxFuelConsumptionRate})");

                if ((burnRatePercent == 0) && chimneyParticles.isPlaying)
                {
                    chimneyParticles.Stop();
                }
                else if ((burnRatePercent > 0) && !chimneyParticles.isPlaying)
                {
                    chimneyParticles.Play();
                }

                if (chimneyParticles.isPlaying)
                {
                    var targetColor = Color.Lerp(smokeColorClear, smokeColorDark, burnRatePercent);
                    smoothedColor = Color.Lerp(smoothedColor, targetColor, SMOOTHING_RATE);
                    main.startColor = smoothedColor;

                    float t = Mathf.InverseLerp(0, MAX_SMOKE_BURN_PERCENT, burnRatePercent);
                    main.startLifetime = Mathf.Lerp(LIFETIME_MIN, LIFETIME_MAX, t);

                    var emission = chimneyParticles.emission;
                    emission.rateOverTime = Mathf.Lerp(EMISSION_RATE_MIN, EMISSION_RATE_MAX, t);
                }
            }
        }

        protected void Chuff(float power)
        {
            if (power < 0.1f || controller.BoilerPressure == 0f)
            {
                return;
            }

            if (!chimneyParticles.isPlaying)
            {
                chimneyParticles.Play();
            }

            float steamLvl = power * 2;

            smoothedColor = Color.Lerp(smoothedColor, steamColor, steamLvl);

            ParticleSystem.MainModule main = chimneyParticles.main;
            main.startSpeedMultiplier = chuffOriginalSpeedMultiplier * Mathf.Lerp(1, chuffSpeedMult, steamLvl);
            main.startSizeMultiplier = chuffOriginalSizeMultiplier * Mathf.Lerp(1, chuffSizeMult, steamLvl);
            main.startColor = smoothedColor;

            var emission = chimneyParticles.emission;
            emission.rateOverTime = Mathf.Lerp(chuffOriginalEmissionRate, chuffEmissionRate, steamLvl);

            StartCoroutine(EndChuffCoro());

            if (spec.ChuffParticlesLeftLocation) chuffParticlesLeft.Play();
            if (spec.ChuffParticlesRightLocation) chuffParticlesRight.Play();
        }

        private IEnumerator EndChuffCoro()
        {
            yield return null;

            var main = chimneyParticles.main;
            main.startSpeedMultiplier = chuffOriginalSpeedMultiplier;
            main.startSizeMultiplier = chuffOriginalSizeMultiplier;

            var emission = chimneyParticles.emission;
            emission.rateOverTime = chuffOriginalEmissionRate;

            if ((controller.BurnRatePercent == 0) && chimneyParticles.isPlaying)
            {
                chimneyParticles.Stop();
            }

            yield break;
        }

        #endregion

        #region Steam Particles

        protected Coroutine SteamCoro = null;
        private const float STEAM_UPDATE_PERIOD = 0.2f;

        private const float WHISTLE_START_SPEED_MIN = 0.5f;
        private const float WHISTLE_START_SPEED_MAX = 1f;

        private const float STEAM_STARTING_SPEED_MIN = 1f;
        private const float STEAM_STARTING_SPEED_MAX = 2f;

        private const float LOW_STEAM_PERCENT_LIMIT = 0.05f;

        protected IEnumerator UpdateSteamParticles(float period)
        {
            WaitForSecondsRealtime wait = WaitFor.SecondsRealtime(period);
            while (true)
            {
                yield return wait;

                UpdateWhistle();
                UpdateSteamRelease();
            }
        }

        private void UpdateWhistle()
        {
            float whistle = controller.GetWhistle();
            if ((controller.BoilerPressure > 0) && (whistle > 0))
            {
                float speed = Mathf.Lerp(WHISTLE_START_SPEED_MIN, WHISTLE_START_SPEED_MAX, whistle);

                if (whistleMid && spec.WhistleMidLocation)
                {
                    var whistleMainM = whistleMid.main;
                    whistleMainM.startSpeed = speed;

                    if (!whistleMid.isPlaying)
                    {
                        whistleMid.Play();
                    }
                }

                if (whistleFront && spec.WhistleFrontLocation)
                {
                    var whistleMainF = whistleFront.main;
                    whistleMainF.startSpeed = speed;

                    if (!whistleFront.isPlaying)
                    {
                        whistleFront.Play();
                    }
                }
            }
            else
            {
                if (whistleMid && whistleMid.isPlaying)
                {
                    whistleMid.Stop();
                }

                if (whistleFront && whistleFront.isPlaying)
                {
                    whistleFront.Stop();
                }
            }
        }

        private void UpdateSteamRelease()
        {
            // Steam Dump
            if ((controller.GetSteamDump() == 0) || (controller.BoilerPressure == 0))
            {
                if (releaseLeft && releaseLeft.isPlaying)
                {
                    releaseLeft.Stop();
                }

                if (releaseRight && releaseRight.isPlaying)
                {
                    releaseRight.Stop();
                }
            }
            else
            {
                float releaseLimit = controller.MaxBoilerPressure * LOW_STEAM_PERCENT_LIMIT;
                float releaseSpeed = controller.GetSteamDump() * Mathf.Clamp01(controller.BoilerPressure / releaseLimit) + 1;

                if (releaseLeft && spec.ReleaseLeftLocation)
                {
                    var leftMain = releaseLeft.main;
                    leftMain.startSpeed = releaseSpeed;

                    if (!releaseLeft.isPlaying)
                    {
                        releaseLeft.Play();
                    }
                }

                if (releaseRight && spec.ReleaseRightLocation)
                {
                    var rightMain = releaseRight.main;
                    rightMain.startSpeed = releaseSpeed;

                    if (!releaseRight.isPlaying)
                    {
                        releaseRight.Play();
                    }
                }
            }

            // Safety Release
            if (safetyRelease)
            {
                if ((controller.SafetyValve == 0) && safetyRelease.isPlaying)
                {
                    safetyRelease.Stop();
                }
                else if ((controller.SafetyValve > 0) && !safetyRelease.isPlaying && spec.SafetyReleaseLocation)
                {
                    safetyRelease.Play();
                }
            }
        }

        #endregion
    }
}