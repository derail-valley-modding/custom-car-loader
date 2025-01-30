using UnityEngine;

namespace CCL.Types.Components
{
    public enum CouplerDirection
    {
        Front,
        Rear
    }

    public class CarAutoCoupler : MonoBehaviour
    {
        public CouplerDirection Direction;

        public bool AlwaysCouple = false;
        [CarKindField]
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];
    }
}
