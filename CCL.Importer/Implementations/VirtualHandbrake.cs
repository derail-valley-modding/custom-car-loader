using CCL.Importer.Components;
using DV.Simulation.Controllers;
using DV.ThingTypes;

namespace CCL.Importer.Implementations
{
    internal class VirtualHandbrake : HandbrakeControl
    {
        private TrainCar? _coupled;
        private VirtualHandbrakeOverriderInternal _overrider;

        public override float Value => _coupled != null ? _coupled.brakeSystem.handbrakePosition : 0f;

        protected override bool canExistWithoutHandbrake => true;

        public VirtualHandbrake(TrainCar car, VirtualHandbrakeOverriderInternal overrider) : base(car)
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
            if (_coupled != null)
            {
                _coupled.brakeSystem.SetHandbrakePosition(value, true);
            }
        }

        private void OnUncoupled(object sender, UncoupleEventArgs e)
        {
            if (e.otherCoupler && (MeetsConditions(e.otherCoupler.train.carLivery) && _coupled == e.otherCoupler.train))
            {
                _coupled.brakeSystem.HandbrakePositionChanged -= OnControlUpdated;
                _coupled = null;
            }
        }

        private void OnCoupled(object sender, CoupleEventArgs e)
        {
            if (e.otherCoupler && MeetsConditions(e.otherCoupler.train.carLivery))
            {
                _coupled = e.otherCoupler.train;
                _coupled.brakeSystem.HandbrakePositionChanged += OnControlUpdated;
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
