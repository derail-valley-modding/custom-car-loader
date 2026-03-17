using DV.CabControls;
using DV.CabControls.VRTK;
using DV.Interaction;
using DV.VR;

namespace CCL.Importer.Implementations.Controls
{
    internal class ScrewVRTK : ScrewBase, IGrabPoseProvider
    {
        private VRTK_ControlImplBaseInteractableObject _interactable = null!;

        public HandPose GrabPose => _interactable.interactionHandPoses.grabPose;

        protected override void Awake()
        {
            base.Awake();

            _interactable = gameObject.AddComponent<VRTK_ControlImplBaseInteractableObject>();
            _interactable.isGrabbable = true;
            _interactable.priority = 0;
            _interactable.pipaExclusiveInteraction = false;
            _interactable.controlImplBase = this;
            _interactable.interactionHandPoses = GenerateHandPoses();
            _interactable.InteractableObjectGrabbed += (x, y) => { FireGrabbed(); };
            _interactable.InteractableObjectUngrabbed += (x, y) => { FireUngrabbed(); };

            var force = gameObject.AddComponent<VRTK_RotatorTrackGrabAttachLimitedForce>();
            force.precisionGrab = true;
            force.maxForceMagnitude = Spec.RotatorMaxForceMagnitude;
            force.forceMultiplier = Spec.RotatorForceMultiplier;
            if (Spec.UseHaptics)
            {
                var haptics = gameObject.AddComponent<WheelRotatorHaptics>();
                haptics.enableWhenTouching = Spec.EnableWhenTouching;
                haptics.notchAngle = Spec.NotchAngle;
                DiffThreshold = Spec.NotchAngle;
            }

            _interactable.grabAttachMechanicScript = force;
            gameObject.AddComponent<TelegrabbableGrabbable>();
        }

        public override void ForceEndInteraction()
        {
            _interactable.ForceStopInteracting();
        }

        public override bool IsGrabbed()
        {
            return _interactable.IsGrabbed();
        }
    }
}
