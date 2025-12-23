using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    public class FuseLogicDefinition : SimComponentDefinitionProxy, IHasFuseIdFields, ICustomSerialized
    {
        public enum LogicType
        {
            AND,
            OR,
            XOR,
            NOR,
            NAND,
            XNOR
        }

        [FuseId(true)]
        public string FuseA = string.Empty;
        [FuseId(true)]
        public string FuseB = string.Empty;

        public LogicType Logic = LogicType.AND;

        public FuseDefinition OutputFuse = new FuseDefinition("OUT", false);

        [SerializeField, HideInInspector]
        private string? _json;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(FuseA), FuseA, true),
            new FuseIdField(this, nameof(FuseB), FuseB, true)
        };

        public void OnValidate()
        {
            _json = JSONObject.ToJson(OutputFuse);
        }

        public void AfterImport()
        {
            OutputFuse = JSONObject.FromJson(_json, () => new FuseDefinition("OUT", false));
        }
    }
}
