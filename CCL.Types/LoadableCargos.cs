using DV.ThingTypes;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Cargo Assignment")]
    public class LoadableCargo : ScriptableObject
    {
        public float AmountPerCar = 1f;
        public CargoType CargoType;
        public GameObject[]? ModelVariants;
    }
}
