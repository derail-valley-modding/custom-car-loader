using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Resource Explosion Base Proxy")]
    public class ResourceExplosionBaseProxy : MonoBehaviour
    {
        [CargoField(false)]
        public string explosionLiquid = "Diesel";
        public ExplosionPrefab explosionPrefab = ExplosionPrefab.DieselLocomotive;
        public Transform explosionAnchor = null!;
    }
}
