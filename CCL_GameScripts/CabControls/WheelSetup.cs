using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class WheelSetup : ControlSetupBase
    {
        public override string TargetTypeName => "DV.CabControls.Spec.Wheel";
        public override bool IsOverrideSet(int index) => false;
        protected override bool DestroyAfterCreation => true;
        public override CabControlType ControlType => CabControlType.Wheel;

        [Header("Wheel Basic Settings")]
        [ProxyField]
        public bool invertDirection = true;
        [ProxyField("scrollWheelHoverScroll")]
        public float scrollWheelDelta = 1f;

        [Header("Hinge Joint")]
        [ProxyField]
        public Vector3 jointAxis = Vector3.up;
        [ProxyField]
        public bool useSpring = false;
        [ProxyField]
        public float jointSpring = 0;
        [ProxyField]
        public float springDamper = 0;
        [ProxyField]
        public bool useLimits = true;
        [ProxyField]
        public float jointLimitMin;
        [ProxyField]
        public float jointLimitMax;
        [ProxyField]
        public float bounciness;
        [ProxyField]
        public float bounceMinVelocity;

        [Tooltip("A value between joint min and max limit")]
        [ProxyField]
        public float jointStartingPos;

        [Header("Physics")]
        [ProxyField("mass")]
        public float rigidbodyMass = 1f;
        [ProxyField("angularDrag")]
        public float rigidbodyAngularDrag = 1f;

        [ProxyField]
        public float rotatorMaxForceMagnitude = 0.1f;

        [ProxyComponent("nonVrStaticInteractionArea", "StaticInteractionArea")]
        public GameObject StaticInteractionArea = null;

        [Header("Haptics")]
        [ProxyField]
        public bool useHaptics = true;
        [ProxyField]
        public float notchAngle = 1f;
        [ProxyField]
        public bool enableWhenTouching;
    }
}