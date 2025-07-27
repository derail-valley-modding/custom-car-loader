using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Ladder")]
    [RequireComponent(typeof(Collider))]
    public class Ladder : MonoBehaviour
    {
        private void Start()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
            tag = CarPartNames.Tags.LADDERS;

            Destroy(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            var pos = transform.position;
            var up = transform.up * 0.5f;
            var right = transform.right * 0.25f;

            Gizmos.DrawLine(pos + right - up, pos + right + up);
            Gizmos.DrawLine(pos - right - up, pos - right + up);

            up *= 0.8f;

            for (int i = 0; i < 4; i++)
            {
                var vert = Vector3.LerpUnclamped(-up, up, i / 3.0f);
                Gizmos.DrawLine(pos - right + vert, pos + right + vert);
            }

            Gizmos.color = Color.green;

            up *= 0.8f;
            right *= 1.5f;
            pos += transform.forward * -0.25f;

            Gizmos.DrawLine(pos - right - up, pos);
            Gizmos.DrawLine(pos + right - up, pos);
            Gizmos.DrawLine(pos - right, pos + up);
            Gizmos.DrawLine(pos + right, pos + up);
        }
    }
}
