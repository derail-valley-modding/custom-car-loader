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

        [Space]
        [Tooltip("These objects will be moved left when regauged")]
        public GameObject[] MoveLeft = new GameObject[0];
        [Tooltip("These objects will be moved right when regauged")]
        public GameObject[] MoveRight = new GameObject[0];

        [Space]
        [Tooltip("Will regauge for smaller gauges")]
        public bool RegaugeForSmaller = true;
        [Tooltip("Will regauge for bigger gauges")]
        public bool RegaugeForBiggerer = true;
    }
}
