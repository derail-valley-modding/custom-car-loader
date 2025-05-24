using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class VehicleDiagramExtras
    {
        [Header("Dimensions")]
        [Tooltip("Dimensions are in millimetres")]
        public int Length = 7600;
        public int Width = 2960;
        public int Height = 3830;

        [Header("Drawing")]
        [Tooltip("Use a thin vehicle background instead of the standard (only handcar in the base game)")]
        public bool IsThinVehicle = false;

        public bool HasFrontBumper = true;
        public bool HasRearBumper = true;
    }
}
