using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class RotatorPortReaderProxy : MonoBehaviour, IHasPortIdFields, ICustomSerialized
    {
        [Serializable]
        public class RotationData
        {
            [JsonIgnore]
            public Transform transformToRotate;
            public Vector3 rotationAxis = Vector3.forward;
            public float maxRps = 10f;
        }

        [PortId(null, null, false)]
        public string portId;
        public RotationData[] transformsToRotate;

        [SerializeField, HideInInspector]
        private string? _json;
        [SerializeField, HideInInspector]
        private Transform[] _transforms = new Transform[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId)
        };

        public void OnValidate()
        {
            _transforms = transformsToRotate.Select(x => x.transformToRotate).ToArray();
            _json = JSONObject.ToJson(transformsToRotate);
        }

        public void AfterImport()
        {
            transformsToRotate = JSONObject.FromJson(_json, () => new RotationData[0]);

            for (int i = 0; i < _transforms.Length; i++)
            {
                transformsToRotate[i].transformToRotate = _transforms[i];
            }
        }
    }
}
