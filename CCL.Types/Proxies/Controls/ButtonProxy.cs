using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class ButtonProxy : ControlSpecProxy
    {
        [Header("Button")]
        public bool createRigidbody = true;
        public bool useJoints = true;
        public float pushStrength = 0.5f;
        public float linearLimit = 0.003f;
        public bool isToggle;
        public bool isTogglingBack;

        [Header("Audio")]
        public AudioClip press;
        public AudioClip toggleOn;
        public AudioClip toggleOff;

        [Header("VR")]
        public bool disableTouchUse;
        public VRButtonAlias overrideUseButton;

        private void OnDrawGizmos()
        {
            Vector3 pressedOffset = transform.TransformPoint(Vector3.back * linearLimit);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, pressedOffset);
        }
    }
}
