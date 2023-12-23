using DV.Wheels;
using UnityEngine;

namespace CCL.Importer.Types
{
    public class WheelRotationViaAnimation : WheelRotationBase
    {
        public PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair[] animatorSetups;

        private const string SPEED = "SpeedMultiplier";

        private static readonly int SPEED_ID = Animator.StringToHash("SpeedMultiplier");

        protected override void Awake()
        {
            base.Awake();

            foreach (var animatorStartTimeOffsetPair in animatorSetups)
            {
                Animator animator = animatorStartTimeOffsetPair.animator;
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, animatorStartTimeOffsetPair.startTimeOffset);
            }
        }

        private void Update()
        {
            float rps = GetRPS();

            for (int i = 0; i < animatorSetups.Length; i++)
            {
                animatorSetups[i].animator.SetFloat(SPEED_ID, rps);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < animatorSetups.Length; i++)
            {
                animatorSetups[i].animator.SetFloat(SPEED_ID, 0f);
            }
        }
    }
}
