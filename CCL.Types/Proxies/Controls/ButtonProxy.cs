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

        // Token: 0x04004756 RID: 18262
        public bool useJoints = true;

        // Token: 0x04004757 RID: 18263
        public float pushStrength = 0.5f;

        // Token: 0x04004758 RID: 18264
        public float linearLimit = 0.003f;

        // Token: 0x04004759 RID: 18265
        public bool isToggle;

        // Token: 0x0400475A RID: 18266
        public bool isTogglingBack;

        // Token: 0x0400475B RID: 18267
        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea;

        // Token: 0x0400475C RID: 18268
        [Header("Audio")]
        public AudioClip press;

        // Token: 0x0400475D RID: 18269
        public AudioClip toggleOn;

        // Token: 0x0400475E RID: 18270
        public AudioClip toggleOff;

        // Token: 0x0400475F RID: 18271
        [Header("VR")]
        public bool disableTouchUse;

        // Token: 0x04004760 RID: 18272
        public VRButtonAlias overrideUseButton;
    }
}
