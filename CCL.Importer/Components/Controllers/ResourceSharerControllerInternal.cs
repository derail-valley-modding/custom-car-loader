using CCL.Types;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using LocoSim.Attributes;
using LocoSim.Definitions;
using LocoSim.Implementations;
using LocoSim.Resources;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Components.Controllers
{
    internal class ResourceSharerControllerInternal : ASimInitializedController
    {
        public ResourceContainerType Type = ResourceContainerType.WATER;
        public float MaxTransfer = 500.0f;
        public float MinTransfer = 10.0f;
        public bool AllowSharingToFront = true;
        public bool AllowSharingToRear = true;

        [PortId(PortType.READONLY_OUT)]
        public string CapacityPortId = string.Empty;
        [PortId(PortType.READONLY_OUT)]
        public string AmountPortId = string.Empty;
        [PortId(PortType.EXTERNAL_IN)]
        public string ConsumePortId = string.Empty;
        [PortId(PortType.EXTERNAL_IN)]
        public string RefillPortId = string.Empty;

        private Port _capacity = null!;
        private Port _amount = null!;
        private Port _consume = null!;
        private Port _refill = null!;

        private ResourceSharerControllerInternal? _frontShared;
        private ResourceSharerControllerInternal? _rearShared;
        private CarPitStopParametersBase? _frontPit;
        private CarPitStopParametersBase? _rearPit;

        public override bool ExternalTick => true;
        public float Normalized => _amount.Value / _capacity.Value;

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            if (!simFlow.TryGetPort(CapacityPortId, out _capacity) ||
                !simFlow.TryGetPort(AmountPortId, out _amount) ||
                !simFlow.TryGetPort(ConsumePortId, out _consume) ||
                !simFlow.TryGetPort(RefillPortId, out _refill))
            {
                Debug.LogError("ResourceSharerController can't function! Destroying self!", this);
                Destroy(this);
            }

            if (car.frontCoupler != null && AllowSharingToFront)
            {
                car.frontCoupler.Coupled += FrontCoupled;
                car.frontCoupler.Uncoupled += FrontUncoupled;

                if (car.frontCoupler.coupledTo != null)
                {
                    FrontCoupled(false, car.frontCoupler.CreateDummyArgs());
                }
            }

            if (car.rearCoupler != null && AllowSharingToRear)
            {
                car.rearCoupler.Coupled += RearCoupled;
                car.rearCoupler.Uncoupled += RearUncoupled;

                if (car.rearCoupler.coupledTo != null)
                {
                    RearCoupled(false, car.rearCoupler.CreateDummyArgs());
                }
            }
        }

        private void FrontCoupled(object sender, CoupleEventArgs e)
        {
            _frontShared = e.otherCoupler.train.GetComponentsInChildren<ResourceSharerControllerInternal>().FirstOrDefault(x => x.Type == Type);

            if (_frontShared != null) return;

            var pit = e.otherCoupler.train.GetComponentInChildren<CarPitStopParametersBase>();

            if (pit != null && pit.carPitStopParameters.ContainsKey(Type.ConvertToDVResource()))
            {
                _frontPit = pit;
            }
        }

        private void FrontUncoupled(object sender, UncoupleEventArgs e)
        {
            _frontShared = null;
            _frontPit = null;
        }

        private void RearCoupled(object sender, CoupleEventArgs e)
        {
            _rearShared = e.otherCoupler.train.GetComponentsInChildren<ResourceSharerControllerInternal>().FirstOrDefault(x => x.Type == Type);

            if (_rearShared != null) return;

            var pit = e.otherCoupler.train.GetComponentInChildren<CarPitStopParametersBase>();

            if (pit != null && pit.carPitStopParameters.ContainsKey(Type.ConvertToDVResource()))
            {
                _rearPit = pit;
            }
        }

        private void RearUncoupled(object sender, UncoupleEventArgs e)
        {
            _rearShared = null;
            _rearPit = null;
        }

        public override void Tick(float deltaTime)
        {
            // First try to transfer to a shared resource thingy, before
            // retrying with the pit stop abuse.
            if (_frontShared != null)
            {
                DoTransferShare(_frontShared);
            }
            else if (_frontPit != null)
            {
                DoTransferPit(_frontPit);
            }

            // Same for the rear.
            if (_rearShared != null)
            {
                DoTransferShare(_rearShared);
            }
            else if (_rearPit != null)
            {
                DoTransferPit(_rearPit);
            }

            void DoTransferShare(ResourceSharerControllerInternal other)
            {
                // Calculate total transfer needed.
                var transfer = MathHelper.MaxTransfer2ContainersPositiveOnly(
                    _capacity.Value, _amount.Value, other._capacity.Value, other._amount.Value);

                // Cap the transfer rate based on max and min transfer rates, and the difference between amounts.
                transfer = Mathf.Min(transfer, Mathf.Lerp(MinTransfer, MaxTransfer, Normalized - other.Normalized)) * deltaTime;

                // Apply the values to the ports. These are automatically cleared
                // each time so no infinite fluid is possible.
                _consume.Value = transfer;
                other._refill.Value = transfer;
            }

            void DoTransferPit(CarPitStopParametersBase other)
            {
                // Similar to the other one but values are in the repair stuff.
                // This allows transfering into vanilla vehicles.
                var pit = other.GetCarPitStopParameters()[Type.ConvertToDVResource()];

                var transfer = MathHelper.MaxTransfer2ContainersPositiveOnly(
                    _capacity.Value, _amount.Value, pit.maxValue, pit.value);

                transfer = Mathf.Min(transfer, Mathf.Lerp(MinTransfer, MaxTransfer, Normalized - (pit.value / pit.maxValue))) * deltaTime;

                _consume.Value = transfer;
                other.UpdateCarPitStopParameter(Type.ConvertToDVResource(), transfer);
            }
        }
    }
}
