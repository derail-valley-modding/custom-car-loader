using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class VehicleDiagramExtras
    {
        [Tooltip("Dimensions are in millimetres")]
        public int Length = 7600;
        public int Width = 2960;
        public int Height = 3830;

        [Tooltip("Use a thin vehicle background instead of the standard (only handcar in the base game)")]
        public bool IsThinVehicle = false;

        [Tooltip("Mass is in tons\n" +
            "Full mass counts all resources like fuel/sand\n" +
            "If the vehicle's mass never changes (like the caboose), " +
            "use 0 in this field.")]
        public float MassFull = 39.2f;
        [Tooltip("Empty mass does not count resources")]
        public float MassEmpty = 38.0f;

        public bool HasFrontBumper = true;
        public bool HasRearBumper = true;

        [Tooltip("Total cost to repair this vehicle from 0% to 100%")]
        public int TotalCost = 88500;
    }
}
