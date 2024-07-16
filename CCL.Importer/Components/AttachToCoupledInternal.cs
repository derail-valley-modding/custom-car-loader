using CCL.Types.Components;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class AttachToCoupledInternal : MonoBehaviour
    {
        private TrainCar _car = null!;
        private Coupler _coupledToFront = null!;
        private Coupler _coupledToRear = null!;
        private Transform _target = null!;
        private Vector3 _position = Vector3.zero;
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _scale = Vector3.one;
        private bool _coupleStateFront = false;
        private bool _coupleStateRear = false;

        public CouplingDirection Direction;
        public CouplingDirection OtherDirection = CouplingDirection.Rear;
        public ConnectionMode Mode;
        public bool HideWhenUncoupled = false;
        public string TransformPath = string.Empty;

        private void Start()
        {
            // Ensure we are on a train car.
            _car = TrainCar.Resolve(gameObject);

            if (!_car)
            {
                Debug.Log($"Could not find attached TrainCar! ({name})");
                Destroy(this);
                return;
            }

            // Grab the original position to reset.
            _position = transform.localPosition;
            _rotation = transform.localRotation;
            _scale = transform.localScale;
        }

        private void FixedUpdate()
        {
            // Why aren't events working 😔
            CheckForCoupling();
        }

        private void Update()
        {
            if (!_target) return;

            switch (Mode)
            {
                case ConnectionMode.Rigid:
                    transform.LookAt(_target);
                    break;
                case ConnectionMode.Attach:
                    transform.position = _target.position;
                    transform.rotation = _target.rotation;
                    break;
                case ConnectionMode.HalfMeet:
                    // WIP
                    break;
                default:
                    break;
            }
        }

        private void CheckForCoupling()
        {
            switch (Direction)
            {
                case CouplingDirection.Front:
                    FrontCheck();
                    break;
                case CouplingDirection.Rear:
                    RearCheck();
                    break;
                default:
                    break;
            }
        }

        private void FrontCheck()
        {
            // Coupled.
            if (!_coupledToFront && _car.frontCoupler.coupledTo)
            {
                Coupler other = _car.frontCoupler.coupledTo;

                switch (OtherDirection)
                {
                    case CouplingDirection.Front:
                        if (other == other.train.frontCoupler)
                        {
                            AcquireTarget(other.train, CouplingDirection.Front);
                        }
                        break;
                    case CouplingDirection.Rear:
                        if (other == other.train.rearCoupler)
                        {
                            AcquireTarget(other.train, CouplingDirection.Front);
                        }
                        break;
                    default:
                        return;
                }

                _coupledToFront = _car.frontCoupler.coupledTo;
                return;
            }

            // Uncoupled.
            if (_coupledToFront && !_car.frontCoupler.coupledTo && _coupleStateFront)
            {
                ResetState();
                _coupledToFront = _car.frontCoupler.coupledTo;
                _coupleStateFront = false;
            }
        }

        private void RearCheck()
        {
            // Coupled.
            if (!_coupledToRear && _car.rearCoupler.coupledTo)
            {
                Coupler other = _car.rearCoupler.coupledTo;

                switch (OtherDirection)
                {
                    case CouplingDirection.Front:
                        if (other == other.train.frontCoupler)
                        {
                            AcquireTarget(other.train, CouplingDirection.Rear);
                        }
                        break;
                    case CouplingDirection.Rear:
                        if (other == other.train.rearCoupler)
                        {
                            AcquireTarget(other.train, CouplingDirection.Rear);
                        }
                        break;
                    default:
                        return;
                }

                _coupledToRear = _car.rearCoupler.coupledTo;
                return;
            }

            // Uncoupled.
            if (_coupledToRear && !_car.rearCoupler.coupledTo && _coupleStateRear)
            {
                ResetState();
                _coupledToRear = _car.rearCoupler.coupledTo;
                _coupleStateRear = false;
            }
        }

        private void AcquireTarget(TrainCar other, CouplingDirection direction)
        {
            _target = other.transform.Find(TransformPath);

            if (_target)
            {
                switch (direction)
                {
                    case CouplingDirection.Front:
                        _coupleStateFront = true;
                        break;
                    case CouplingDirection.Rear:
                        _coupleStateRear = true;
                        break;
                    default:
                        break;
                }

                transform.localScale = _scale;
            }
            else
            {
                CCLPlugin.Error($"Failed to acquire target '{TransformPath}' for '{name}'");
            }
        }

        public void ResetState()
        {
            transform.localPosition = _position;
            transform.localRotation = _rotation;
            _target = null!;

            if (HideWhenUncoupled)
            {
                transform.localScale = Vector3.zero;
            }
        }

        public void Copy(AttachToCoupled source)
        {
            Direction = source.Direction;
            OtherDirection = source.OtherDirection;
            Mode = source.Mode;
            HideWhenUncoupled = source.HideWhenUncoupled;
            TransformPath = source.TransformPath;
        }
    }
}
