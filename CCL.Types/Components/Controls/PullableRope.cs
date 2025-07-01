using CCL.Types.Proxies.Controls;
using UnityEngine;

namespace CCL.Types.Components.Controls
{
    public class PullableRope : ControlSpecProxy
    {
        [Header("Rope")]
        public Transform Origin = null!;
        public float RestLength = 0.5f;
        public float Extension = 0.35f;

        //[Header("Audio")]
        //public AudioClip? Drag;
        //public AudioClip? LimitHit;
        //public bool LimitVibration;

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
