using DV.CabControls;
using DV.VRTK_Extensions;
using UnityEngine;
using VRTK;

namespace CCL.Importer.Implementations.Controls
{
    internal class ScrewRotatorHaptics : MonoBehaviour
    {
        [Tooltip("Enable haptics when controller is overlapping but not grabbing")]
        public bool EnableWhenTouching = true;
        public float NotchAngle = 1f;

        private ScrewBase _screw = null!;
        private VRTK_InteractableObject _interactable = null!;

        public bool TouchedByLeft { get; private set; }
        public bool TouchedByRight { get; private set; }
        public bool GrabbedByLeft { get; private set; }
        public bool GrabbedByRight { get; private set; }

        private void Start()
        {
            _screw = GetComponent<ScrewBase>();
            _interactable = GetComponent<VRTK_InteractableObject>();
            SetupListeners(true);
        }

        private void SetupListeners(bool on)
        {
            if (on)
            {
                _screw.ValueChanged += OnValueChanged;
                _interactable.InteractableObjectTouched += OnTouched;
                _interactable.InteractableObjectGrabbed += OnGrabbed;
                _interactable.InteractableObjectUntouched += OnUntouched;
                _interactable.InteractableObjectUngrabbed += OnUngrabbed;
            }
            else
            {
                _screw.ValueChanged -= OnValueChanged;
                _interactable.InteractableObjectTouched -= OnTouched;
                _interactable.InteractableObjectGrabbed -= OnGrabbed;
                _interactable.InteractableObjectUntouched -= OnUntouched;
                _interactable.InteractableObjectUngrabbed -= OnUngrabbed;
            }
        }

        private void OnGrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left).scriptAlias)
            {
                GrabbedByLeft = true;
            }

            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right).scriptAlias)
            {
                GrabbedByRight = true;
            }
        }

        private void OnTouched(object sender, InteractableObjectEventArgs e)
        {
            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left).scriptAlias)
            {
                TouchedByLeft = true;
            }

            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right).scriptAlias)
            {
                TouchedByRight = true;
            }
        }

        private void OnUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left).scriptAlias)
            {
                GrabbedByLeft = false;
            }

            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right).scriptAlias)
            {
                GrabbedByRight = false;
            }
        }

        private void OnUntouched(object sender, InteractableObjectEventArgs e)
        {
            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left).scriptAlias)
            {
                TouchedByLeft = false;
            }

            if (e.interactingObject == VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right).scriptAlias)
            {
                TouchedByRight = false;
            }
        }

        private void OnValueChanged(ValueChangedEventArgs e)
        {
            if ((_interactable.IsGrabbed() || _interactable.IsTouched()) && e.delta > NotchAngle)
            {
                if (GrabbedByLeft || (TouchedByLeft && EnableWhenTouching))
                {
                    HapticUtils.DoHapticPulse(VRTK_ControllerReference.GetControllerReference(
                        SDK_BaseController.ControllerHand.Left), GrabbedByLeft ? HapticIntensityType.Strong : HapticIntensityType.Weak);
                }

                if (GrabbedByRight || (TouchedByRight && EnableWhenTouching))
                {
                    HapticUtils.DoHapticPulse(VRTK_ControllerReference.GetControllerReference(
                        SDK_BaseController.ControllerHand.Right), GrabbedByRight ? HapticIntensityType.Strong : HapticIntensityType.Weak);
                }
            }
        }
    }
}
