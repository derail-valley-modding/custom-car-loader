using CCL.Importer.Components.Controls;
using DV.Interaction;
using UnityEngine;

namespace CCL.Importer.Implementations.Controls
{
    internal class PullableRopeNonVR : PullableRopeBase
    {
        private class GrabHandlerRope : AGrabHandler
        {
            public PullableRopeInternal Rope = null!;

            public override bool IsItem => false;

            protected override void Start()
            {
                base.Start();
                enabled = false;
            }

            public override Vector3 GetAnchor()
            {
                return Vector3.zero;
            }

            public override Vector3 GetAxis()
            {
                return Vector3.right;
            }

            public override void FeedPosition(Vector3 worldPosition)
            {
                // Ensure the object is not pulled further away than max length.
                var dif = worldPosition - Rope.Origin.position;

                if (dif.sqrMagnitude > Rope.MaxLength * Rope.MaxLength)
                {
                    transform.position = Rope.Origin.position + dif.normalized * Rope.MaxLength;
                }
                else
                {
                    transform.position = worldPosition;
                }
            }

            public override void EndInteraction()
            {
                base.EndInteraction();
                enabled = false;
            }
        }

        private GrabHandlerRope _grabHandler = null!;

        protected override void Awake()
        {
            base.Awake();

            _grabHandler = AGrabHandler.AddGrabHandler<GrabHandlerRope>(gameObject, InteractionColliderObjects);
            _grabHandler.Rope = Spec;
            _grabHandler.Grabbed += FireGrabbed;
            _grabHandler.UnGrabbed += FireUngrabbed;
            InteractionAllowedChanged += x => _grabHandler.interactionAllowed = x;

            if (Spec.nonVrStaticInteractionArea != null)
            {
                Spec.nonVrStaticInteractionArea.Initialize(_grabHandler, gameObject.layer);
            }
        }

        public override bool IsGrabbed()
        {
            return _grabHandler != null && _grabHandler.IsGrabbed();
        }

        public override void ForceEndInteraction()
        {
            if (_grabHandler == null) return;

            _grabHandler.ForceEndInteraction();
        }
    }
}
