using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Particles
{
    public class SteamSmokeParticlePortReaderProxy : AParticlePortReaderProxy, IHasPortIdFields, IS060Defaults, IS282Defaults
    {
        [PortId(DVPortValueType.STATE, false)]
        public string fireOnPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string chuffEventPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string isBoilerBrokenPortId;
        [PortId(DVPortValueType.PRESSURE, false)]
        public string exhaustPressurePortId;

        [Header("Smoke particles")]
        public GameObject smokeParticlesParent;
        public AnimationCurve smokeStartSpeedMultiplier;
        public AnimationCurve smokeEmissionRateMultiplier;
        public AnimationCurve smokeMaxParticlesMultiplier;

        [Header("Ember particles")]
        public GameObject emberParticlesParent;
        public AnimationCurve emberStartSpeedMultiplier;
        public AnimationCurve emberEmissionRateMultiplier;
        public AnimationCurve emberMaxParticlesMultiplier;

        [RenderMethodButtons]
        [MethodButton(nameof(ApplyS060Defaults), "Apply S060 Defaults")]
        [MethodButton(nameof(ApplyS282Defaults), "Apply S282 Defaults")]
        public bool buttonRender;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(fireOnPortId), fireOnPortId),
            new PortIdField(this, nameof(chuffEventPortId), chuffEventPortId),
            new PortIdField(this, nameof(isBoilerBrokenPortId), isBoilerBrokenPortId),
            new PortIdField(this, nameof(exhaustPressurePortId), exhaustPressurePortId)
        };

        public void ApplyS060Defaults()
        {
            if (!string.IsNullOrEmpty(fireOnPortId))
            {
                fireOnPortId = ".FIRE_ON";
            }
            if (!string.IsNullOrEmpty(chuffEventPortId))
            {
                chuffEventPortId = ".CHUFF_EVENT";
            }
            if (!string.IsNullOrEmpty(isBoilerBrokenPortId))
            {
                isBoilerBrokenPortId = ".IS_BROKEN";
            }
            if (!string.IsNullOrEmpty(exhaustPressurePortId))
            {
                exhaustPressurePortId = ".EXHAUST_PRESSURE";
            }
        }

        public void ApplyS282Defaults()
        {
            if (!string.IsNullOrEmpty(fireOnPortId))
            {
                fireOnPortId = ".FIRE_ON";
            }
            if (!string.IsNullOrEmpty(chuffEventPortId))
            {
                chuffEventPortId = ".CHUFF_EVENT";
            }
            if (!string.IsNullOrEmpty(isBoilerBrokenPortId))
            {
                isBoilerBrokenPortId = ".IS_BROKEN";
            }
            if (!string.IsNullOrEmpty(exhaustPressurePortId))
            {
                exhaustPressurePortId = ".EXHAUST_PRESSURE";
            }

            smokeStartSpeedMultiplier = new AnimationCurve(
                new Keyframe(0, 1, 2.6479797f, 2.6479797f, 0, 0.041025642f),
                new Keyframe(1, 2, 0.07901208f, 0.07901208f, 0.07051283f, 0));
            smokeEmissionRateMultiplier = new AnimationCurve(
                new Keyframe(0, 1, 49.091328f, 49.091328f, 0, 0.0474359f),
                new Keyframe(1, 20, 2.7892754f, 2.7892754f, 0.09487182f, 0));
            smokeMaxParticlesMultiplier = new AnimationCurve(
                new Keyframe(0, 1, 1, 1, 0, 1 / 3f),
                new Keyframe(1, 2, 1, 1, 1 / 3f, 0));

            emberStartSpeedMultiplier = new AnimationCurve(
                new Keyframe(0, 0.5f, 4.0258913f, 4.0258913f, 0, 0.043589745f),
                new Keyframe(1, 2, 0.046558026f, 0.046558026f, 0.089743614f, 0));
            emberEmissionRateMultiplier = new AnimationCurve(
                new Keyframe(0, 0, 0, 0, 0, 1 / 3f),
                new Keyframe(0.5f, 0, 0, 0, 0.13589746f, 1 / 3f),
                new Keyframe(1, 1, 2, 2, 1 / 3f, 0));
            emberMaxParticlesMultiplier = new AnimationCurve(
                new Keyframe(0, 1, 0, 0, 0, 1 / 3f),
                new Keyframe(1, 1, 0, 0, 1 / 3f, 0));
        }
    }
}
