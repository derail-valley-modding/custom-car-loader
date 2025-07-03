using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Duplicate Handbrake Overrider")]
    [DisallowMultipleComponent]
    public class DuplicateHandbrakeOverrider : MonoBehaviour
    {
        public CouplerDirection Direction;

        public bool AlwaysCopy = false;
        [CarKindField]
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];
    }
}
