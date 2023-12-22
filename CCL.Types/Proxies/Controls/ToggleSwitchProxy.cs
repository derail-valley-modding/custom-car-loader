using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class ToggleSwitchProxy : ControlSpecProxy
    {
        private void OnValidate()
        {
            if (this.nonVrStaticInteractionArea != null && this.nonVrStaticInteractionArea.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("nonVrStaticInteractionArea gameObject must be disabled in prefabs! Forcing disable on nonVrStaticInteractionArea gameObject", this);
                this.nonVrStaticInteractionArea.gameObject.SetActive(false);
            }
        }

        [Header("Toggle switch")]
        public Vector3 jointAxis = Vector3.forward;
        public float jointLimitMin;
        public float jointLimitMax;

        public float autoOffTimer;

        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea;

        [Header("Audio")]
        public AudioClip toggle;

        [Header("VR")]
        public Vector3 touchInteractionAxis = Vector3.up;
        public bool disableTouchUse;
    }
}
