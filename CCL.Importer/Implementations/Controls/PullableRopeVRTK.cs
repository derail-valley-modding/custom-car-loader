using DV.CabControls;
using DV.CabControls.VRTK;
using DV.Interaction;
using DV.VR;
using VRTK.GrabAttachMechanics;

namespace CCL.Importer.Implementations.Controls
{
    internal class PullableRopeVRTK : PullableRopeBase, IGrabPoseProvider
    {
        private VRTK_ControlImplBaseInteractableObject _interactable = null!;

        public HandPose GrabPose => _interactable.interactionHandPoses.grabPose;

        protected override void Awake()
        {
            base.Awake();

            _interactable = gameObject.AddComponent<VRTK_ControlImplBaseInteractableObject>();
            _interactable.priority = 0;
            _interactable.pipaExclusiveInteraction = false;
            _interactable.controlImplBase = this;
            _interactable.interactionHandPoses = GenerateHandPoses();

            _interactable.InteractableObjectGrabbed += (_, _) => FireGrabbed();
            _interactable.InteractableObjectUngrabbed += (_, _) => FireUngrabbed();

            _interactable.grabAttachMechanicScript = gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
            _interactable.grabAttachMechanicScript.precisionGrab = true;

            gameObject.AddComponent<TelegrabbableGrabbable>();
        }

        public override bool IsGrabbed()
        {
            return _interactable.IsGrabbed();
        }

        public override void ForceEndInteraction()
        {
            _interactable.ForceStopInteracting();
        }
    }
}
