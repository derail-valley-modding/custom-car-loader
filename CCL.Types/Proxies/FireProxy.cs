using UnityEngine;

namespace CCL.Types.Proxies
{
    public class FireProxy : MonoBehaviour, ICanReplaceInstanced
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
