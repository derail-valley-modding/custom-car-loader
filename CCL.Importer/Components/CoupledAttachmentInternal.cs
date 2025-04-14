using CCL.Types;
using CCL.Types.Components;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class CoupledAttachmentInternal : MonoBehaviour
    {
        private TrainCar _car = null!;
        private Coupler? _coupled = null;
        private Transform _target = null!;
        private Vector3 _position = Vector3.zero;
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _scale = Vector3.one;

        public Transform MovedObject = null!;
        public string ConnectionTag = string.Empty;
        public CouplerDirection Direction;
        public CouplerDirection OtherDirection;
        public ConnectionMode Mode;
        public bool HideWhenUncoupled = false;

        public bool HasFrontConnection => Direction == CouplerDirection.Front;
        public bool HasRearConnection => Direction == CouplerDirection.Rear;
        public bool ToOtherFront => OtherDirection == CouplerDirection.Front;
        public bool ToOtherRear => OtherDirection == CouplerDirection.Rear;

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

            var coupler = HasFrontConnection ? _car.frontCoupler : _car.rearCoupler;

            if (coupler == null)
            {
                Debug.LogError($"Could not find coupler! CoupledAttachment {name}", this);
                Destroy(this);
                return;
            }

            coupler.Coupled += OnCouple;
            coupler.Uncoupled += OnUncouple;

            // Grab the original position to reset.
            _position = MovedObject.localPosition;
            _rotation = MovedObject.localRotation;
            _scale = MovedObject.localScale;

            if (HideWhenUncoupled)
            {
                MovedObject.localScale = Vector3.zero;
            }

            // If it is already coupled when this component is activated...
            if (coupler.coupledTo != null)
            {
                SuccessfulCouple(coupler.coupledTo);
            }
        }

        private void OnCouple(object sender, CoupleEventArgs e)
        {
            // Already coupled to something.
            if (_coupled)
            {
                return;
            }

            if ((ToOtherFront && e.otherCoupler == e.otherCoupler.train.frontCoupler) ||
                (ToOtherRear && e.otherCoupler == e.otherCoupler.train.rearCoupler))
            {
                SuccessfulCouple(e.otherCoupler);
            }
        }

        private void SuccessfulCouple(Coupler other)
        {
            _coupled = other;
            TryAcquireTarget(_coupled.train, ConnectionTag);
        }

        private void OnUncouple(object sender, UncoupleEventArgs e)
        {
            // Don't disconnect if it was the other coupler that triggered the targetting.
            if (_coupled == e.otherCoupler)
            {
                ResetState();
            }
        }

        private bool TryAcquireTarget(TrainCar other, string tag)
        {
            if (other.TryGetComponent(out CoupledAttachmentController controller) &&
                controller.TryGetTag(tag, out var comp))
            {
                _target = comp.transform;
                MovedObject.localScale = _scale;
                return true;
            }

            CCLPlugin.Error($"Failed to acquire target '{tag}' for '{name}'");
            return false;
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
            _coupled = null;

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
    }
}
