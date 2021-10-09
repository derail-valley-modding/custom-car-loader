using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DV.CabControls;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class DoorAndWindowTracker : MonoBehaviour
    {
        protected readonly List<ControlImplBase> Apertures = new List<ControlImplBase>();

        private bool initialized = false;

        protected void Start()
        {
            var doorObjects = GetComponentsInChildren<LeverSetup>().Where(l => l.TrackAsDoor).Select(l => l.gameObject);
            Apertures.AddRange(doorObjects.Select(obj => obj.GetComponent<LeverBase>()).Where(l => l));

            var windowObjects = GetComponentsInChildren<PullerSetup>().Where(p => p.TrackAsWindow).Select(p => p.gameObject);
            Apertures.AddRange(windowObjects.Select(obj => obj.GetComponent<PullerBase>()).Where(p => p));

            initialized = true;
        }

        public bool IsAnythingOpen()
        {
            return initialized && Apertures.Any(c => c.Value >= 0.1f);
        }
    }
}