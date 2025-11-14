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
        private bool _updating = false;

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            _module = car.muModule;

            if (_module == null)
            {
                Debug.LogError("(MultipleUnitExtraControl) TrainCar has no muModule!");
                Destroy(this);
                return;
            }

            _module.frontCable.ConnectionChanged += ConnectionChanged;
            _module.rearCable.ConnectionChanged += ConnectionChanged;
        }

        public abstract void SetValue(T source);

        public void ValueChanged()
        {
            if (_updating) return;

            _updating = true;

            if (_module.UseCable)
            {
                PropagateThroughCable(true);
                PropagateThroughCable(false);
            }

            if (_module.UseWireless && _module.RemoteChannel.Transmitter == _module)
            {
                foreach (var device in _module.RemoteChannel.devices)
                {
                    TrySetValue(device);
                }
            }

            _updating = false;
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
            if (module != _module && module.train.TryGetComponent(out T comp))
            {
                comp.SetValue((T)this);
            }
        }

        protected abstract void ConnectionChanged(bool connected, bool playAudio);
    }
}
