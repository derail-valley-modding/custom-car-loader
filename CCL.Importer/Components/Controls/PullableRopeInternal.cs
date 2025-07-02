using DV.CabControls.Spec;
using DV.Interaction;
using UnityEngine;

namespace CCL.Importer.Components.Controls
{
    internal class PullableRopeInternal : ControlSpec
    {
        public StaticInteractionArea nonVrStaticInteractionArea = null!;

        public Transform Origin = null!;
        public float RestLength = 0.5f;
        public float Extension = 0.35f;
        public float Tolerance = 0.03f;

        public AudioClip? Drag;
        public AudioClip? LimitHit;
        public bool LimitVibration;

        public override InteractableTag InteractableTag => InteractableTag.Gizmo;
        public float MinLength => RestLength + Tolerance;
        public float MaxLength => RestLength + Extension;
    }
}
