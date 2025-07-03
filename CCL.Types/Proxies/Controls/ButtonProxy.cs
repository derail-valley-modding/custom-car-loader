using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Button Proxy")]
    public class ButtonProxy : ControlSpecProxy
    {
        [Header("Button")]
        public bool createRigidbody = true;
        public bool useJoints = true;
        public float pushStrength = 0.5f;
        public float linearLimit = 0.003f;
        public Vector3 pushLocalOffset;
        public bool isToggle;
        public bool isTogglingBack;

        [Header("Audio")]
        public AudioClip press = null!;
        public AudioClip toggleOn = null!;
        public AudioClip toggleOff = null!;
        public bool play2DAudio;

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
