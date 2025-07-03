using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Puller Proxy")]
    public class PullerProxy : ControlSpecProxy
    {
        [Header("Rigidbody")]
        public float rigidbodyMass = 5f;
        public float rigidbodyDrag = 15f;

        [Header("Stepped puller")]
        [Header("Puller")]
        public bool useSteppedPuller;

        public int notches = 20;
        public bool invertDirection;
        public float scrollWheelHoverScroll = 0.025f;

        [Header("Configurable Joint")]
        public bool useCustomConnectionAnchor;
        public Transform connectionAnchor = null!;
        public Transform pivot = null!;
        public float linearLimit = 0.003f;

        [Header("Audio")]
        public AudioClip notch = null!;
        public AudioClip drag = null!;
        public AudioClip limitHit = null!;

        private void OnDrawGizmos()
        {
            Vector3 movedOffset = transform.TransformPoint(Vector3.up * linearLimit);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, movedOffset);
        }
    }
}
