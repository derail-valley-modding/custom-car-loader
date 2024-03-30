using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    public class CylinderCockParticlePortReaderProxy : AParticlePortReaderProxy, IHasPortIdFields, ICustomSerialized
    {
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
