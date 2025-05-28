using DV.Simulation.Cars;
using UnityEngine;

namespace CCL.Importer.Components.Headlights
{
    internal class NoCableHeadlightsInternal : MonoBehaviour
    {
        private TrainCar _car = null!;
        private HeadlightsMainController _controller = null!;
        private HeadlightsMainController? _frontComp;
        private HeadlightsMainController? _rearComp;
        private bool _frontPowered = false;
        private bool _rearPowered = false;

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
            }

            if (_car.rearCoupler != null)
            {
                _car.rearCoupler.Coupled += Coupled;
                _car.rearCoupler.Uncoupled += Uncoupled;
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

        private void RefreshConnections()
        {
            // Check for front coupling.
            var frontCar = _car.frontCoupler.coupledTo != null ? _car.frontCoupler.coupledTo.train : null;

            if (frontCar != null)
            {
                // If we are coupled at the front, get the headlight controller there.
                var frontComp = frontCar.SimController != null ? frontCar.SimController.headlightsController : null;

                // If there was a change to the front component, disconnect the old one and connect the new.
                // If it is null, no worries, since it won't connect.
                if (frontComp != _frontComp)
                {
                    Disconnect(_frontComp);
                    Connect(frontComp, ConnectedFrontToFront(_car, frontCar));

                    _frontComp = frontComp;
                    _frontPowered = !frontCar.GetComponentInChildren<NoCableHeadlightsInternal>();
                }
            }
            else
            {
                // If not coupled, disconnect any existing one.
                Disconnect(_frontComp);
                _frontComp = null;
                _frontPowered = false;
            }

            // Repeat steps for the rear.
            var rearCar = _car.rearCoupler.coupledTo != null ? _car.rearCoupler.coupledTo.train : null;

            if (rearCar != null)
            {
                var rearComp = rearCar.SimController != null ? rearCar.SimController.headlightsController : null;

                if (rearComp != _rearComp)
                {
                    Disconnect(_rearComp);
                    Connect(rearComp, ConnectedRearToRear(_car, rearCar));

                    _rearComp = rearComp;
                    _rearPowered = !rearCar.GetComponentInChildren<NoCableHeadlightsInternal>();
                }
            }
            else
            {
                Disconnect(_rearComp);
                _rearComp = null;
                _rearPowered = false;
            }

            // If both sides are null, reset the headlights.
            // Also reset if neither side is self powered.
            if ((_frontComp == null && _rearComp == null) ||
                !(_frontPowered || _rearPowered))
            {
                ResetHeadlights();
            }
        }

        private void Connect(HeadlightsMainController? controller, bool reverse)
        {
            if (controller == null) return;

            if (reverse)
            {
                controller.headlightControlFront.ValueUpdatedInternally += _controller.headlightControlRear.ExternalValueUpdate;
                controller.headlightControlRear.ValueUpdatedInternally += _controller.headlightControlFront.ExternalValueUpdate;

                _controller.headlightControlFront.Value = controller.headlightControlRear.Value;
                _controller.headlightControlRear.Value = controller.headlightControlFront.Value;
            }
            else
            {
                controller.headlightControlFront.ValueUpdatedInternally += _controller.headlightControlFront.ExternalValueUpdate;
                controller.headlightControlRear.ValueUpdatedInternally += _controller.headlightControlRear.ExternalValueUpdate;

                _controller.headlightControlFront.Value = controller.headlightControlFront.Value;
                _controller.headlightControlRear.Value = controller.headlightControlRear.Value;
            }

            if (controller.PowerFuse != null && _controller.PowerFuse != null)
            {
                controller.PowerFuse.StateUpdated += _controller.PowerFuse.ChangeState;

                _controller.PowerFuse.ChangeState(controller.PowerFuse.State);
            }
        }

        private void Disconnect(HeadlightsMainController? controller)
        {
            if (controller == null) return;

            controller.headlightControlFront.ValueUpdatedInternally -= _controller.headlightControlFront.ExternalValueUpdate;
            controller.headlightControlFront.ValueUpdatedInternally -= _controller.headlightControlRear.ExternalValueUpdate;
            controller.headlightControlRear.ValueUpdatedInternally -= _controller.headlightControlFront.ExternalValueUpdate;
            controller.headlightControlRear.ValueUpdatedInternally -= _controller.headlightControlRear.ExternalValueUpdate;

            if (controller.PowerFuse != null && _controller.PowerFuse != null)
            {
                controller.PowerFuse.StateUpdated -= _controller.PowerFuse.ChangeState;
            }
        }

        private void ResetHeadlights()
        {
            _controller.headlightControlFront.Value = _controller.GetNeutralPortValue(true);
            _controller.headlightControlRear.Value = _controller.GetNeutralPortValue(false);
            _controller.PowerFuse?.ChangeState(false);
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
