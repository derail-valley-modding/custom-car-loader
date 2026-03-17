using CCL.Importer.Components.Controls;
using CCL.Types;
using DV.CabControls;
using DV.HUD;
using DV.Interaction;
using UnityEngine;

namespace CCL.Importer.Implementations.Controls
{
    internal abstract class ScrewBase : ControlImplBase, IScrollable
    {
        private static readonly Vector3 AXIS = Vector3.back;

        private bool _initialised = false;
        private float _scroll;
        private float _prevAngle;
        private float _currentRev;
        private Vector3 _initialPosition;
        private Rigidbody _rb = null!;
        private ControlNameHolderBase? _names;
        private HingeJointAngleFix? _hjaf;
        private LeverAudio? _audio;

        protected ScrewInternal Spec = null!;
        protected HingeJoint Joint = null!;
        protected float DiffThreshold = 1;

        protected override InteractionHandPoses GenericHandPoses { get; } = new InteractionHandPoses(HandPose.PreGrab, HandPose.PreGrab, HandPose.Grab);
        protected float CurrentAngle
        {
            get
            {
                var angle = transform.localRotation.eulerAngles.z;

                if (angle > 180) angle -= 360;
                if (angle < -180) angle += 360;

                return angle;
            }
        }

        protected virtual void Awake()
        {
            Spec = GetComponent<ScrewInternal>();

            // Ensure local position and rotation are 0.
            var _reparent = new GameObject("Screw Parent").transform;
            _reparent.parent = transform.parent;
            _reparent.CopyLocalFrom(transform);
            transform.parent = _reparent;
            transform.ResetLocal();

            _scroll = Spec.ScrollWheelHoverScroll;
            _names = GetComponent<ControlNameHolderBase>();

            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.mass = Spec.Mass;
            _rb.angularDrag = Spec.AngularDrag;
            _rb.useGravity = false;
            RefreshCoM();

            Joint = gameObject.AddComponent<HingeJoint>();
            Joint.useSpring = false;
            ResetParent(true);
            UnlockLimits();

            var limits = Joint.limits;
            limits.bounciness = Spec.Bounciness;
            limits.bounceMinVelocity = Spec.BounceMinVelocity;
            Joint.limits = limits;
            Joint.axis = AXIS;

            _initialPosition = Joint.connectedAnchor;
            Joint.autoConfigureConnectedAnchor = false;

            if (Spec.Drag != null && Spec.LimitHit != null)
            {
                // This HJAF is needed for audio...
                _hjaf = gameObject.AddComponent<HingeJointAngleFix>();
                _hjaf.invertPercentage = Spec.InvertDirection;

                _audio = gameObject.AddComponent<LeverAudio>();
                _audio.dragClip = Spec.Drag;
                _audio.hitClip = Spec.LimitHit;
                _audio.hitToleranceAngle = Spec.HitTolerance;
            }

            _initialised = true;
        }

        private void RefreshCoM()
        {
            if (Spec.ZeroCenterOfMass && !IsGrabbed())
            {
                _rb.centerOfMass = Vector3.zero;
            }
            else
            {
                _rb.ResetCenterOfMass();
            }
        }

        private void SetMinLimit()
        {
            var limits = Joint.limits;
            limits.min = -180;
            limits.max = 0;
            Joint.limits = limits;
            Joint.useLimits = true;

            if (_audio != null)
            {
                _audio.minAngle = -180;
                _audio.maxAngle = 0;
            }
        }

        private void SetMaxLimit()
        {
            var limits = Joint.limits;
            limits.min = 0;
            limits.max = 180;
            Joint.limits = limits;
            Joint.useLimits = true;

            if (_audio != null)
            {
                _audio.minAngle = 0;
                _audio.maxAngle = 180;
            }
        }

        private void UnlockLimits()
        {
            Joint.useLimits = false;

            if (_audio != null)
            {
                _audio.minAngle = -270;
                _audio.maxAngle = 270;
            }
        }

        protected override void FireGrabbed()
        {
            base.FireGrabbed();
            RefreshCoM();
        }

        protected override void FireUngrabbed()
        {
            base.FireUngrabbed();
            RefreshCoM();
        }

        public override void ResetParent(bool forced = false)
        {
            if (!_initialised && !forced) return;
            Joint.connectedBody = transform.parent.GetComponentInParentIncludingInactive<Rigidbody>();
        }

        private void CheckAngle(float angle)
        {
            // Set joint limits to prevent rotating more than the allowed number of revolutions.
            if (_currentRev == 0 && angle < 90)
            {
                SetMinLimit();
            }
            else if (_currentRev == Spec.Revolutions && angle > -90)
            {
                SetMaxLimit();
            }
            else
            {
                UnlockLimits();
            }
        }

        private float GetNormalised(float angle)
        {
            var normal = ((angle / 360.0f) + _currentRev) / Spec.Revolutions;
            return Spec.InvertDirection ? 1 - normal : normal;
        }

        private void SetTravel(float normal)
        {
            var pos = AXIS * Spec.Travel * -normal;
            Joint.connectedAnchor = _initialPosition + pos;
        }

        private void FixedUpdate()
        {
            if (!_initialised) return;

            var angle = CurrentAngle;
            var diff = angle - _prevAngle;

            if (Mathf.Abs(diff) < DiffThreshold) return;

            // If the difference is too large, it likely changed the current revolution.
            switch (diff)
            {
                case > 180:
                    _currentRev--;
                    break;
                case < -180:
                    _currentRev++;
                    break;
                default:
                    break;
            }

            CheckAngle(angle);
            var normal = GetNormalised(angle);
            RequestValueUpdate(normal);
            SetTravel(normal);

            //Debug.Log($"Check: {angle:F2} - {_currentRev} - {Joint.spring.targetPosition}");

            _prevAngle = angle;
        }

        protected override void AcceptSetValue(float value)
        {
            if (!_initialised || IsGrabbed()) return;

            if (Spec.InvertDirection)
            {
                value = 1 - value;
            }

            var angle = value * Spec.Revolutions * 360;
            _currentRev = 0;

            while (angle > 180)
            {
                angle -= 360;
                _currentRev++;
            }

            CheckAngle(angle);

            transform.localRotation = Quaternion.Euler(0, 0, angle);
            _prevAngle = angle;

            var spring = Joint.spring;
            spring.targetPosition = -angle;
            Joint.spring = spring;

            SetTravel(value);

            //Debug.Log($"Set: {angle:F2} - {_currentRev} - {Joint.spring.targetPosition}");
        }

        public override void BlockControl(bool setBlock)
        {
            InteractionAllowed = !setBlock;
        }

        public override (string value, string unit) GetCurrentPositionName()
        {
            if (_names != null) return _names.GetName();

            return base.GetCurrentPositionName();
        }

        private void SetSpringStrength(float strength, float damper)
        {
            var spring = Joint.spring;
            spring.spring = strength;
            spring.damper = damper;
            Joint.spring = spring;
        }

        public void Scroll(ScrollAction action, ScrollSource source = ScrollSource.Mouse)
        {
            if (action == ScrollAction.Release)
            {
                SetSpringStrength(Spec.Spring, Spec.Damper);
                AcceptSetValue(GetNormalised(CurrentAngle));
                return;
            }

            LastSetValueSource = SetValueSource.Default;

            if (!InteractionAllowed) return;

            SetSpringStrength(0, Spec.Damper * 0.1f);
            _rb.AddRelativeTorque(AXIS * _scroll * -action.IsPositive().ToDir(), ForceMode.Impulse);
        }

        public bool IsAtEnd(ScrollAction action)
        {
            if (!Joint.useLimits) return false;
            return Mathf.Approximately(CurrentAngle, 0);
        }
    }
}
