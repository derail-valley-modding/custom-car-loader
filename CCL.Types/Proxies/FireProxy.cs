using UnityEngine;

namespace CCL.Types.Proxies
{
    public class FireProxy : MonoBehaviour, ICanReplaceGO
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

        public void CheckGOFields()
        {
            if (fireObj.TryGetComponent(out IInstancedGO go) && go.CanReplace)
            {
                fireObj = go.InstancedGO!;
            }

            if (sparksObj.TryGetComponent(out go) && go.CanReplace)
            {
                sparksObj = go.InstancedGO!;
            }
        }
    }
}
