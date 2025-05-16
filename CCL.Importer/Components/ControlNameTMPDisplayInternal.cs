using DV.CabControls;
using TMPro;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class ControlNameTMPDisplayInternal : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public TMP_Text Text = null!;

        private ControlImplBase _control = null!;

        private void Start()
        {
            if (!ControlObject || !ControlObject.TryGetComponent(out _control))
            {
                Debug.LogError("ControlNameTMPDisplay has no control assigned!");
                Destroy(this);
                return;
            }

            if (!Text)
            {
                Debug.LogError("ControlNameTMPDisplay has no text assigned!");
                Destroy(this);
                return;
            }

            Text.text = _control.GetCurrentPositionName().value;
            _control.ValueChanged += UpdateText;
        }

        private void UpdateText(ValueChangedEventArgs _)
        {
            Text.text = _control.GetCurrentPositionName().value;
        }
    }
}
