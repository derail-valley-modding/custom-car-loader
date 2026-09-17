using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Puller Proxy")]
    public class PullerProxy : ControlSpecProxy, ISelfValidation
    {
        [Header("Rigidbody")]
        public float rigidbodyMass = 5f;
        public float rigidbodyDrag = 15f;
        public bool zeroCenterOfMass;

        [Header("Puller")]
        public BoxCollider insideVolume = null!;

        [Header("Stepped puller")]
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
            var start = invertDirection ? END_COLOR : START_COLOR;
            var end = invertDirection ? START_COLOR : END_COLOR;

            using (GizmoUtil.MatrixScope.LocalTransform(useCustomConnectionAnchor ? connectionAnchor : transform))
            {
                GizmoUtil.DrawGradientLine(movedOffset, -movedOffset, start, end);
                Gizmos.color = start;
                Gizmos.DrawWireSphere(movedOffset, 0.01f);
                Gizmos.color = end;
                Gizmos.DrawWireSphere(-movedOffset, 0.01f);
            }
        }

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            highlight = null;

            if (transform.localPosition != Vector3.zero)
            {
                message = "local position should be (0, 0, 0)";
                return SelfValidationResult.Warning;
            }

            if (useCustomConnectionAnchor && connectionAnchor != null)
            {
                var dot = Vector3.Dot(transform.up, connectionAnchor.up);

                if (invertDirection)
                {
                    dot = -dot;
                }

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

            if (insideVolume != null && insideVolume.gameObject.activeSelf)
            {
                message = "inside volume should be disabled";
                return SelfValidationResult.Warning;
            }

            return this.Pass(out message, out highlight);
        }
    }
}
