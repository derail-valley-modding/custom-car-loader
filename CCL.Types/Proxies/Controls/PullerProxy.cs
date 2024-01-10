using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class PullerProxy : ControlSpecProxy
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
        public float rigidbodyMass = 5f;
        public float rigidbodyDrag = 15f;

        [Header("Stepped puller")]
        [Header("Puller")]
        public bool useSteppedPuller;

        public int notches = 20;
        public bool invertDirection;
        public float scrollWheelHoverScroll = 0.025f;

        [Header("Configurable Joint")]
        public bool useCustomConnectionAnchor;
        public Transform connectionAnchor;
        public Transform pivot;
        public float linearLimit = 0.003f;

        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea;

        [Header("Audio")]
        public AudioClip notch;
        public AudioClip drag;
        public AudioClip limitHit;

        private void OnDrawGizmos()
        {
            Vector3 movedOffset = transform.TransformPoint(Vector3.up * linearLimit);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, movedOffset);
        }
    }
}
