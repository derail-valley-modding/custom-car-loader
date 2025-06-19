using UnityEngine;

namespace CCL.Types.Components
{
    public class VehicleAOShadow : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.grey * 0.2f;
            Gizmos.DrawCube(Vector3.zero, new Vector3(1, 0, 1));
            Gizmos.color = Color.grey * 0.8f;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1, 0, 1));
        }
    }
}
