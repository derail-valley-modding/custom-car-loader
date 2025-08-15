using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Fire Proxy")]
    public class FireProxy : MonoBehaviourWithVehicleDefaults, ICanReplaceInstanced, IS060Defaults, IS282Defaults
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
        public Light fillLight = null!;
        public float fillLightMultiplier = 0.5f;
        public Light bounceLight = null!;
        public float bounceLightMultiplier = 0.25f;
        public AnimationCurve fireboxDoorCurve = AnimationCurve.Linear(0f, 0.05f, 1f, 1f);

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

        public void ApplyS060Defaults()
        {
            fireboxDoorCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0.0f, 0.0f, 2f, 2f, 0, 0),
                    new Keyframe(1.0f, 1.0f, 0f, 0f, 0, 0)
                }
            };
        }

        public void ApplyS282Defaults()
        {
            fireboxDoorCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0.0f, 0.05f, 2f, 2f, 0, 0),
                    new Keyframe(1.0f, 1.0f, 0f, 0f, 0, 0)
                },
            };
        }
    }
}
