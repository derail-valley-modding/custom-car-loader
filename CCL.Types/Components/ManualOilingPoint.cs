using UnityEngine;

namespace CCL.Types.Components
{
    public class ManualOilingPoint : MonoBehaviour
    {
        private static Vector3 s_pos = Vector3.up * 0.05f;
        private static Vector3 s_size = Vector3.up * 0.1f;

        public string SyncTag = "o0";

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(s_pos, s_size);
        }
    }
}
