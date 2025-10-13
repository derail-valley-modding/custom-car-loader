using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Mesh Grabber (Collider)")]
    public class MeshGrabberCollider : MonoBehaviour, ICustomGrabberValidation
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

        public bool IsValid(out string error)
        {
            if (Collider == null)
            {
                error = $"MeshGrabberCollider in {gameObject.GetPath()} does not have a mesh collider.";
                return false;
            }

            if (!MeshGrabber.MeshNames.Contains(ReplacementName))
            {
                error = $"MeshGrabberCollider in {gameObject.GetPath()} does not have a valid replacement ({ReplacementName}).";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
