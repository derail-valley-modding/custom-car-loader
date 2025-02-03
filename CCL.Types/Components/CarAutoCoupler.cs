using UnityEngine;

namespace CCL.Types.Components
{
    public class CarAutoCoupler : MonoBehaviour
    {
        [Tooltip("The coupler of this car to connect")]
        public CouplerDirection Direction;
        [Tooltip("The coupler of the other car to connect")]
        public CouplerDirection OtherDirection;

        public bool AlwaysCouple = false;
        [CarKindField]
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];
    }
}
