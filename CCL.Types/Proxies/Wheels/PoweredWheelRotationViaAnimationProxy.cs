using System;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class PoweredWheelRotationViaAnimationProxy : PoweredWheelRotationBaseProxy
    {
        [Serializable]
        public class AnimatorStartTimeOffsetPair
        {
            public Animator animator;

            [Range(0f, 1f)]
            public float startTimeOffset;
        }

        public AnimatorStartTimeOffsetPair[] animatorSetups;
    }
}
