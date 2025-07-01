using CCL.Importer.Components.Controls;
using DV.CabControls;
using DV.Interaction;
using UnityEngine;

namespace CCL.Importer.Implementations.Controls
{
    internal abstract class PullableRopeBase : ControlImplBase, IScrollable
    {
        protected PullableRopeInternal Spec = null!;

        private Rigidbody _rb = null!;
        private ConfigurableJoint _joint = null!;
        private Coroutine? _resetting = null;
        private bool _initialised = false;
        private float _normalised = 0;

        protected override InteractionHandPoses GenericHandPoses => new(HandPose.PreGrab, HandPose.PreGrab, HandPose.Grab);

        public Vector3 Direction => transform.position - Spec.Origin.position;
        public float DistanceSqr => Direction.sqrMagnitude;
        public float Distance => Mathf.Sqrt(DistanceSqr);

        protected virtual void Awake()
        {
            Spec = GetComponent<PullableRopeInternal>();
            Initialize();
        }

        private void Initialize()
        {
            if (_initialised) return;

            transform.position = Spec.Origin.position;

            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.mass = 2.0f;
            _rb.drag = 0.2f;
            _rb.angularDrag = 2.0f;

            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();
            _joint.anchor = Vector3.zero;
            _joint.axis = Vector3.right;
            _joint.secondaryAxis = Vector3.forward;

            _joint.xMotion = ConfigurableJointMotion.Limited;
            _joint.yMotion = ConfigurableJointMotion.Limited;
            _joint.zMotion = ConfigurableJointMotion.Limited;
            _joint.angularYMotion = ConfigurableJointMotion.Locked;
            _joint.linearLimit = GetJointLimit(false);

            _rb.velocity =  _joint.connectedBody.velocity;

            _initialised = true;
        }

        private void Update()
        {
            if (!_initialised) return;

            var distance = Distance;

            if (distance <= Spec.RestLength + 0.01f)
            {
                _normalised = 0;
            }
            else if (distance >= Spec.MaxLength)
            {
                _normalised = 1;
            }
            else
            {
                _normalised = (Mathf.InverseLerp(Spec.RestLength, Spec.MaxLength, distance));
            }

            RequestValueUpdate(_normalised);

            if (IsGrabbed())
            {
                _rb.velocity = _joint.connectedBody.velocity;
            }
        }

        protected override void AcceptSetValue(float newValue)
        {
            Debug.Log("PullableRope doesn't support setting value", this);
        }

        protected override void FireGrabbed()
        {
            if (_resetting != null)
            {
                StopCoroutine(_resetting);
            }

            _joint.linearLimit = GetJointLimit(true);
            _rb.angularVelocity = Vector3.zero;

            base.FireGrabbed();
        }

        protected override void FireUngrabbed()
        {
            StartResetCoroutine();
            base.FireUngrabbed();
        }

        private SoftJointLimit GetJointLimit(bool grabbed)
        {
            return new SoftJointLimit()
            {
                limit = grabbed ? Spec.MaxLength : Spec.RestLength,
                bounciness = 0.1f,
                contactDistance = 0.0f
            };
        }

        private void StartResetCoroutine()
        {
            if (_resetting != null)
            {
                StopCoroutine(_resetting);
            }

            _resetting = StartCoroutine(ResetCoro());
        }

        private System.Collections.IEnumerator ResetCoro()
        {
            _rb.velocity = _joint.connectedBody.velocity;

            while (DistanceSqr >= Spec.RestLength * Spec.RestLength)
            {
                _rb.AddForce(Direction * -20.0f, ForceMode.Acceleration);
                _rb.AddForce(Physics.gravity * -0.2f, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }

            _joint.linearLimit = GetJointLimit(false);
        }

        public void Scroll(ScrollAction action, ScrollSource source = ScrollSource.Mouse)
        {
            if (action == ScrollAction.Release)
            {
                StartResetCoroutine();
                return;
            }

            if (_resetting != null)
            {
                StopCoroutine(_resetting);
            }

            _joint.linearLimit = GetJointLimit(true);
            _rb.MovePosition(transform.position + Direction.normalized * (action.IsPositive() ? 0.1f : -0.1f) * Time.deltaTime);
            _rb.velocity = Vector3.Lerp(_rb.velocity, _joint.connectedBody.velocity, 0.5f);
        }

        public bool IsAtEnd(ScrollAction action) => _normalised <= 0 || _normalised >= 1;
    }
}
