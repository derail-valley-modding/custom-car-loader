using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class WheelProxy : ControlSpecProxy
    {
        private void OnValidate()
        {
            if (this.nonVrStaticInteractionArea != null && this.nonVrStaticInteractionArea.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("nonVrStaticInteractionArea gameObject must be disabled in prefabs! Forcing disable on nonVrStaticInteractionArea gameObject", this);
                this.nonVrStaticInteractionArea.gameObject.SetActive(false);
            }
        }

        [Header("RigidBody")]
        public float mass = 1f;
        public float angularDrag = 1f;

        [Header("Hinge Joint")]
        public Vector3 jointAxis = Vector3.up;
        public bool useSpring;
        public float jointSpring;
        public float springDamper;

        public bool useLimits = true;
        public float jointLimitMin;
        public float jointLimitMax;

        public float bounciness;
        public float bounceMinVelocity;

        [Tooltip("A value between joint min and max limit")]
        public float jointStartingPos;

        public bool invertDirection = true;
        public float scrollWheelHoverScroll = 1f;

        [Header("RotatorTrack")]
        public float rotatorMaxForceMagnitude = 0.1f;

        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea;

        [Header("Audio")]
        public AudioClip drag;
        public AudioClip limitHit;

        public float hitTolerance = 0.1f;

        [Header("Haptics")]
        public bool useHaptics = true;
        public float notchAngle = 1f;
        public bool enableWhenTouching;
    }
}
