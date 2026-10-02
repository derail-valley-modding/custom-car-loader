using DV.HUD;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using LocoSim.Attributes;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Controllers
{
    internal class AlerterControllerInternal : ASimInitializedController
    {
        [PortId(PortType.EXTERNAL_IN)]
        public string AlerterActivePortId = string.Empty;
        [PortId(PortValueType.STATE)]
        public string AlerterStatePortId = string.Empty;
        public float ControlsImpulseInterval = 0.2f;

        private ILocomotiveRemoteControl? _remote;
        private TrainCar _car = null!;
        private BaseControlsOverrider _controls = null!;
        private Port _activePort = null!;
        private Port _statePort = null!;
        private float _controlTimer;

        public override bool ExternalTick => true;

        private bool IsRemoteActive => _remote != null && _remote.IsActivelyControlled;

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            _car = car;
            _remote = car.GetComponent<ILocomotiveRemoteControl>();
            var controls = car.SimController?.controlsOverrider;

            if (controls == null)
            {
                CCLPlugin.Error("AlerterController could not find controls! Destroying self");
                Destroy(this);
                return;
            }

            if (!simFlow.TryGetPort(AlerterActivePortId, out _activePort) || !simFlow.TryGetPort(AlerterStatePortId, out _statePort))
            {
                CCLPlugin.Error("AlerterController is not initialized properly! Destroying self");
                Destroy(this);
                return;
            }

            _controls = controls;
        }

        public override void Tick(float deltaTime)
        {
            // Disable if the loco is being remote controlled.
            _activePort.Value = IsRemoteActive ? 0 : 1;

            if (_statePort.Value < 2) return;

            // This implementation is effectively the same as the ATS Gadget.
            _controlTimer += deltaTime;
            if (_controlTimer <= ControlsImpulseInterval) return;

            _controlTimer -= ControlsImpulseInterval;
            UnhandControl(InteriorControlsManager.ControlType.Throttle);
            UnhandControl(InteriorControlsManager.ControlType.TrainBrake);
            _controls.Throttle.Move(-1f);
            _controls.Brake.Move(1f);
        }

        private void UnhandControl(InteriorControlsManager.ControlType type)
        {
            if (_car.loadedInterior == null ||
                !_car.loadedInterior.TryGetComponent(out InteriorControlsManager icm) ||
                !icm.TryGetControl(type, out var reference) ||
                !reference.controlImplBase.IsGrabbed()) return;

            reference.controlImplBase.ForceEndInteraction();
        }
    }
}
