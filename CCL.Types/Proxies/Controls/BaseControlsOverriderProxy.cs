using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Base Controls Overrider Proxy")]
    public class BaseControlsOverriderProxy : MonoBehaviour, ICustomSerialized, IHasPortIdFields
    {
        [Serializable]
        public class PortSetter
        {
            [PortId(null, null)]
            public string portId;
            public float value;

            // Default constructor for deserialization.
            public PortSetter()
            {
                portId = "";
                value = 0f;
            }

            public PortSetter(string portId, float value)
            {
                this.portId = portId;
                this.value = value;
            }
        }

        public bool propagateNeutralStateToFront;
        public bool propagateNeutralStateToRear;

        public PortSetter[] neutralStateSetters = new PortSetter[0];
        [SerializeField, HideInInspector]
        private string _neutralStateSettersJson = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields
        {
            get
            {
                if (neutralStateSetters == null || neutralStateSetters.Length == 0)
                {
                    return Enumerable.Empty<PortIdField>();
                }
                return neutralStateSetters.Select((nss, i) =>
                    new PortIdField(this, nameof(neutralStateSetters), nss.portId));
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
