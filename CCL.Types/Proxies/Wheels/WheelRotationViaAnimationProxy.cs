﻿using UnityEngine;
using static CCL.Types.Proxies.Wheels.PoweredWheelRotationViaAnimationProxy;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Wheel Rotation Via Animation")]
    public class WheelRotationViaAnimationProxy : WheelRotationBaseProxy, ICustomSerialized
    {
        public AnimatorStartTimeOffsetPair[] animatorSetups = new AnimatorStartTimeOffsetPair[0];

        [HideInInspector]
        [SerializeField]
        private Animator[] _animators = new Animator[0];
        [HideInInspector]
        [SerializeField]
        private float[] _offsets = new float[0];

        public void OnValidate()
        {
            int length = animatorSetups.Length;
            _animators = new Animator[length];
            _offsets = new float[length];

            for (int i = 0; i < length; i++)
            {
                _animators[i] = animatorSetups[i].animator;
                _offsets[i] = animatorSetups[i].startTimeOffset;
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(_animators.Length, _offsets.Length);
            animatorSetups = new AnimatorStartTimeOffsetPair[length];

            for (int i = 0; i < length; i++)
            {
                animatorSetups[i] = new AnimatorStartTimeOffsetPair()
                {
                    animator = _animators[i],
                    startTimeOffset = _offsets[i]
                };
            }
        }
    }
}
