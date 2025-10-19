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
        public VRControllerButton overrideUseButton;

        private void OnDrawGizmos()
        {
            var offset = useJoints ? transform.TransformDirection(Vector3.back * linearLimit) : pushLocalOffset;
            var pos = transform.position;

            Gizmos.color = END_COLOR;
            Gizmos.DrawLine(pos, pos + offset);
            Gizmos.DrawLine(pos, pos - offset);
            Gizmos.color = START_COLOR;
            Gizmos.DrawWireSphere(pos + offset, 0.0025f);
            Gizmos.color = MID_COLOR;
            Gizmos.DrawWireSphere(pos, 0.00125f);
        }
    }
}
