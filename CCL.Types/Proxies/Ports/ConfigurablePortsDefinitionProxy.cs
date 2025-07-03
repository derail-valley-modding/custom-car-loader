using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Configurable Ports Definition Proxy")]
    public class ConfigurablePortsDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        [Serializable, NotProxied]
        public class PortStartValue
        {
            public PortDefinition Port;
            public float StartingValue;

            public PortStartValue() : this(new PortDefinition(), 0) { }

            public PortStartValue(PortDefinition port, float startingValue)
            {
                Port = port;
                StartingValue = startingValue;
            }
        }

        public PortStartValue[] Ports = new PortStartValue[0];

        [SerializeField, HideInInspector]
        private string? _json;

        public override IEnumerable<PortDefinition> ExposedPorts => Ports.Select(p => p.Port);

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Ports);
        }

        public void AfterImport()
        {
            Ports = JSONObject.FromJson(_json, () => new PortStartValue[0]);
        }
    }
}
