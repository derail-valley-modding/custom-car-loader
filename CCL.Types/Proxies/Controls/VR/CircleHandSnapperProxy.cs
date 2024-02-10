using System;
using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    public class CircleHandSnapperProxy : MonoBehaviour
    {
        [Tooltip("This object should have its Y+ direction aligned with the wheel rotation axis, pointing outwards from the object, i.e. towards the player")]
        public Transform centerUpward = null!;
        public float radius = 0.5f;

        public void OnDrawGizmosSelected()
        {
            if (centerUpward != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 position = centerUpward.position;
                Vector3 forward = centerUpward.forward;
                Vector3 right = centerUpward.right;
                Vector3 from = position + right * radius;
                for (int i = 1; i <= 16; i++)
                {
                    Vector3 vector = position + Mathf.Cos(i / 16f * (float)Math.PI * 2f) * radius * right + Mathf.Sin(i / 16f * (float)Math.PI * 2f) * radius * forward;
                    Gizmos.DrawLine(from, vector);
                    from = vector;
                }

                Gizmos.DrawLine(position - right * radius, position + right * radius);
                Gizmos.DrawLine(position - forward * radius, position + forward * radius);
                Gizmos.DrawSphere(position, 0.01f);
            }
        }
    }
}
