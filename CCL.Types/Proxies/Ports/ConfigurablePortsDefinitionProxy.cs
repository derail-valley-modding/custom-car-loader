using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurablePortsDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public PortStartValue[] Ports = new PortStartValue[0];

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

    [Serializable]
    public class PortStartValue
    {
        public PortDefinition Port;
        public float StartingValue;

        public PortStartValue() { }

        public PortStartValue(PortDefinition port, float startingValue)
        {
            Port = port;
            StartingValue = startingValue;
        }
    }
}
