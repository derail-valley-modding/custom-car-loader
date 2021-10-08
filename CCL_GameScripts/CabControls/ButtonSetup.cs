using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class ButtonSetup : ControlSetupBase
    {
        public override string TargetTypeName => "DV.CabControls.Spec.Button";
        public override bool IsOverrideSet(int index) => false;
        protected override bool DestroyAfterCreation => true;
        public override CabControlType ControlType => CabControlType.Button;

        [ProxyField]
        public float pushStrength = 0.5f;
        [ProxyField]
        public float linearLimit = 0.003f;
        [ProxyField]
        public bool isToggle = false;

        [ProxyComponent("nonVrStaticInteractionArea", "StaticInteractionArea")]
        public GameObject StaticInteractionArea = null;

        // GIZMOS
        private void OnDrawGizmos()
        {
            Vector3 pressedOffset = transform.TransformPoint(Vector3.back * linearLimit);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, pressedOffset);
        }
    }
}