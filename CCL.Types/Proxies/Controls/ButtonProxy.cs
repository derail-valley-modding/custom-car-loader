using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class ButtonProxy : ControlSpecProxy
    {
        private void OnValidate()
        {
            if (this.nonVrStaticInteractionArea != null && this.nonVrStaticInteractionArea.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("nonVrStaticInteractionArea gameObject must be disabled in prefabs! Forcing disable on nonVrStaticInteractionArea gameObject", this);
                this.nonVrStaticInteractionArea.gameObject.SetActive(false);
            }
        }

        [Header("Button")]
        public bool createRigidbody = true;
        public bool useJoints = true;
        public float pushStrength = 0.5f;
        public float linearLimit = 0.003f;
        public bool isToggle;
        public bool isTogglingBack;

        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea;

        [Header("Audio")]
        public AudioClip press;
        public AudioClip toggleOn;
        public AudioClip toggleOff;

        [Header("VR")]
        public bool disableTouchUse;
        public VRButtonAlias overrideUseButton;
    }
}
