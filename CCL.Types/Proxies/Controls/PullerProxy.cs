using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Puller Proxy")]
    public class PullerProxy : ControlSpecProxy, ISelfValidation
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
        [EnableIf(nameof(useCustomConnectionAnchor))]
        public Transform connectionAnchor = null!;
        public Transform pivot = null!;
        public float linearLimit = 0.003f;

        [Header("Audio")]
        public AudioClip notch = null!;
        public AudioClip drag = null!;
        public AudioClip limitHit = null!;

        private void OnDrawGizmos()
        {
            if (useCustomConnectionAnchor && connectionAnchor == null) return;

            Vector3 movedOffset = Vector3.up * linearLimit;

            using (GizmoUtil.MatrixScope.LocalTransform(useCustomConnectionAnchor ? connectionAnchor : transform))
            {
                GizmoUtil.DrawGradientLine(movedOffset, -movedOffset, START_COLOR, END_COLOR);
                Gizmos.color = START_COLOR;
                Gizmos.DrawWireSphere(movedOffset, 0.01f);
                Gizmos.color = END_COLOR;
                Gizmos.DrawWireSphere(-movedOffset, 0.01f);
            }
        }

        public SelfValidationResult Validate(out string message)
        {
            if (transform.localPosition != Vector3.zero)
            {
                message = "local position should be (0, 0, 0)";
                return SelfValidationResult.Warning;
            }

            if (useCustomConnectionAnchor && connectionAnchor != null)
            {
                var dot = Vector3.Dot(transform.up, connectionAnchor.up);

                if (dot < 0.95f || dot > 1.05f)
                {
                    message = "puller and anchor are not aligned";
                    return SelfValidationResult.Warning;
                }
            }

            if (transform.localRotation != Quaternion.identity)
            {
                message = "local rotation should be (0, 0, 0)";
                return SelfValidationResult.Warning;
            }

            return this.Pass(out message);
        }
    }
}
