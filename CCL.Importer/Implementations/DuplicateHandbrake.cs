using CCL.Importer.Components;
using DV.Simulation.Controllers;
using DV.ThingTypes;

namespace CCL.Importer.Implementations
{
    internal class DuplicateHandbrake : HandbrakeControl
    {
        private TrainCar? _coupled;
        private DuplicateHandbrakeOverriderInternal _overrider;
        private bool _fromMain = false;
        private bool _fromDupe = false;

        public override float Value => _coupled != null ? _coupled.brakeSystem.handbrakePosition : base.Value;

        public DuplicateHandbrake(TrainCar car, DuplicateHandbrakeOverriderInternal overrider) : base(car)
        {
            _overrider = overrider;

            var coupler = overrider.Direction.IsFront() ? car.frontCoupler : car.rearCoupler;

            coupler.Coupled += OnCoupled;
            coupler.Uncoupled += OnUncoupled;

            if (coupler.coupledTo)
            {
                OnCoupled(this, new CoupleEventArgs(coupler, coupler.coupledTo, false));
            }
        }

        public override void Set(float value)
        {
            base.Set(value);

            if (_coupled != null)
            {
                _coupled.brakeSystem.SetHandbrakePosition(value, true);
            }
        }

        private void OnUncoupled(object sender, UncoupleEventArgs e)
        {
            if (e.otherCoupler && (MeetsConditions(e.otherCoupler.train.carLivery) && _coupled == e.otherCoupler.train))
            {
                car.brakeSystem.HandbrakePositionChanged -= OnDupeControlUpdated;
                _coupled.brakeSystem.HandbrakePositionChanged -= OnControlUpdatedCopy;
                _coupled = null;
            }
        }

        private void OnCoupled(object sender, CoupleEventArgs e)
        {
            if (e.otherCoupler && MeetsConditions(e.otherCoupler.train.carLivery))
            {
                _coupled = e.otherCoupler.train;
                _coupled.brakeSystem.HandbrakePositionChanged += OnControlUpdatedCopy;
                car.brakeSystem.HandbrakePositionChanged += OnDupeControlUpdated;

                CopyHandbrake();
            }
        }

        private void OnControlUpdatedCopy((float value, bool forced) args)
        {
            CopyHandbrake();
            OnControlUpdated(args);
        }

        private void CopyHandbrake()
        {
            if (_fromDupe)
            {
                _fromDupe = false;
                return;
            }

            if (_coupled != null)
            {
                _fromMain = true;
                car.brakeSystem.SetHandbrakePosition(_coupled.brakeSystem.handbrakePosition, true);
            }
        }

        private void OnDupeControlUpdated((float value, bool forced) args)
        {
            CopyDupeHandbrake();
        }

        private void CopyDupeHandbrake()
        {
            if (_fromMain)
            {
                _fromMain = false;
                return;
            }

            if (_coupled != null)
            {
                _fromDupe = true;
                _coupled.brakeSystem.SetHandbrakePosition(car.brakeSystem.handbrakePosition, true);
            }
        }

        private bool MeetsConditions(TrainCarLivery car)
        {
            if (_overrider.AlwaysCopy) return true;

            foreach (var item in _overrider.CarKinds)
            {
                if (car.parentType.kind.id == item) return true;
            }

            foreach (var item in _overrider.CarTypes)
            {
                if (car.parentType.id == item) return true;
            }

            foreach (var item in _overrider.CarLiveries)
            {
                if (car.id == item) return true;
            }

            return false;
        }
    }
}
