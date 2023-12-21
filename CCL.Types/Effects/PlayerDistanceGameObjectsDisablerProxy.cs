using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Effects
{
    public class PlayerDistanceGameObjectsDisablerProxy : MonoBehaviour
    {
        public List<GameObject> optimizingGameObjects;

        public float disableDistance = 500f;

        public float checkPeriodPerGO = 2f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, disableDistance);
        }
    }
}
