using UnityEngine;

namespace CCL.Types.Proxies
{
    public class ResourceExplosionBaseProxy : MonoBehaviour
    {
        [CargoField(false)]
        public string explosionLiquid = "Diesel";
        public ExplosionPrefab explosionPrefab = ExplosionPrefab.DieselLocomotive;
        public Transform explosionAnchor = null!;
    }
}
