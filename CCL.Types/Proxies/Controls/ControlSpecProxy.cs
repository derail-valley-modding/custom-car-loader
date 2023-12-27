using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class ControlSpecProxy : MonoBehaviour
    {
        [Header("Common")]
        public bool disallowShortTriggerLockHold;
        public GameObject[] colliderGameObjects;
        public InteractionHandPosesProxy handPosesOverride;
    }
}
