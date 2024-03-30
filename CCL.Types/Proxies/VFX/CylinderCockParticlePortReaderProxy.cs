using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    public class CylinderCockParticlePortReaderProxy : AParticlePortReaderProxy, IHasPortIdFields, ICustomSerialized, IS060Defaults, IS282Defaults
    {
        public static AnimationCurve Curve0 => new AnimationCurve(
            new Keyframe(0.00f, 1),
            new Keyframe(0.24f, 1),
            new Keyframe(0.25f, 0),
            new Keyframe(0.75f, 0),
            new Keyframe(0.76f, 1),
            new Keyframe(1.00f, 1));
        public static AnimationCurve Curve90 => new AnimationCurve(
            new Keyframe(0.00f, 1),
            new Keyframe(0.49f, 1),
            new Keyframe(0.50f, 0),
            new Keyframe(1.00f, 0));
        public static AnimationCurve Curve180 => new AnimationCurve(
            new Keyframe(0.00f, 0),
            new Keyframe(0.25f, 0),
            new Keyframe(0.26f, 1),
            new Keyframe(0.74f, 1),
            new Keyframe(0.75f, 0),
            new Keyframe(1.00f, 0));
        public static AnimationCurve Curve270 => new AnimationCurve(
            new Keyframe(0.00f, 0),
            new Keyframe(0.50f, 0),
            new Keyframe(0.51f, 1),
            new Keyframe(1.00f, 1));

        [PortId(DVPortValueType.STATE, false)]
        public string crankRotationPortId;
        [PortId(DVPortValueType.CONTROL, false)]
        public string reverserPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string cylindersSteamInjectionPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string cylinderCockFlowNormalizedPortId;
        [PortId(DVPortValueType.GENERIC, false)]
        public string forwardSpeedPortId;

        public CylinderSetup[] cylinderSetups = new CylinderSetup[0];
        public float startSpeedMultiplierMin;
        public float startSpeedMultiplierMax;
        public float startSizeMultiplierMin;
        public float startSizeMultiplierMax;
        public float emissionRateMultiplierMin;
        public float emissionRateMultiplierMax;
        public float emissionRateMaxSpeedKmh = 60f;

        private GameObject[] _frontParents = new GameObject[0];
        private AnimationCurve[] _frontCurves = new AnimationCurve[0];
        private GameObject[] _rearParents = new GameObject[0];
        private AnimationCurve[] _rearCurves = new AnimationCurve[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(crankRotationPortId), crankRotationPortId),
            new PortIdField(this, nameof(reverserPortId), reverserPortId),
            new PortIdField(this, nameof(cylindersSteamInjectionPortId), cylindersSteamInjectionPortId),
            new PortIdField(this, nameof(cylinderCockFlowNormalizedPortId), cylinderCockFlowNormalizedPortId),
            new PortIdField(this, nameof(forwardSpeedPortId), forwardSpeedPortId),
        };

        public void OnValidate()
        {
            int length = cylinderSetups.Length;
            _frontParents = new GameObject[length];
            _frontCurves = new AnimationCurve[length];
            _rearParents = new GameObject[length];
            _rearCurves = new AnimationCurve[length];

            for (int i = 0; i < length; i++)
            {
                _frontParents[i] = cylinderSetups[i].frontParticlesParent;
                _frontCurves[i] = cylinderSetups[i].frontActivityCurve;
                _rearParents[i] = cylinderSetups[i].rearParticlesParent;
                _rearCurves[i] = cylinderSetups[i].rearActivityCurve;
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(_frontParents.Length, _frontCurves.Length, _rearParents.Length, _rearCurves.Length);
            cylinderSetups = new CylinderSetup[length];

            for (int i = 0; i < length; i++)
            {
                cylinderSetups[i] = new CylinderSetup
                {
                    frontParticlesParent = _frontParents[i],
                    frontActivityCurve = _frontCurves[i],
                    rearParticlesParent = _rearParents[i],
                    rearActivityCurve = _rearCurves[i]
                };
            }
        }

        public void ApplyS060Defaults()
        {
            ApplyS282Defaults();
        }

        public void ApplyS282Defaults()
        {
            cylinderSetups = new[]
            {
                new CylinderSetup
                {
                    frontParticlesParent = null!,
                    frontActivityCurve = Curve90,
                    rearParticlesParent = null!,
                    rearActivityCurve = Curve270
                },
                new CylinderSetup
                {
                    frontParticlesParent = null!,
                    frontActivityCurve = Curve0,
                    rearParticlesParent = null!,
                    rearActivityCurve = Curve180
                }
            };

            startSpeedMultiplierMin = 1.0f;
            startSpeedMultiplierMax = 2.0f;
            startSizeMultiplierMin = 1.0f;
            startSizeMultiplierMax = 2.0f;
            emissionRateMultiplierMin = 1.0f;
            emissionRateMultiplierMax = 5.0f;
            emissionRateMaxSpeedKmh = 60.0f;
        }

        [Serializable]
        public class CylinderSetup
        {
            public GameObject frontParticlesParent;
            public AnimationCurve frontActivityCurve;
            public GameObject rearParticlesParent;
            public AnimationCurve rearActivityCurve;
        }
    }
}
