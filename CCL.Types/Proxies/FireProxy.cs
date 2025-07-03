using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Fire Proxy")]
    public class FireProxy : MonoBehaviour, ICanReplaceInstanced
    {
        public GameObject fireObj = null!;
        public GameObject sparksObj = null!;
        public GameObject helperTriggerVR = null!;
        [SerializeField]
        private SphereCollider ignitionCollider = null!;
        public Renderer[] emissiveRenderersAffectedByFire = new Renderer[0];
        public Light fireLight = null!;
        public float minFireIntensity;
        public float maxFireIntensity = 1f;

        public void DoFieldReplacing()
        {
            if (fireObj.TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
            {
                fireObj = go.InstancedObject!;
            }

            if (sparksObj.TryGetComponent(out go) && go.CanReplace)
            {
                sparksObj = go.InstancedObject!;
            }
        }
    }
}
