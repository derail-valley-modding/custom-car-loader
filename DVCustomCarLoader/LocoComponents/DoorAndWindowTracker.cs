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
        protected readonly List<(ControlImplBase Control, int CabId)> Apertures = new List<(ControlImplBase, int)>();

        private bool initialized = false;

        protected void Start()
        {
            var openings = gameObject.GetComponentsInChildrenByInterface<IApertureTrackable>()
                .Where(a => a.TrackAsAperture)
                .Select(a => (Control: a.gameObject.GetComponent<ControlImplBase>(), CabId: a.CabNumber))
                .Where(c => c.Control);

            Apertures.AddRange(openings);
            Main.LogVerbose($"Found {Apertures.Count} apertures to track");

            initialized = true;
        }

        public bool IsAnythingOpen(int cabId)
        {
            if (!initialized) return false;

            return Apertures.Any(c => (c.CabId == cabId) && (c.Control.Value >= 0.1f));
        }
    }
}