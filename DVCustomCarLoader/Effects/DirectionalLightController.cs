using System.Collections;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts.CabControls;
using System;
using System.Collections.Generic;

namespace DVCustomCarLoader.Effects
{
    public class DirectionalLightController : MonoBehaviour, ILocoMultiValueWatcher, ILocoValueProvider
    {
        public bool MasterLightState = false;
        public float Direction = 0;

        protected LocoValueBinding[] Bindings = new[]
        {
            new LocoValueBinding(CabIndicatorType.Headlights),
            new LocoValueBinding(CabIndicatorType.Reverser)
        };

        public IEnumerable<ILocoValueWatcher> ValueBindings => Bindings;

        public float GetForwardLightIntensity()
        {
            return MasterLightState ? Mathf.InverseLerp(0, 1, Direction) : 0;
        }

        public float GetReverseLightIntensity()
        {
            return MasterLightState ? Mathf.InverseLerp(0, -1, Direction) : 0;
        }

        protected void Update()
        {
            foreach (var binding in Bindings) binding.GetLatest();

            MasterLightState = Bindings[0].LatestValue > 0.5;
            Direction = Bindings[1].LatestValue;
        }

        public bool TryBind(ILocoValueWatcher watcher)
        {
            switch (watcher.ValueBinding)
            {
                case CabIndicatorType.LightsForward:
                    watcher.Bind(GetForwardLightIntensity);
                    return true;

                case CabIndicatorType.LightsReverse:
                    watcher.Bind(GetReverseLightIntensity);
                    return true;

                default:
                    return false;
            }
        }
    }
}