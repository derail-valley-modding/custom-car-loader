using DV.CabControls;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Components.Controls
{
    internal class ControlRatchetFeederInternal : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public float Multiplier = 1.0f;
        public bool Reverse = false;

        private ControlImplBase _controlSelf = null!;
        private ControlImplBase _controlOther = null!;

        private IEnumerator Start()
        {
            yield return null;

            _controlSelf = GetComponent<ControlImplBase>();
            _controlOther = ControlObject.GetComponent<ControlImplBase>();

            if (_controlSelf == null)
            {
                Debug.LogError($"Failed to find ControlImplBase on object '{name}'!", this);
                yield break;
            }

            if (_controlOther == null)
            {
                Debug.LogError($"Failed to find ControlImplBase on object '{ControlObject.name}'!", this);
                yield break;
            }

            yield return null;

            _controlSelf.ValueChanged += ValueChanged;
        }

        private void OnDestroy()
        {
            if (_controlSelf != null)
            {
                _controlSelf.ValueChanged -= ValueChanged;
            }
        }

        private void ValueChanged(ValueChangedEventArgs args)
        {
            var dif = args.delta;

            if (Reverse)
            {
                dif = -dif;
            }

            if (dif > 0)
            {
                _controlOther.SetValue(_controlOther.Value + dif * Multiplier);
            }
        }
    }
}
