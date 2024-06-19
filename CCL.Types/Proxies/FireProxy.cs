using UnityEngine;

namespace CCL.Types.Proxies
{
    public class FireProxy : MonoBehaviour
    {
        public GameObject fireObj;
        public GameObject sparksObj;
        public GameObject helperTriggerVR;
        [SerializeField]
        private SphereCollider ignitionCollider;
        public Renderer[] emissiveRenderersAffectedByFire;
        public Light fireLight;
        public float minFireIntensity;
        public float maxFireIntensity = 1f;
    }
}
