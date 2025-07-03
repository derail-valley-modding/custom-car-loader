using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Smooth Transmission Definition Proxy")]
    public class SmoothTransmissionDefinitionProxy : SimComponentDefinitionProxy, IDM1UDefaults
    {
        public float transitionTime = 1f;
        public float[] gearRatios = new float[0];
        public float transmissionEfficiency = 1f;
        public AnimationCurve gearChangeEaseCurve = null!;

        [Header("Damage")]
        public float powerShiftRpmThreshold = 400f;
        public float powerShiftDamage = 10f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.IN, DVPortValueType.TORQUE, "TORQUE_IN"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM_OF_GEARS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "GEAR_RATIO"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "GEAR_CHANGE_IN_PROGRESS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_DAMAGE"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "GEAR"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "RETARDER"),
            new PortReferenceDefinition(DVPortValueType.RPM, "ENGINE_RPM"),
        };

        [MethodButton(nameof(ApplyDM3BoxADefaults), "Apply DM3 Gearbox A Defaults")]
        [MethodButton(nameof(ApplyDM3BoxBDefaults), "Apply DM3 Gearbox B Defaults")]
        [RenderMethodButtons]
        public bool renderButtons;

        #region Defaults

        public void ApplyDM3BoxADefaults()
        {
            ApplyDM3Defaults(new float[] { 5, 3, 2 });
        }

        public void ApplyDM3BoxBDefaults()
        {
            ApplyDM3Defaults(new float[] { 4, 3, 1.5f });
        }

        private void ApplyDM3Defaults(float[] ratios)
        {
            transitionTime = 1;
            transmissionEfficiency = 1;
            powerShiftRpmThreshold = 400;
            powerShiftDamage = 10;

            gearRatios = ratios;

            gearChangeEaseCurve = new AnimationCurve
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = new Keyframe[]
                {
                    new Keyframe(0, 0, 2, 2),
                    new Keyframe(0.528225541f, 0.949414432f, 0.401515067f, 0.401515067f, 0.333333343f, 0.1374266f),
                    new Keyframe(1, 1, 0, 0, 0.232664958f, 0),
                }
            };
        }

        public void ApplyDM1UDefaults()
        {
            transitionTime = 1;
            gearRatios = new[]
            {
                0f,
                15f,
                10.6f,
                7.8f,
                5.8f,
                4.4f,
                3.4f
            };
            transmissionEfficiency = 1;
            gearChangeEaseCurve = new AnimationCurve
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = new Keyframe[]
                {
                    new Keyframe(0, 0, 2, 2),
                    new Keyframe(0.528225541f, 0.949414432f, 0.401515067f, 0.401515067f, 0.333333343f, 0.1374266f),
                    new Keyframe(1, 1, 0, 0, 0.232664958f, 0),
                }
            };

            powerShiftRpmThreshold = 500;
            powerShiftDamage = 10;
        }

        #endregion
    }
}