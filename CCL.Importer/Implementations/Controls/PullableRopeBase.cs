using CCL.Importer.Components.Controls;
using DV.CabControls;
using DV.Interaction;
using DV.VRTK_Extensions;
using System;
using UnityEngine;
using VRTK;

namespace CCL.Importer.Implementations.Controls
{
    internal abstract class PullableRopeBase : ControlImplBase, IScrollable
    {
        private class RopeAudio : MonoBehaviour
        {
            public PullableRopeBase Rope = null!;

            private AudioSource _dragSource = null!;
            private AudioSource _limitSource = null!;
            private AudioSource? _notchSource = null!;
            private bool _muted = true;
            private bool _justHit = false;
            private float _prevTime = 0;

            private void Start()
            {
                _dragSource = NAudio.CreateSource(transform, Rope.Spec.Drag, 1, 1, true).source;
                _limitSource = NAudio.CreateSource(transform, Rope.Spec.LimitHit, 1, 1, false).source;

                if (Rope.Spec.Notch != null)
                {
                    _notchSource = NAudio.CreateSource(transform, Rope.Spec.Notch, 1, 1, false).source;
                    Rope.AudioNotchChanged += PlayNotch;
                }

                StartCoroutine(Unmute(0.5f));
            }

            private System.Collections.IEnumerator Unmute(float timeout)
            {
                yield return WaitFor.Seconds(timeout);

                _prevTime = Time.fixedTime;
                _muted = false;
            }

            private void Update()
            {
                var time = Time.fixedTime;

                if (time == 0 || time == _prevTime) return;

                if (_muted)
                {
                    _dragSource.Stop();
                    _limitSource.Stop();
                    return;
                }

                _prevTime = time;

                // Volume based on how fast it is being pulled.
                var volume = Rope.NormalDelta * 30.0f / time;

                if (Rope.NormalDelta == 0 && _dragSource.isPlaying)
                {
                    _dragSource.Stop();
                }
                else
                {
                    if (!_dragSource.isPlaying)
                    {
                        _dragSource.PlayRandomTime();
                        _dragSource.volume = volume;
                    }
                    else
                    {
                        _dragSource.volume = volume;
                    }
                }

                // If the value is at max, check if it was just reached.
                if (Rope._normalised == 1)
                {
                    if (!_justHit)
                    {
                        // Play the limit clip and vibrate if needed.
                        _limitSource.volume = volume * 3f;
                        _limitSource.Play();

                        if (Rope.Spec.LimitVibration)
                        {
                            Vibrate(volume);
                        }

                        _justHit = true;
                    }
                }
                else
                {
                    _justHit = false;
                }
            }

            private void Vibrate(float strength)
            {
                var comp = gameObject.GetComponentInParent<VRTK_InteractableObject>();
                if (comp == null) return;

                HapticUtils.DoHapticPulse(VRTK_ControllerReference.GetControllerReference(comp.GetGrabbingObject()), strength);
            }

            private void PlayNotch(int _)
            {
                if (_notchSource != null)
                {
                    _notchSource.Play();
                }
            }
        }

        private const float ScrollMultiplier = 0.0001f;

        public Action<int>? AudioNotchChanged;

        protected PullableRopeInternal Spec = null!;

        private Rigidbody _rb = null!;
        private ConfigurableJoint _joint = null!;
        private RopeAudio _audio = null!;
        private Coroutine? _resetting = null;
        private bool _initialised = false;
        private float _normalised = 0;
        private float _prevNorm = 0;
        private int _notch = 0;

        protected override InteractionHandPoses GenericHandPoses => new(HandPose.PreGrab, HandPose.PreGrab, HandPose.Grab);

        public Vector3 Direction => transform.position - Spec.Origin.position;
        public float DistanceSqr => Direction.sqrMagnitude;
        public float Distance => Mathf.Sqrt(DistanceSqr);

        private float NormalDelta => _normalised - _prevNorm;

        protected virtual void Awake()
        {
            Spec = GetComponent<PullableRopeInternal>();
            Initialize();
        }

        private void Initialize()
        {
            if (_initialised) return;

            transform.position = Spec.Origin.position;

            // Add a rigidbody so the "rope" hangs.
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.mass = 2.0f;
            _rb.drag = 0.2f;
            _rb.angularDrag = 2.0f;

            // Add the joint that will enforce limits.
            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();
            _joint.anchor = Vector3.zero;
            _joint.axis = Vector3.right;
            _joint.secondaryAxis = Vector3.forward;

            // Limit to a sphere around the origin.
            _joint.xMotion = ConfigurableJointMotion.Limited;
            _joint.yMotion = ConfigurableJointMotion.Limited;
            _joint.zMotion = ConfigurableJointMotion.Limited;
            _joint.linearLimit = GetJointLimit(false);

            // Make the starting velocity the same as the body it is parented to,
            // to prevent weird stretching.
            _rb.velocity =  _joint.connectedBody.velocity;

            _audio = gameObject.AddComponent<RopeAudio>();
            _audio.Rope = this;

            ValueChanged += OnValueChanged;

            _initialised = true;
        }

        private void Update()
        {
            if (!_initialised) return;

            var distance = Distance;
            _prevNorm = _normalised;

            // Set the value of the control to the normalised value between min and max lengths.
            // Min being the rest length + tolerance.
            if (distance <= Spec.MinLength)
            {
                _normalised = 0;
            }
            else if (distance >= Spec.MaxLength)
            {
                _normalised = 1;
            }
            else
            {
                _normalised = Mathf.InverseLerp(Spec.MinLength, Spec.MaxLength, distance);
            }

            RequestValueUpdate(_normalised);

            // Prevent it from trying to move away.
            if (IsGrabbed())
            {
                _rb.velocity = _joint.connectedBody.velocity;
            }
        }

        private void OnValueChanged(ValueChangedEventArgs args)
        {
            if (Spec.AudioNotches > 1)
            {
                var notch = Mathf.RoundToInt(args.newValue * (Spec.AudioNotches - 1));

                if (notch != _notch && notch > 0)
                {
                    AudioNotchChanged?.Invoke(notch);
                }

                _notch = notch;
            }
        }

        private void OnDisable()
        {
            // Don't let it get stuck active.
            StopResetCoroutine();
            RequestValueUpdate(0);
            transform.position = Spec.Origin.position;
        }

        protected override void AcceptSetValue(float newValue)
        {
            Debug.Log("PullableRope doesn't support setting value", this);
        }

        protected override void FireGrabbed()
        {
            StopResetCoroutine();

            // Increase the joint limit to max length.
            _joint.linearLimit = GetJointLimit(true);
            _rb.angularVelocity = Vector3.zero;

            base.FireGrabbed();
        }

        protected override void FireUngrabbed()
        {
            StartResetCoroutine();
            base.FireUngrabbed();
        }

        protected virtual SoftJointLimit GetJointLimit(bool grabbed) => new()
        {
            limit = grabbed ? Spec.MaxLength : Spec.RestLength,
            bounciness = 0.1f,
            contactDistance = 0.0f
        };

        private void StopResetCoroutine()
        {
            if (_resetting != null)
            {
                StopCoroutine(_resetting);
                _resetting = null;
            }
        }

        private void StartResetCoroutine()
        {
            StopResetCoroutine();
            _resetting = StartCoroutine(ResetCoro());
        }

        private System.Collections.IEnumerator ResetCoro()
        {
            // "Stop" moving.
            _rb.velocity = _joint.connectedBody.velocity;

            // While it is longer than rest length...
            while (DistanceSqr >= Spec.RestLength * Spec.RestLength)
            {
                yield return new WaitForFixedUpdate();

                // Apply a spring force towards the origin.
                // Also reduce gravity by a bit to help.
                _rb.AddForce(Direction * -20.0f, ForceMode.Acceleration);
                _rb.AddForce(Physics.gravity * -0.2f, ForceMode.Acceleration);
            }

            // Once under rest length, reset limit so it doesn't reactivate.
            _joint.linearLimit = GetJointLimit(false);
            _rb.velocity = Vector3.LerpUnclamped(_rb.velocity, _joint.connectedBody.velocity, 0.5f);
        }

        public void Scroll(ScrollAction action, ScrollSource source = ScrollSource.Mouse)
        {
            // Same procedure as ungrab.
            if (action == ScrollAction.Release)
            {
                StartResetCoroutine();
                return;
            }

            StopResetCoroutine();

            // Set joint limit to max length.
            // Move the rigidbody away from the origin when scrolling is positive.
            // Set the velocity to the connected velocity so it "stops" moving.
            _joint.linearLimit = GetJointLimit(true);
            _rb.MovePosition(transform.position + Direction.normalized * (action.IsPositive() ? ScrollMultiplier : -ScrollMultiplier) * Time.deltaTime);
            _rb.velocity = _joint.connectedBody.velocity;
        }

        public bool IsAtEnd(ScrollAction action) => _normalised <= 0 || _normalised >= 1;
    }
}
