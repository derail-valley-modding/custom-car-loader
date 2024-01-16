using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class LeverProxy : ControlSpecProxy
    {
        private void OnValidate()
        {
            if (this.nonVrStaticInteractionArea != null && this.nonVrStaticInteractionArea.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("nonVrStaticInteractionArea gameObject must be disabled in prefabs! Forcing disable on nonVrStaticInteractionArea gameObject", this);
                this.nonVrStaticInteractionArea.gameObject.SetActive(false);
            }
        }

        [Header("Rigidbody")]
        public float rigidbodyMass = 30f;
        public float rigidbodyDrag = 15f;
        public float rigidbodyAngularDrag;
        public float blockDrag;
        public float blockAngularDrag;

        [Header("Lever")]
        public bool invertDirection;

        [Tooltip("Optional")]
        public Transform interactionPoint;

        public float maxForceAppliedMagnitude = float.PositiveInfinity;
        public float pullingForceMultiplier = 1f;
        public float scrollWheelHoverScroll = 0.025f;
        public float scrollWheelSpring;

        [Header("Notches")]
        public bool useSteppedJoint = true;
        public bool steppedValueUpdate = true;
        public int notches = 20;
        public bool useInnerLimitSpring;

        public int innerLimitMinNotch;
        public int innerLimitMaxNotch;

        [Header("Hinge Joint")]
        public Vector3 jointAxis = Vector3.up;

        public bool useSpring = true;
        public float jointSpring = 100f;
        public float jointDamper;
        public bool useLimits = true;
        public float jointLimitMin;
        public float jointLimitMax;

        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea;

        [Header("Audio")]
        public AudioClip notch;
        public AudioClip drag;
        public AudioClip limitHit;
        public bool limitVibration;
    }
}
