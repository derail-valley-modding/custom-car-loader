using DV.CabControls;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Components.Controls
{
    internal class ControlConstantFeederInternal : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public float Multiplier = 1.0f;
        public bool Constant = false;

        private ControlImplBase _controlSelf = null!;
        private ControlImplBase _controlOther = null!;
        private bool _init = false;

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

            _init = true;
        }

        private void Update()
        {
            if (!_init) return;

            var value = _controlSelf.Value;

            if (value > 0.001f)
            {
                value = Constant ? Multiplier : value * Multiplier;
                _controlOther.SetValue(_controlOther.Value + value * Time.deltaTime);
            }
        }
    }
}
