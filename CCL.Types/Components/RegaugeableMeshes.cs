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
        [Tooltip("Objects on the left side to be moved when regauged")]
        public Transform[] MoveLeft = new Transform[0];
        [Tooltip("Objects on the right side to be moved when regauged")]
        public Transform[] MoveRight = new Transform[0];

        [Space]
        [Tooltip("Will regauge for smaller gauges")]
        public bool RegaugeForSmaller = true;
        [Tooltip("Will regauge for bigger gauges")]
        public bool RegaugeForBigger = true;
    }
}
