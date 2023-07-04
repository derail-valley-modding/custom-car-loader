using DV.ThingTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types
{
    [Serializable]
    public class LoadableCargo
    {
        public List<LoadableCargoEntry> Entries;

        public bool IsEmpty => Entries == null || Entries.Count == 0;
    }

    [Serializable]
    public class LoadableCargoEntry
    {
        public float AmountPerCar = 1f;
        public CargoType CargoType;
        public GameObject[]? ModelVariants;
    }
}
