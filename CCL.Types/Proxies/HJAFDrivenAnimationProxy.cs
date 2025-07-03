using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/HJAF Driven Animation Proxy")]
    public class HJAFDrivenAnimationProxy : MonoBehaviour
    {
        [Tooltip("Optional")]
        public Animator animator = null!;
        [Tooltip("A name of the parameter to control, found in \"Animator\" window")]
        public string floatParameterName = "Main";
        [Tooltip("GameObject where HingeJointAngleFix will appear after Start")]
        public GameObject hjafObject = null!;
    }
}
