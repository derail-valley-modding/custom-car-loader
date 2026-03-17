using DV.Interaction;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Implementations.Controls
{
    internal class ScrewNonVR : ScrewBase
    {
        private GrabHandlerHingeJoint _grab = null!;

        protected override void Awake()
        {
            base.Awake();

            var spring = Joint.spring;
            spring.spring = Spec.Spring;
            spring.damper = Spec.Damper;
            Joint.spring = spring;
            Joint.useSpring = true;

            _grab = AGrabHandler.AddGrabHandler<GrabHandlerHingeJoint>(gameObject, Spec.colliderGameObjects);
            _grab.Grabbed += FireGrabbed;
            _grab.UnGrabbed += FireUngrabbed;
            _grab.invertFeedValueDirection = (Mathf.Sign(Spec.ScrollWheelHoverScroll) * (Spec.InvertDirection ? -1 : 1) < 0f);
            _grab.AssignInteractionPassThrough(new AGrabHandler.InteractionPassThroughDelegate(BaseInteractionPassThrough));

            CoroutineManager.Instance.Run(InitializeStaticArea());
        }

        private IEnumerator InitializeStaticArea()
        {
            while (Spec == null) yield return null;

            yield return WaitFor.EndOfFrame;

            if (Spec.nonVrStaticInteractionArea != null)
            {
                Spec.nonVrStaticInteractionArea.Initialize(_grab, gameObject.layer);
            }
        }

        public override void ForceEndInteraction()
        {
            if (_grab != null)
            {
                _grab.ForceEndInteraction();
            }
        }

        public override bool IsGrabbed()
        {
            return _grab.IsGrabbed();
        }
    }
}
