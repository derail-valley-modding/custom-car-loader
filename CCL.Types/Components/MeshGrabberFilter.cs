using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Mesh Grabber (Filter)")]
    public class MeshGrabberFilter : MonoBehaviour, ICustomGrabberValidation
    {
        public MeshFilter Filter = null!;
        public string ReplacementName = string.Empty;

        private void Reset()
        {
            PickSame();
        }

        public void PickSame()
        {
            Filter = GetComponent<MeshFilter>();
        }

        public bool IsValid(out string error)
        {
            if (Filter == null)
            {
                error = $"MeshGrabberFilter in {gameObject.GetPath()} does not have a mesh filter.";
                return false;
            }

            if (!MeshGrabber.MeshNames.Contains(ReplacementName))
            {
                error = $"MeshGrabberFilter in {gameObject.GetPath()} does not have a valid replacement ({ReplacementName}).";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
