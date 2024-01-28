using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class BaseControlsOverriderProxy : MonoBehaviour, ICustomSerialized, IHasPortIdFields
    {
        [Serializable]
        public class PortSetter
        {
            [PortId(null, null)]
            public string portId;

            public float value;

            public PortSetter(string portId, float value)
            {
                this.portId = portId;
                this.value = value;
            }
        }

        public bool propagateNeutralStateToFront;
        public bool propagateNeutralStateToRear;

        public PortSetter[] neutralStateSetters;
        [SerializeField]
        [HideInInspector]
        private string _neutralStateSettersJson;

        public IEnumerable<PortIdField> ExposedPortIdFields
        {
            get
            {
                if (neutralStateSetters == null || neutralStateSetters.Length == 0)
                {
                    return Enumerable.Empty<PortIdField>();
                }
                return neutralStateSetters.Select((nss, i) =>
                    new PortIdField(this, $"{nameof(PortSetter.portId)}_{i}", nss.portId));
            }
        }

        public void OnValidate()
        {
            _neutralStateSettersJson = JSONObject.ToJson(neutralStateSetters);
        }

        public void AfterImport()
        {
            neutralStateSetters = JSONObject.FromJson<PortSetter[]>(_neutralStateSettersJson);
        }
    }
}
