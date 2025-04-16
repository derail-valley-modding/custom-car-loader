using DV.MultipleUnit;
using DV.Simulation.Controllers;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Components.MultipleUnit
{
    public abstract class MultipleUnitExtraControlInternal<T> : ASimInitializedController
        where T : MultipleUnitExtraControlInternal<T>
    {
        private MultipleUnitModule _module = null!;

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            _module = car.muModule;

            if (_module == null)
            {
                Debug.LogError("(MultipleUnitExtraControl) TrainCar has no muModule!");
                Destroy(this);
                return;
            }
        }

        public abstract void SetValue(T source);

        public void ValueChanged()
        {
            switch (_module.Mode)
            {
                case MultipleUnitModule.MultipleUnitMode.CABLE:
                    PropagateThroughCable(true);
                    PropagateThroughCable(false);
                    break;
                case MultipleUnitModule.MultipleUnitMode.RADIO:
                    if (_module.RemoteChannel.Transmitter != this)
                    {
                        return;
                    }

                    foreach (var device in _module.RemoteChannel.devices)
                    {
                        TrySetValue(device);
                    }
                    break;
                default:
                    Debug.LogError($"Unexpected mode: {_module.Mode}! Ignoring");
                    break;
            }
        }

        private void PropagateThroughCable(bool direction)
        {
            var module = _module;

            while (module != null)
            {
                var cable = direction ? module.FrontCable : module.RearCable;

                if (!cable.IsConnected) return;

                direction = !cable.connectedTo.isFront;
                module = cable.connectedTo.muModule;
                TrySetValue(module);
            }
        }

        private void TrySetValue(MultipleUnitModule module)
        {
            if (module != this && module.train.TryGetComponent(out T comp))
            {
                comp.SetValue((T)this);
            }
        }
    }
}
