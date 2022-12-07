using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class PullerSetup : ControlSetupBase, IApertureTrackable
    {
        public override string TargetTypeName => "DV.CabControls.Spec.Puller";
        public override bool DestroyAfterCreation => false;
        public override CabControlType ControlType => CabControlType.Puller;

        [Header("Basic Settings")]
        [ProxyField("useSteppedPuller")]
        public bool useNotches = false;
        [ProxyField]
        public int notches = 20;
        [ProxyField]
        public bool invertDirection = false;
        [ProxyField("scrollWheelHoverScroll")]
        public float scrollWheelDelta = 0.025f;
        [ProxyField]
        public float linearLimit = 0.003f;

        [Header("Window")]
        public bool TrackAsWindow = false;
        public bool TrackAsAperture => TrackAsWindow;

        [SerializeField]
        private int _cabNumber = -1;
        public int CabNumber
        {
            get => _cabNumber;
            set => _cabNumber = value;
        }

        [Header("Physics")]
        [ProxyField]
        public float rigidbodyMass = 5f;
        [ProxyField]
        public float rigidbodyDrag = 15f;

        [ProxyField]
        public bool useCustomConnectionAnchor = false;
        [ProxyField]
        public Transform connectionAnchor;
        [ProxyField]
        public Transform pivot;

        [ProxyComponent("nonVrStaticInteractionArea", "StaticInteractionArea")]
        public GameObject StaticInteractionArea = null;

        private void OnDrawGizmos()
        {
            Vector3 movedOffset = transform.TransformPoint(Vector3.up * linearLimit);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, movedOffset);
        }
    }
}