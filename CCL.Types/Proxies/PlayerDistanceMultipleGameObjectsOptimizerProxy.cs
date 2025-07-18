using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Optimizers/Player Distance Multiple GameObjects Optimizer Proxy")]
    public class PlayerDistanceMultipleGameObjectsOptimizerProxy : MonoBehaviour
    {
        public GameObject gameObjectToCheckDistance = null!;
        public List<GameObject> gameObjectsToDisable = new List<GameObject>();
        public List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();
        // Vanilla only uses squared distance, so there's a special mapping for this
        // field to square it. Using non squared for ease of use.
        public float disableDistance = 500f;
        public float checkPeriod = 2f;
    }
}
