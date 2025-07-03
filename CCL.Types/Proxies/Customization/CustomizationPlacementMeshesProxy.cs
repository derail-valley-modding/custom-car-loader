using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    [AddComponentMenu("CCL/Proxies/Customization/Customization Placement Meshes Proxy")]
    public class CustomizationPlacementMeshesProxy : MonoBehaviour
    {
        [Tooltip("Create mesh colliders from these")]
        public MeshFilter[] collisionMeshes = new MeshFilter[0];
        [Tooltip("Create mesh colliders from these, but don't allow drilling")]
        public MeshFilter[] drillDisableMeshes = new MeshFilter[0];

        [Header("Optional")]
        public bool generateFromTrainInteriorCols;
    }
}
