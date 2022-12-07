using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class SwivelExpandLink : MonoBehaviour
    {
        public Transform Target;
        public Transform Slider;
        private bool slideEnabled = false;

        private Vector3 initialSlidePosition;
        private float slideTargetDistance;
        private float initialSlideDistance;

        private Vector3 initialLocalTarget;

        private void Start()
        {
            if (!Target)
            {
                enabled = false;
                return;
            }

            initialLocalTarget = transform.InverseTransformPoint(Target.position).normalized;

            if (Slider)
            {
                slideTargetDistance = (Target.position - Slider.position).magnitude;
                initialSlidePosition = Slider.localPosition;
                initialSlideDistance = (Slider.position - transform.position).magnitude;
                slideEnabled = true;
            }
        }
                
        private void LateUpdate()
        {
            var currentTargetVector = Target.position - transform.position;
            var currentLocalTarget = transform.InverseTransformPoint(Target.position).normalized;

            var deltaRotation = Quaternion.FromToRotation(initialLocalTarget, currentLocalTarget);
            transform.localRotation = transform.localRotation * deltaRotation;

            if (slideEnabled)
            {
                float slideLerpAmount = (currentTargetVector.magnitude - slideTargetDistance) / initialSlideDistance;
                Slider.localPosition = initialSlidePosition * slideLerpAmount;
            }
        }

        private void OnDrawGizmos()
        {
            if (Target)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, Target.position);
            }
        }
    }
}