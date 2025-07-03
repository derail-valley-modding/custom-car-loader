using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Mesh Grabber (Filter)")]
    public class MeshGrabberFilter : MonoBehaviour
    {
        public MeshFilter Filter = null!;
        public string ReplacementName = string.Empty;

        public void PickSame()
        {
            Filter = GetComponent<MeshFilter>();
        }
    }
}
