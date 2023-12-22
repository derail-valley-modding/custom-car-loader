using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class ControlSpecProxy : MonoBehaviour
    {
        [Header("Common")]
        public bool disallowShortTriggerLockHold;

        // Token: 0x04004762 RID: 18274
        public GameObject[] colliderGameObjects;

        // Token: 0x04004763 RID: 18275
        public InteractionHandPosesProxy handPosesOverride;
    }
}