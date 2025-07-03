using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    [AddComponentMenu("CCL/Proxies/VFX/Steam Smoke Particle Port Reader Proxy")]
    public class SteamSmokeParticlePortReaderProxy : AParticlePortReaderProxy, IHasPortIdFields, IS060Defaults, IS282Defaults
    {
        [PortId(DVPortValueType.STATE, false)]
        public string fireOnPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string chuffEventPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string isBoilerBrokenPortId = string.Empty;
        [PortId(DVPortValueType.PRESSURE, false)]
        public string exhaustPressurePortId = string.Empty;

        [Header("Smoke particles")]
        public GameObject smokeParticlesParent = null!;
        public AnimationCurve smokeStartSpeedMultiplier = null!;
        public AnimationCurve smokeEmissionRateMultiplier = null!;
        public AnimationCurve smokeMaxParticlesMultiplier = null!;

        [Header("Ember particles")]
        public GameObject emberParticlesParent = null!;
        public AnimationCurve emberStartSpeedMultiplier = null!;
        public AnimationCurve emberEmissionRateMultiplier = null!;
        public AnimationCurve emberMaxParticlesMultiplier = null!;

        [RenderMethodButtons]
        [MethodButton(nameof(ApplyS060Defaults), "Apply S060 Defaults")]
        [MethodButton(nameof(ApplyS282Defaults), "Apply S282 Defaults")]
        public bool buttonRender;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(fireOnPortId), fireOnPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(chuffEventPortId), chuffEventPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(isBoilerBrokenPortId), isBoilerBrokenPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(exhaustPressurePortId), exhaustPressurePortId, DVPortValueType.PRESSURE)
        };

        public void ApplyS060Defaults()
        {
            ApplyS282Defaults();
        }

        public void ApplyS282Defaults()
        {
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
                new Keyframe(0, 1),
                new Keyframe(1, 1));
        }
    }
}
