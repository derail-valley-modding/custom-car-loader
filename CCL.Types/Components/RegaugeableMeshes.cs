using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Regaugeable Meshes")]
    public class RegaugeableMeshes : MonoBehaviour
    {
        [Tooltip("All child meshes of these will be regauged if necessary")]
        public GameObject[] Objects = new GameObject[0];
        [Tooltip("Only these specific meshes will be regauged if necessary")]
        public MeshFilter[] Meshes = new MeshFilter[0];
    }
}
