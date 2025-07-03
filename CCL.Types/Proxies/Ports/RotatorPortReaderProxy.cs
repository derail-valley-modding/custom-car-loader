using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Rotator Port Reader Proxy")]
    public class RotatorPortReaderProxy : MonoBehaviour, IHasPortIdFields, ICustomSerialized
    {
        [Serializable]
        public class RotationData
        {
            public Transform transformToRotate = null!;
            public Vector3 rotationAxis = Vector3.forward;
            public float maxRps = 10f;
        }

        [PortId(null, null, false)]
        public string portId = string.Empty;
        public RotationData[] transformsToRotate = new RotationData[0];

        [SerializeField, HideInInspector]
        private Transform[] _transforms = new Transform[0];
        [SerializeField, HideInInspector]
        private Vector3[] _axis = new Vector3[0];
        [SerializeField, HideInInspector]
        private float[] _rps = new float[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId)
        };

        public void OnValidate()
        {
            _transforms = new Transform[transformsToRotate.Length];
            _axis = new Vector3[transformsToRotate.Length];
            _rps = new float[transformsToRotate.Length];

            for (int i = 0; i < transformsToRotate.Length; i++)
            {
                _transforms[i] = transformsToRotate[i].transformToRotate;
                _axis[i] = transformsToRotate[i].rotationAxis;
                _rps[i] = transformsToRotate[i].maxRps;
            }
        }

        public void AfterImport()
        {
            transformsToRotate = new RotationData[_transforms.Length];

            for (int i = 0; i < _transforms.Length; i++)
            {
                transformsToRotate[i] = new RotationData
                {
                    transformToRotate = _transforms[i],
                    rotationAxis = _axis[i],
                    maxRps = _rps[i]
                };
            }
        }
    }
}
