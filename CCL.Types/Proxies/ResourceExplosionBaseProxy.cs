using UnityEngine;

namespace CCL.Types.Proxies
{
    public class ResourceExplosionBaseProxy : MonoBehaviour
    {
        [SerializeField]
        private BaseCargoType explosionLiquid = BaseCargoType.Diesel;
        [SerializeField]
        private GameObject explosionPrefab;
        [SerializeField]
        private Transform explosionAnchor;
    }
}
