using CCL.Types;
using CCL.Types.Components;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class AttachToCoupledInternal : MonoBehaviour
    {
        private TrainCar _car = null!;
        private Transform _target = null!;
        private bool _coupledFront = false;
        private bool _coupledRear = false;
        private bool _flipDirection = false;
        private Vector3 _position = Vector3.zero;
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _scale = Vector3.one;

        public Transform MovedObject = null!;
        public string FrontConnectionTransformFront = string.Empty;
        public string FrontConnectionTransformRear = string.Empty;
        public string RearConnectionTransformFront = string.Empty;
        public string RearConnectionTransformRear = string.Empty;
        public ConnectionMode Mode;
        public bool HideWhenUncoupled = false;

        public bool ConnectFF => !string.IsNullOrEmpty(FrontConnectionTransformFront);
        public bool ConnectFR => !string.IsNullOrEmpty(FrontConnectionTransformRear);
        public bool ConnectRF => !string.IsNullOrEmpty(RearConnectionTransformFront);
        public bool ConnectRR => !string.IsNullOrEmpty(RearConnectionTransformRear);

        public bool HasFrontConnection => ConnectFF || ConnectFR;
        public bool HasRearConnection => ConnectRF || ConnectRR;

        private void Start()
        {
            // Ensure we are on a train car.
            _car = TrainCar.Resolve(gameObject);

            if (!_car)
            {
                Debug.Log($"Could not find attached TrainCar! AttachToCoupler {name}");
                Destroy(this);
                return;
            }

            if (HasFrontConnection)
            {
                if (!_car.frontCoupler)
                {
                    Debug.Log($"Could not find front coupler! AttachToCoupler {name}");
                    Destroy(this);
                    return;
                }

                _car.frontCoupler.Coupled += OnCoupleFront;
                _car.frontCoupler.Uncoupled += OnUncoupleFront;
            }

            if (HasRearConnection)
            {
                if (!_car.rearCoupler)
                {
                    Debug.Log($"Could not find rear coupler! AttachToCoupler {name}");
                    Destroy(this);
                    return;
                }

                _car.rearCoupler.Coupled += OnCoupleRear;
                _car.rearCoupler.Uncoupled += OnUncoupleRear;
            }

            // Grab the original position to reset.
            _position = MovedObject.localPosition;
            _rotation = MovedObject.localRotation;
            _scale = MovedObject.localScale;
        }

        private void OnCoupleFront(object sender, CoupleEventArgs e)
        {
            // Already coupled to something.
            if (_target)
            {
                return;
            }

            if (ConnectFF && e.otherCoupler == e.otherCoupler.train.frontCoupler)
            {
                _coupledFront = AcquireTarget(e.otherCoupler.train, FrontConnectionTransformFront);
                _flipDirection = GetFlipStatus(transform.forward, _target.forward);
                return;
            }
            else if (ConnectFR && e.otherCoupler == e.otherCoupler.train.rearCoupler)
            {
                _coupledFront = AcquireTarget(e.otherCoupler.train, FrontConnectionTransformRear);
                _flipDirection = GetFlipStatus(transform.forward, _target.forward);
                return;
            }
        }

        private void OnUncoupleFront(object sender, UncoupleEventArgs e)
        {
            // Don't disconnect if it was the other coupler that triggered the targetting.
            if (_coupledFront && _target)
            {
                ResetState();
            }
        }

        private void OnCoupleRear(object sender, CoupleEventArgs e)
        {
            if (_target)
            {
                return;
            }

            if (ConnectRF && e.otherCoupler == e.otherCoupler.train.frontCoupler)
            {
                _coupledFront = AcquireTarget(e.otherCoupler.train, RearConnectionTransformFront);
                _flipDirection = GetFlipStatus(transform.forward, _target.forward);
                return;
            }

            if (ConnectRR && e.otherCoupler == e.otherCoupler.train.rearCoupler)
            {
                _coupledFront = AcquireTarget(e.otherCoupler.train, RearConnectionTransformRear);
                _flipDirection = GetFlipStatus(transform.forward, _target.forward);
                return;
            }
        }

        private void OnUncoupleRear(object sender, UncoupleEventArgs e)
        {
            if (_coupledRear && _target)
            {
                ResetState();
            }
        }

        private bool AcquireTarget(TrainCar other, string path)
        {
            _target = other.transform.Find(path);

            if (_target)
            {
                MovedObject.localScale = _scale;
                return true;
            }
            else
            {
                CCLPlugin.Error($"Failed to acquire target '{path}' for '{name}'");
                return false;
            }
        }

        private static bool GetFlipStatus(Vector3 here, Vector3 other)
        {
            return Vector3.Dot(here, other) < 0;
        }

        public void ResetState()
        {
            MovedObject.localPosition = _position;
            MovedObject.localRotation = _rotation;
            _target = null!;
            _coupledFront = false;
            _coupledRear = false;

            if (HideWhenUncoupled)
            {
                MovedObject.localScale = Vector3.zero;
            }
        }

        private void Update()
        {
            if (!_target) return;

            switch (Mode)
            {
                case ConnectionMode.Rigid:
                    RigidUpdate();
                    break;
                case ConnectionMode.Attach:
                    AttachUpdate();
                    break;
                case ConnectionMode.HalfMeet:
                    HalfMeetUpdate();
                    break;
                default:
                    break;
            }
        }

        private void RigidUpdate()
        {
            MovedObject.LookAt(_target);
        }

        private void AttachUpdate()
        {
            MovedObject.position = _target.position;
            MovedObject.rotation = _target.rotation;
        }

        private void HalfMeetUpdate()
        {
            float scale = (transform.position - _target.position).magnitude * 0.3f;
            Vector3 forward = scale * transform.forward;
            Vector3 otherF = -scale * _target.forward;

            MovedObject.position = MathHelper.Bezier(
                transform.position, transform.position + forward, _target.position - otherF, _target.position, 0.5f);
            MovedObject.rotation = Quaternion.LookRotation(MathHelper.BezierDerivative1(
                transform.position, transform.position + forward, _target.position - otherF, _target.position, 0.5f));
        }

        public void Copy(AttachToCoupled source)
        {
            MovedObject = source.MovedObject;
            FrontConnectionTransformFront = source.FrontConnectionTransformFront;
            FrontConnectionTransformRear = source.FrontConnectionTransformRear;
            RearConnectionTransformFront = source.RearConnectionTransformFront;
            RearConnectionTransformRear = source.RearConnectionTransformRear;
            Mode = source.Mode;
            HideWhenUncoupled = source.HideWhenUncoupled;
        }
    }
}
