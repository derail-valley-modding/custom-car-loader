using System;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class KeepCoupledInteriorLoadedInternal : MonoBehaviour
    {
        public bool KeepFrontCoupledLoaded = true;
        public bool KeepRearCoupledLoaded = false;

        private TrainCar _car = null!;

        private TrainCar? _coupledF = null;
        private TrainCar? _coupledR = null;

        private void Start()
        {
            _car = TrainCar.Resolve(gameObject);

            if (_car == null)
            {
                Debug.LogError("Could not find TrainCar for KeepCoupledInteriorLoaded!");
                Destroy(this);
                return;
            }
            
            RegisterCouplerEvents(_car.frontCoupler, true);
            RegisterCouplerEvents(_car.rearCoupler, true);
            
            _coupledF = _car.frontCoupler?.coupledTo?.train;
            _coupledR = _car.rearCoupler?.coupledTo?.train;

            PlayerManager.CarChanged += CarChanged;
        }

        private void OnDestroy()
        {
            PlayerManager.CarChanged -= CarChanged;

            RegisterCouplerEvents(_car.frontCoupler, false);
            RegisterCouplerEvents(_car.rearCoupler, false);
        }

        private void RegisterCouplerEvents(Coupler coupler, bool register)
        {
            if (coupler == null) return;

            if (register)
            {
                coupler.Coupled += Coupled;
                coupler.Uncoupled += Uncoupled;
            }
            else
            {
                coupler.Coupled -= Coupled;
                coupler.Uncoupled -= Uncoupled;
            }
        }

        private void Coupled(object sender, CoupleEventArgs e)
        {
            if (e.thisCoupler.isFrontCoupler)
            {
                _coupledF = e.otherCoupler.train;
            }
            else
            {
                _coupledR = e.otherCoupler.train;
            }

            // On couple just call this, locking something already locked does nothing,
            // so only the new coupling gets checked.
            LockLOD();
        }

        private void Uncoupled(object sender, UncoupleEventArgs e)
        {
            // Unlocked the LOD only in the uncoupled side.
            if (e.thisCoupler.isFrontCoupler)
            {
                _coupledF = null;
                UnlockLOD(true, false);
            }
            else
            {
                _coupledR = null;
                UnlockLOD(false, true);
            }
        }

        private void CarChanged(TrainCar car)
        {
            if (car == _car)
            {
                LockLOD();
            }
            else
            {
                UnlockLOD(true, true);
            }
        }

        private void LockLOD()
        {
            if (_coupledF != null && KeepFrontCoupledLoaded)
            {
                _coupledF.physicsLod.LockHighestLOD();
            }

            if (_coupledR != null && KeepRearCoupledLoaded)
            {
                _coupledR.physicsLod.LockHighestLOD();
            }
        }

        private void UnlockLOD(bool front, bool rear)
        {
            if (_coupledF != null && front)
            {
                _coupledF.physicsLod.UnlockHighestLOD();
            }

            if (_coupledR != null && rear)
            {
                _coupledR.physicsLod.UnlockHighestLOD();
            }
        }
    }
}
