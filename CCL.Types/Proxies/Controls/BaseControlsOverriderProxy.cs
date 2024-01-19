using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class BaseControlsOverriderProxy : MonoBehaviour, ICustomSerialized
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
