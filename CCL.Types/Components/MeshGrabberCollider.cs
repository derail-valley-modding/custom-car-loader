using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Mesh Grabber (Collider)")]
    public class MeshGrabberCollider : MonoBehaviour
    {
        public MeshCollider Collider = null!;
        public string ReplacementName = string.Empty;

        private void Reset()
        {
            PickSame();
        }

        public void PickSame()
        {
            Collider = GetComponent<MeshCollider>();
        }
    }
}
