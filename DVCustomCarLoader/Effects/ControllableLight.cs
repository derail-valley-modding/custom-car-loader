using System.Collections;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts.CabControls;
using System;

namespace DVCustomCarLoader.Effects
{
    public class ControllableLight : Indicator
    {
        public Light[] Lights;
        public float MinLevel = 0;
        public float MaxLevel = 1;
        public float Lag = 0.05f;

        protected float SmoothedLevel = 0;
        protected float SmoothingVelo = 0;
        protected float TargetLevel = 0;

        public void ApplyNormalizedLevel(float newLevel)
        {
            float mapped = Mathf.Lerp(MinLevel, MaxLevel, newLevel);

            foreach (Light l in Lights)
            {
                l.intensity = mapped;
                SmoothedLevel = mapped;
            }
        }

        protected override void Start()
        {
            base.Start();
            ApplyNormalizedLevel(MinLevel);
        }

        protected override void OnValueSet()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TargetLevel = GetNormalizedValue(true);

            if (Lag == 0)
            {
                ApplyNormalizedLevel(TargetLevel);
            }
        }

        protected void Update()
        {
            if (Lag > 0)
            {
                float smoothedLevel = Mathf.SmoothDamp(SmoothedLevel, TargetLevel, ref SmoothingVelo, Lag);
                ApplyNormalizedLevel(smoothedLevel);
            }
        }
    }
}