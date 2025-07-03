using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Loco Fuse Box Reference Proxy")]
    public class LocoFuseBoxReferenceProxy : MonoBehaviour
    {
        public GameObject starterFuse = null!;
        public GameObject electricsFuse = null!;
        public GameObject tractionMotorFuse = null!;
        public GameObject starterControl = null!;
    }
}
