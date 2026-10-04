using DV.CabControls;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Components.Controls
{
    internal class ControlConstantFeederInternal : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public float Multiplier = 1.0f;
        public float Tolerance = 0.01f;
        public bool Constant = false;

        private ControlImplBase _controlSelf = null!;
        private ControlImplBase _controlOther = null!;
        private bool _init = false;

        private IEnumerator Start()
        {
            Tolerance = Mathf.Max(Tolerance, 0.001f);

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

            _init = true;
        }

        private void Update()
        {
            if (!_init) return;

            var value = _controlSelf.Value;

            if (value > Tolerance)
            {
                value = Constant ? Multiplier : value * Multiplier;
                _controlOther.SetValue(_controlOther.Value + value * Time.deltaTime);
            }
        }
    }
}
