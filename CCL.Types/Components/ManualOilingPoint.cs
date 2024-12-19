using UnityEngine;

namespace CCL.Types.Components
{
    public class ManualOilingPoint : MonoBehaviour
    {
        public string SyncTag = "o0";

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.15f);
        }
    }
}
