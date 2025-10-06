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
        private bool _init = false;
        private float _prev;

        private IEnumerator Start()
        {
            yield return null;

            _controlSelf = GetComponent<ControlImplBase>();
            _controlOther = ControlObject.GetComponent<ControlImplBase>();

            if (_controlOther == null)
            {
                Debug.LogError($"Failed to find ControlImplBase on object '{ControlObject.name}'!", this);
                yield break;
            }

            yield return null;

            _prev = _controlSelf.Value;
            _init = true;
        }

        private void Update()
        {
            if (!_init) return;

            var value = _controlSelf.Value;
            var dif = value - _prev;

            if (Reverse)
            {
                dif = -dif;
            }

            if (dif > 0)
            {
                _controlOther.SetValue(_controlOther.Value + dif * Multiplier);
            }

            _prev = value;
        }
    }
}
