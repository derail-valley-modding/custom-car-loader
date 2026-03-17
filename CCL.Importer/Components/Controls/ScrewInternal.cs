using DV.CabControls.Spec;
using DV.Interaction;
using UnityEngine;

namespace CCL.Importer.Components.Controls
{
    internal class ScrewInternal : ControlSpec
    {
        public StaticInteractionArea nonVrStaticInteractionArea = null!;

        public int Revolutions = 10;
        public bool InvertDirection = true;
        public float ScrollWheelHoverScroll = 100;
        public float Travel = 1;

        public float Mass = 1;
        public float AngularDrag = 1;
        public bool ZeroCenterOfMass;

        public float Spring = 2;
        public float Damper = 2;
        public float Bounciness;
        public float BounceMinVelocity;

        public float RotatorMaxForceMagnitude = 0.5f;
        public float RotatorForceMultiplier = 0.1f;

        public bool UseHaptics = true;
        public float NotchAngle = 1f;
        public bool EnableWhenTouching;
        
        public AudioClip? Drag;
        public AudioClip? LimitHit;
        public float HitTolerance = 0.1f;

        public override InteractableTag InteractableTag => InteractableTag.Wheel;
    }
}
