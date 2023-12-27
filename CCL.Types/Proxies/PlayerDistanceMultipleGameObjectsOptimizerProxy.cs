using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class PlayerDistanceMultipleGameObjectsOptimizerProxy : MonoBehaviour
    {
        public GameObject gameObjectToCheckDistance = null!;

        public List<GameObject> gameObjectsToDisable = new List<GameObject>();

        public List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();

        public float disableDistance = 500f;

        public float checkPeriod = 2f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, disableDistance);
        }
    }
}
