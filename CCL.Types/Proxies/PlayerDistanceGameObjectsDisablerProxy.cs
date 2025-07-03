using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Player Distance GameObjects Disabler Proxy")]
    public class PlayerDistanceGameObjectsDisablerProxy : MonoBehaviour
    {
        public List<GameObject> optimizingGameObjects = new List<GameObject>();
        // Vanilla only uses squared distance, so there's a special mapping for this
        // field to square it. Using non squared for ease of use.
        public float disableDistance = 500f;
        public float checkPeriodPerGO = 2f;
    }
}
