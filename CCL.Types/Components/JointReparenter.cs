using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Joint Reparenter")]
    [RequireComponent(typeof(Joint))]
    public class JointReparenter : MonoBehaviour
    {
        private Joint _joint = null!;

        private void Start()
        {
            _joint = GetComponent<Joint>();
            _joint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();
        }
    }
}
