using UnityEngine;
using static CCL.Types.GizmoUtil;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Bed Sleeping Proxy")]
    public class BedSleepingProxy : MonoBehaviour, ISelfValidation
    {
        public float fadeTime = 1.3f;
        public float waitBeforeUnfade = 1.5f;
        public Transform pillowTarget = null!;

        public SelfValidationResult Validate(out string message)
        {
            if (pillowTarget == null)
            {
                return this.FailForNull(nameof(pillowTarget), out message);
            }

            return this.Pass(out message);
        }

        private void OnDrawGizmos()
        {
            if (pillowTarget == null) return;

            using (new MatrixScope(pillowTarget.localToWorldMatrix))
            {
                Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0.3f, 0));
                Gizmos.DrawWireCube(new Vector3(0.4f, 0, 0), new Vector3(0.6f, 0.1f, 0.2f));
                Gizmos.DrawWireCube(new Vector3(0.3f, 0, 0.2f), new Vector3(0.4f, 0.1f, 0.1f));
                Gizmos.DrawWireCube(new Vector3(0.3f, 0, -0.2f), new Vector3(0.4f, 0.1f, 0.1f));
                Gizmos.DrawWireSphere(Vector3.zero, 0.11f);
            }
        }
    }
}
