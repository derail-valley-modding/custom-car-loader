using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Constant Multiplier + Offset Definition")]
    public class ConstantMultiplierOffsetDefinition : SimComponentDefinitionProxy
    {
        public float Multiplier = 1;
        public float Offset = 0;
        public PortReferenceDefinition Input = new PortReferenceDefinition(DVPortValueType.GENERIC, "IN");
        public PortDefinition Output = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT");

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            Output
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            Input
        };

        [SerializeField, HideInInspector]
        private string? _inJson;
        [SerializeField, HideInInspector]
        private string? _outJson;

        public void OnValidate()
        {
            _inJson = JSONObject.ToJson(Input);
            _outJson = JSONObject.ToJson(Output);
        }

        public void AfterImport()
        {
            Input = JSONObject.FromJson(_inJson, () => new PortReferenceDefinition(DVPortValueType.GENERIC, "IN"));
            Output = JSONObject.FromJson(_outJson, () => new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT"));
        }
    }
}
