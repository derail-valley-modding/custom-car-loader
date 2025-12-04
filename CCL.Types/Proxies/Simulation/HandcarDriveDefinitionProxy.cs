using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class HandcarDriveDefinitionProxy : SimComponentDefinitionProxy, IH1Defaults
    {
        public float maxTorqueProduction = 1000f;

        [Tooltip("x-axis should be in [0-1] range")]
        public AnimationCurve positionDiffToTorque = null!;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "BAR_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ENGAGED_EXT_IN"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CURRENT_POSITION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ENGAGED_HANDLE_POSITION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "DIRECTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ACTING_AGAINST")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM", false),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO", false)
        };

        #region Defaults

        public void ApplyH1Defaults()
        {
            maxTorqueProduction = 600.0f;

            positionDiffToTorque = new AnimationCurve()
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = new[]
                {
                    new Keyframe
                    {
                        time = 0.0f,
                        value = 0.0f,
                        inTangent = -0.00014002957f,
                        outTangent = -0.00014002957f,
                        inWeight = 1 / 3f,
                        outWeight = 0.44464868f,
                    },
                    new Keyframe
                    {
                        time = 0.22639596f,
                        value = 0.20998888f,
                        inTangent = 2.9024727f,
                        outTangent = 2.9024727f,
                        inWeight = 0.19126332f,
                        outWeight = 0.20629348f,
                    },
                    new Keyframe
                    {
                        time = 0.382995f,
                        value = 1f,
                        inTangent = 0.0f,
                        outTangent = 0.0f,
                        inWeight = 0.10115377f,
                        outWeight = 0.108527f,
                    },
                    new Keyframe
                    {
                        time = 1f,
                        value = 1f,
                        inTangent = -0.0027138456f,
                        outTangent = -0.0027138456f,
                        inWeight = 0.13515477f,
                        outWeight = 1 / 3f,
                    }
                }
            };
        }

        #endregion
    }
}
