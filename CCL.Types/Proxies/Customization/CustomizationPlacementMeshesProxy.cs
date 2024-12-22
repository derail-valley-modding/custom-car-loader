using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    public class CustomizationPlacementMeshesProxy : MonoBehaviour
    {
        [Tooltip("Create mesh colliders from these")]
        public MeshFilter[] collisionMeshes;
        [Tooltip("Create mesh colliders from these, but don't allow drilling")]
        public MeshFilter[] drillDisableMeshes;

        [Header("Optional")]
        public bool generateFromTrainInteriorCols;
    }
}
