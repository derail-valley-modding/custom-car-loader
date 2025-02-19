using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    public class ParticlesPortReadersControllerProxy : MonoBehaviour, ICustomSerialized, ICanReplaceInstanced
    {
        public enum ColorPropertyChange
        {
            ALL,
            RGB_ONLY,
            ALPHA_ONLY
        }

        public List<ParticlePortReader> particlePortReaders = new List<ParticlePortReader>();
        public List<ParticleColorPortReader> particleColorPortReaders = new List<ParticleColorPortReader>();
        public bool selfInitialization = false;

        [SerializeField, HideInInspector]
        private GameObject[] _goPort = new GameObject[0];
        [SerializeField, HideInInspector]
        private string?[] _ports = new string?[0];
        [SerializeField, HideInInspector]
        private GameObject[] _goColorPort = new GameObject[0];
        [SerializeField, HideInInspector]
        private string?[] _colorPorts = new string?[0];

        public void OnValidate()
        {
            int length = particlePortReaders.Count;
            _goPort = new GameObject[length];
            _ports = new string?[length];

            for (int i = 0; i < length; i++)
            {
                _goPort[i] = particlePortReaders[i].particlesParent;
                _ports[i] = JSONObject.ToJson(particlePortReaders[i].particleUpdaters);
            }

            length = particleColorPortReaders.Count;
            _goColorPort = new GameObject[length];
            _colorPorts = new string?[length];

            for (int i = 0; i < length; i++)
            {
                _goColorPort[i] = particleColorPortReaders[i].particlesParent;
                _colorPorts[i] = JSONObject.ToJson(new FakeParticleColorPortReader(particleColorPortReaders[i]));
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(_goPort.Length, _ports.Length);
            particlePortReaders = new List<ParticlePortReader>(length);

            for (int i = 0; i < length; i++)
            {
                particlePortReaders.Add(new ParticlePortReader
                {
                    particlesParent = _goPort[i],
                    particleUpdaters = JSONObject.FromJson(_ports[i], () => new List<ParticlePortReader.PortParticleUpdateDefinition>())
                });
            }


            var fakes = _colorPorts.Select(x => JSONObject.FromJson<FakeParticleColorPortReader>(x)).ToList();
            length = Mathf.Min(_goColorPort.Length, fakes.Count);
            particleColorPortReaders = new List<ParticleColorPortReader>(length);

            for (int i = 0; i < length; i++)
            {
                particleColorPortReaders.Add(fakes[i].ToReal(_goColorPort[i]));
            }
        }

        public void DoFieldReplacing()
        {
            foreach (var item in particlePortReaders)
            {
                if (item.particlesParent.TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
                {
                    item.particlesParent = go.InstancedObject!;
                }
            }

            foreach (var item in particleColorPortReaders)
            {
                if (item.particlesParent.TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
                {
                    item.particlesParent = go.InstancedObject!;
                }
            }
        }

        [Serializable]
        public class ParticlePortReader
        {
            public enum ParticleProperty
            {
                ON_OFF,
                START_LIFETIME,
                START_SIZE,
                START_SPEED,
                MAX_PARTICLES,
                RATE_OVER_TIME,
                START_LIFETIME_MULTIPLIER = 100,
                START_SIZE_MULTIPLIER,
                START_SPEED_MULTIPLIER,
                MAX_PARTICLES_MULTIPLIER,
                RATE_OVER_TIME_MULTIPLIER
            }

            public GameObject particlesParent = null!;
            public List<PortParticleUpdateDefinition> particleUpdaters = new List<PortParticleUpdateDefinition>();

            [Serializable]
            public class PropertyChangeDefinition
            {
                public ParticleProperty propertyType;
                public AnimationCurve propertyChangeCurve = new AnimationCurve();
            }

            [Serializable]
            public class PortParticleUpdateDefinition
            {
                [PortId(null, null, false)]
                public string portId = string.Empty;
                public ValueModifier inputModifier = new ValueModifier();
                public List<PropertyChangeDefinition> propertiesToUpdate = new List<PropertyChangeDefinition>();
            }
        }

        [Serializable]
        public class ParticleColorPortReader
        {
            public GameObject particlesParent = null!;
            [PortId(null, null, false)]
            public string portId = string.Empty;
            public ValueModifier inputModifier = new ValueModifier();
            public ColorPropertyChange changeType;
            public Color startColorMin;
            public Color startColorMax;
            public AnimationCurve colorLerpCurve = new AnimationCurve();
        }

        [Serializable, NotProxied]
        public class FakeParticleColorPortReader
        {
            public string portId = string.Empty;
            public ValueModifier inputModifier = new ValueModifier();
            public ColorPropertyChange changeType;
            public float startColorMinR = 0;
            public float startColorMinG = 0;
            public float startColorMinB = 0;
            public float startColorMinA = 0;
            public float startColorMaxR = 0;
            public float startColorMaxG = 0;
            public float startColorMaxB = 0;
            public float startColorMaxA = 0;
            public AnimationCurve colorLerpCurve = new AnimationCurve();

            // Default constructor for deserialization.
            public FakeParticleColorPortReader() { }

            public FakeParticleColorPortReader(ParticleColorPortReader p)
            {
                portId = p.portId;
                inputModifier = p.inputModifier;
                changeType = p.changeType;
                startColorMinR = p.startColorMin.r;
                startColorMinG = p.startColorMin.g;
                startColorMinB = p.startColorMin.b;
                startColorMinA = p.startColorMin.a;
                startColorMaxR = p.startColorMax.r;
                startColorMaxG = p.startColorMax.g;
                startColorMaxB = p.startColorMax.b;
                startColorMaxA = p.startColorMax.a;
                colorLerpCurve = p.colorLerpCurve;
            }

            public ParticleColorPortReader ToReal(GameObject go)
            {
                return new ParticleColorPortReader
                {
                    particlesParent = go,
                    portId = portId,
                    inputModifier = inputModifier,
                    changeType = changeType,
                    startColorMin = new Color(startColorMinR, startColorMinG, startColorMinB, startColorMinA),
                    startColorMax = new Color(startColorMaxR, startColorMaxG, startColorMaxB, startColorMaxA),
                    colorLerpCurve = colorLerpCurve
                };
            }
        }

        [Serializable]
        public class ValueModifier
        {
            public float valueMultiplier = 1;
            public float valueOffset = 0;
            public bool absoluteInputValue = false;
            public bool absoluteResultValue = false;
        }
    }
}
