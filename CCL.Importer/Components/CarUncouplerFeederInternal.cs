using CCL.Types;
using DV.CabControls;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class CarUncouplerFeederInternal : MonoBehaviour
    {
        private ControlImplBase? _control;
        private Coupler _coupler = null!;

        public CouplerDirection Coupler;

        public void Init(Coupler coupler)
        {
            _coupler = coupler;
            _control = GetComponent<ControlImplBase>();

            if (_control == null)
            {
                Debug.LogError($"Cannot find control for CarUncouplerFeeder {name}!");
                return;
            }

            _control.ValueChanged += ControlChanged;
        }

        public void Deinit()
        {
            if (_control != null)
            {
                _control.ValueChanged -= ControlChanged;
            }
        }

        private void ControlChanged(ValueChangedEventArgs args)
        {
            if (args.oldValue < 0.5f && args.newValue >= 0.5f)
            {
                _coupler.Uncouple();
            }
        }
    }
}
