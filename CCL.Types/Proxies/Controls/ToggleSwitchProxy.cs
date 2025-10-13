using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Toggle Switch Proxy")]
    public class ToggleSwitchProxy : ControlSpecProxy
    {
        [Header("Toggle switch")]
        public Vector3 jointAxis = Vector3.forward;
        public float jointLimitMin;
        public float jointLimitMax;

        public float autoOffTimer;

        [Header("Audio")]
        public AudioClip toggle = null!;

        [Header("VR")]
        public Vector3 touchInteractionAxis = Vector3.up;
        public bool disableTouchUse;

        [Header("Editor visualization")]
        public float gizmoRadius = 0.05f;
        public float angleOffset = 0;

        private void OnDrawGizmos()
        {
            GizmoUtil.DrawLocalRotationArc(transform, jointLimitMin, jointLimitMax, jointAxis,
                START_COLOR, END_COLOR, MID_COLOR, gizmoRadius, angleOffset);

            if (disableTouchUse) return;

            using (new GizmoUtil.MatrixScope(transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, touchInteractionAxis))))
            {
                var up = Vector3.up * gizmoRadius * 0.5f;
                var right = Vector3.right * gizmoRadius * 0.2f;

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(up, -up + right);
                Gizmos.DrawLine(up, -up - right);
            }
        }
    }
}
