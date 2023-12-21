using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Effects
{
    public class PlayerDistanceMultipleGameObjectsOptimizerProxy : MonoBehaviour
    {
        public GameObject gameObjectToCheckDistance;

        public List<GameObject> gameObjectsToDisable;

        public List<MonoBehaviour> scriptsToDisable;

        public float disableDistance = 500f;

        public float checkPeriod = 2f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, disableDistance);
        }
    }
}
