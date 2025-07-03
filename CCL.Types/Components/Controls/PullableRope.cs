using CCL.Types.Proxies.Controls;
using UnityEngine;

namespace CCL.Types.Components.Controls
{
    [AddComponentMenu("CCL/Components/Controls/Pullable Rope")]
    public class PullableRope : ControlSpecProxy
    {
        [Header("Rope")]
        public Transform Origin = null!;
        [Tooltip("The length of the rope at rest")]
        public float RestLength = 0.5f;
        [Tooltip("How far the rope can be stretched")]
        public float Extension = 0.35f;
        [Tooltip("How much the rope must be pulled before being considered \"pulled\"")]
        public float Tolerance = 0.03f;

        [Header("Audio")]
        public AudioClip? Drag;
        public AudioClip? LimitHit;
        [Tooltip("Notches are audio only")]
        public AudioClip? Notch;
        public int AudioNotches;
        public bool LimitVibration;

        private void OnDrawGizmos()
        {
            if (Origin == null) return;

            var pos = Origin.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos += Vector3.down * RestLength);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + Vector3.down * Extension);
        }
    }
}
