using System;
using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class DrivingAnimationSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.LocoComponents.DrivingAnimation";
        public override bool DestroyAfterCreation => true;

        [ProxyField]
        public float MaxWheelslipMultiplier = 8f;
        public float DefaultWheelRadius = 0.7f;
        public bool UseDefaultRadiusForAll = false;

        [Header("Transform Rotation")]
        [ProxyField]
        public Transform[] transformsToRotate;

        [ProxyField]
        public RotationAxis[] rotationAxes;

        [ProxyField]
        public float[] transformWheelRadii;

        [Header("Animations")]
        [ProxyField]
        public Animator[] animators;

        [ProxyField]
        [Range(0f, 1f)]
        public float[] startTimeOffsets;

        [ProxyField]
        public float[] animatorWheelRadii;

        public void OnValidate()
        {
            if (transformsToRotate != null)
            {
                if (transformsToRotate.Length != rotationAxes.Length)
                {
                    var newAxes = new RotationAxis[transformsToRotate.Length];
                    var newRadii = new float[transformsToRotate.Length];

                    for (int i = 0; i < transformsToRotate.Length; i++)
                    {
                        if (i < rotationAxes.Length) newAxes[i] = rotationAxes[i];
                        else newAxes[i] = RotationAxis.XAxis;

                        if (i < transformWheelRadii.Length) newRadii[i] = transformWheelRadii[i];
                        else newRadii[i] = DefaultWheelRadius;
                    }

                    rotationAxes = newAxes;
                    transformWheelRadii = newRadii;
                }

                if (UseDefaultRadiusForAll)
                {
                    Array.Fill(transformWheelRadii, DefaultWheelRadius);
                }
            }
            else
            {
                rotationAxes = null;
                transformWheelRadii = null;
            }

            if (animators != null)
            {
                if (animators.Length != startTimeOffsets.Length)
                {
                    var newOffsets = new float[animators.Length];
                    var newRadii = new float[animators.Length];

                    for (int i = 0; i < animators.Length; i++)
                    {
                        if (i < startTimeOffsets.Length) newOffsets[i] = startTimeOffsets[i];
                        else newOffsets[i] = 0;

                        if (i < animatorWheelRadii.Length) newRadii[i] = animatorWheelRadii[i];
                        else newRadii[i] = DefaultWheelRadius;
                    }

                    startTimeOffsets = newOffsets;
                    animatorWheelRadii = newRadii;
                }

                if (UseDefaultRadiusForAll)
                {
                    Array.Fill(animatorWheelRadii, DefaultWheelRadius);
                }
            }
            else
            {
                startTimeOffsets = null;
                animatorWheelRadii = null;
            }
        }
    }

    [Flags]
    public enum RotationAxis
    {
        XAxis = 1,
        YAxis = 2,
        ZAxis = 4
    }
}