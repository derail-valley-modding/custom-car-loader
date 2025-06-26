using DV.Simulation.Cars;
using UnityEngine;

namespace CCL.Importer.Components.Headlights
{
    internal class NoCableHeadlightsControllerInternal : MonoBehaviour
    {
        private TrainCar _car = null!;
        private HeadlightsMainController _controller = null!;
        private HeadlightsMainController? _frontComp;
        private HeadlightsMainController? _rearComp;
        private bool _setupFlag = false;

        private void Start()
        {
            _car = TrainCar.Resolve(gameObject);

            if (_car == null)
            {
                Debug.LogError("Could not find TrainCar for NoCableHeadlight, destroying self!");
                Destroy(this);
                return;
            }

            _controller = _car.SimController != null ? _car.SimController.headlightsController : null!;

            if (_controller == null)
            {
                Debug.LogError("Could not find HeadlightMainController for NoCableHeadlight, destroying self!");
                Destroy(this);
                return;
            }

            if (_car.frontCoupler != null)
            {
                _car.frontCoupler.Coupled += Coupled;
                _car.frontCoupler.Uncoupled += Uncoupled;
                _car.frontCoupler.HoseConnectionChanged += HoseChanged;
            }

            if (_car.rearCoupler != null)
            {
                _car.rearCoupler.Coupled += Coupled;
                _car.rearCoupler.Uncoupled += Uncoupled;
                _car.rearCoupler.HoseConnectionChanged += HoseChanged;
            }

            RefreshConnections();
        }

        private void OnDestroy()
        {
            if (_car.frontCoupler != null)
            {
                _car.frontCoupler.Coupled -= Coupled;
                _car.frontCoupler.Uncoupled -= Uncoupled;
            }

            if (_car.rearCoupler != null)
            {
                _car.rearCoupler.Coupled -= Coupled;
                _car.rearCoupler.Uncoupled -= Uncoupled;
            }
        }

        private void Coupled(object sender, CoupleEventArgs e)
        {
            RefreshConnections();
        }

        private void Uncoupled(object sender, UncoupleEventArgs e)
        {
            RefreshConnections();
        }

        private void HoseChanged(bool connected, bool isFront, bool playAudio)
        {
            if (isFront)
            {
                UpdateFrontHeadlights(_controller.headlightControlFront.Value);
            }
            else
            {
                UpdateRearHeadlights(_controller.headlightControlRear.Value);
            }
        }

        private void SetupChanged(HeadlightsMainController.HeadlightSetup newSetup, HeadlightsMainController.HeadlightSetup oldSetup, bool front)
        {
            if (_setupFlag) return;

            _setupFlag = true;
            UpdateFrontHeadlights(_controller.headlightControlFront.Value);
            UpdateRearHeadlights(_controller.headlightControlRear.Value);
            _setupFlag = false;
        }

        private void RefreshConnections()
        {
            ResetHeadlights();
            Disconnect(_frontComp);
            Disconnect(_rearComp);

            // Check for front coupling.
            var frontCar = _car.frontCoupler.coupledTo != null ? _car.frontCoupler.coupledTo.train : null;

            if (frontCar != null)
            {
                // If we are coupled at the front, get the headlight controller there.
                var frontComp = frontCar.SimController != null ? frontCar.SimController.headlightsController : null;

                Connect(frontComp, ConnectedFrontToFront(_car, frontCar));
                _frontComp = frontComp;
            }

            // Repeat steps for the rear.
            var rearCar = _car.rearCoupler.coupledTo != null ? _car.rearCoupler.coupledTo.train : null;

            if (rearCar != null)
            {
                var rearComp = rearCar.SimController != null ? rearCar.SimController.headlightsController : null;

                Connect(rearComp, ConnectedRearToRear(_car, rearCar));
                _rearComp = rearComp;
            }
        }

        private void Connect(HeadlightsMainController? controller, bool reverse)
        {
            if (controller == null) return;

            if (reverse)
            {
                controller.headlightControlFront.ValueUpdatedInternally += UpdateRearHeadlights;
                controller.headlightControlRear.ValueUpdatedInternally += UpdateFrontHeadlights;
                controller.HeadlightSetupChanged += SetupChanged;

                UpdateFrontHeadlights(controller.headlightControlRear.Value);
                UpdateRearHeadlights(controller.headlightControlFront.Value);
            }
            else
            {
                controller.headlightControlFront.ValueUpdatedInternally += UpdateFrontHeadlights;
                controller.headlightControlRear.ValueUpdatedInternally += UpdateRearHeadlights;
                controller.HeadlightSetupChanged += SetupChanged;

                UpdateFrontHeadlights(controller.headlightControlFront.Value);
                UpdateRearHeadlights(controller.headlightControlRear.Value);
            }

            if (controller.PowerFuse != null && _controller.PowerFuse != null)
            {
                controller.PowerFuse.StateUpdated += _controller.PowerFuse.ChangeState;

                _controller.PowerFuse.ChangeState(controller.PowerFuse.State);
            }
        }

        private void Disconnect(bool front)
        {
            HeadlightsMainController? controller;

            if (front)
            {
                controller = _frontComp;
                _frontComp = null;
            }
            else
            {
                controller = _rearComp;
                _rearComp = null;
            }

            if (controller == null) return;

            controller.headlightControlFront.ValueUpdatedInternally -= UpdateFrontHeadlights;
            controller.headlightControlFront.ValueUpdatedInternally -= UpdateRearHeadlights;
            controller.headlightControlRear.ValueUpdatedInternally -= UpdateFrontHeadlights;
            controller.headlightControlRear.ValueUpdatedInternally -= UpdateRearHeadlights;
            controller.HeadlightSetupChanged -= SetupChanged;

            if (controller.PowerFuse != null && _controller.PowerFuse != null)
            {
                controller.PowerFuse.StateUpdated -= _controller.PowerFuse.ChangeState;
            }

            controller.OnConnectionChanged(false, false);
        }

        private void ResetHeadlights()
        {
            _controller.headlightControlFront.Value = _controller.GetNeutralPortValue(true);
            _controller.headlightControlRear.Value = _controller.GetNeutralPortValue(false);
            _controller.PowerFuse?.ChangeState(false);
        }

        private void UpdateFrontHeadlights(float value)
        {
            _controller.headlightControlFront.ExternalValueUpdate(value);

            if (!HoseConnected(true)) return;

            _controller.ForceTurnOffHeadlights(true);

            if (_frontComp == null) return;

            _frontComp.ForceTurnOffHeadlights(_frontComp.car.IsFrontCoupler(_car.frontCoupler.coupledTo));
        }

        private void UpdateRearHeadlights(float value)
        {
            _controller.headlightControlRear.ExternalValueUpdate(value);

            if (!HoseConnected(false)) return;

            _controller.ForceTurnOffHeadlights(false);

            if (_rearComp == null) return;

            _rearComp.ForceTurnOffHeadlights(_rearComp.car.IsFrontCoupler(_car.rearCoupler.coupledTo));
        }

        private bool HoseConnected(bool front)
        {
            var coupler = front ? _car.frontCoupler : _car.rearCoupler;
            return coupler != null && coupler.hoseAndCock != null && coupler.hoseAndCock.IsHoseConnected;
        }

        private static bool ConnectedFrontToFront(TrainCar a, TrainCar b)
        {
            return a.frontCoupler != null && b.frontCoupler != null && a.frontCoupler.coupledTo == b.frontCoupler;
        }

        private static bool ConnectedRearToRear(TrainCar a, TrainCar b)
        {
            return a.rearCoupler != null && b.rearCoupler != null && a.rearCoupler.coupledTo == b.rearCoupler;
        }
    }
}
