using System.Collections;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts.CabControls;
using System;

namespace DVCustomCarLoader.Effects
{
    public class ControllableLight : MonoBehaviour, ILocoEventAcceptor
    {
        public Light[] Lights;
        public float MinLevel = 0;
        public float MaxLevel = 1;
        public float Lag = 0.05f;

        protected float SmoothedLevel = 0;
        protected float SmoothingVelo = 0;
        protected float TargetLevel = 0;

        public SimEventType OutputBinding;
        public SimEventType[] EventTypes => new[] { OutputBinding };
        public float MinValue = 0;
        public float MaxValue = 1;

        private float MapInputValue(float input)
        {
            return (input - MinValue) / (MaxValue - MinValue);
        }

        public void ApplyNormalizedLevel(float newLevel)
        {
            float mapped = Mathf.LerpUnclamped(MinLevel, MaxLevel, newLevel);

            foreach (Light l in Lights)
            {
                l.intensity = mapped;
            }
        }

        protected void Start()
        {
            TargetLevel = 0;
            ApplyNormalizedLevel(0);
        }

        protected void Update()
        {
            if (Lag > 0)
            {
                SmoothedLevel = Mathf.SmoothDamp(SmoothedLevel, TargetLevel, ref SmoothingVelo, Lag);
                ApplyNormalizedLevel(SmoothedLevel);
            }
        }

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float value)
            {
                TargetLevel = MapInputValue(value);
            }
            else if (eventInfo.NewValue is bool onOff)
            {
                TargetLevel = onOff ? 1 : 0;
            }
            else return;

            if (Lag == 0)
            {
                ApplyNormalizedLevel(TargetLevel);
            }
        }
    }
}