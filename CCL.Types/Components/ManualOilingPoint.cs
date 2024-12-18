using UnityEngine;

namespace CCL.Types.Components
{
    public class ManualOilingPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.15f);
        }
    }
}
