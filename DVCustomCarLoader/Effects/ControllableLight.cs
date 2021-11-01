using System.Collections;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts.CabControls;
using System;

namespace DVCustomCarLoader.Effects
{
    public class ControllableLight : Indicator, ILocoValueWatcher
    {
        public bool IsBound { get; set; }
        public bool IsBoundToInterior { get; set; }

        [field:SerializeField]
        public CabIndicatorType ValueBinding { get; set; }

        public Func<float> ValueFunction { get; set; }

        public Light[] Lights;
        public float MinLevel = 0;
        public float MaxLevel = 1;
        public float Lag = 0.05f;

        protected float SmoothedLevel = 0;
        protected float SmoothingVelo = 0;

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

        protected void Update()
        {
            if (Lights.Length == 0)
            {
                enabled = false;
            }

            if (ValueFunction == null) return;
            value = ValueFunction();
        }

        protected override void OnValueSet()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            float newLevel = GetNormalizedValue(true);

            if (Lag == 0)
            {
                ApplyNormalizedLevel(newLevel);
            }
            else
            {
                float smoothedLevel = Mathf.SmoothDamp(SmoothedLevel, newLevel, ref SmoothingVelo, Lag);
                ApplyNormalizedLevel(smoothedLevel);
            }
        }
    }
}