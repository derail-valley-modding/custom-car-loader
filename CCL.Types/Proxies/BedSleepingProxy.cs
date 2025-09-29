using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Bed Sleeping Proxy")]
    public class BedSleepingProxy : MonoBehaviour
    {
        public float fadeTime = 1.3f;
        public float waitBeforeUnfade = 1.5f;
        public Transform pillowTarget = null!;

        private void OnDrawGizmosSelected()
        {
            if (pillowTarget == null) return;

            var pos = pillowTarget.position;
            Gizmos.DrawSphere(pos, 0.11f);
            Gizmos.DrawLine(pos, pos + pillowTarget.up);
            Gizmos.DrawSphere(pos + pillowTarget.right * 0.15f, 0.05f);
            Gizmos.DrawSphere(pos + pillowTarget.right * 0.30f, 0.05f);
            Gizmos.DrawSphere(pos + pillowTarget.right * 0.45f, 0.05f);
            Gizmos.DrawSphere(pos + pillowTarget.right * 0.60f, 0.05f);
            Gizmos.DrawSphere(pos, 0.11f);
        }
    }
}
