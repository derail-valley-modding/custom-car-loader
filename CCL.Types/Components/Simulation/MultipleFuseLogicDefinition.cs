using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static CCL.Types.Components.Simulation.FuseLogicDefinition;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Multiple Fuse Logic Definition")]
    public class MultipleFuseLogicDefinition : SimComponentDefinitionProxy, IHasFuseIdFields, ICanSetFuses, ICustomSerialized
    {
        [FuseId(true)]
        public string[] Fuses = new string[0];

        public LogicType Logic = LogicType.AND;

        public FuseDefinition OutputFuse = new FuseDefinition("OUT", false);

        [SerializeField, HideInInspector]
        private string? _json;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => Fuses.Select(x =>
            new FuseIdField(this, nameof(Fuses), x, true));

        public override IEnumerable<FuseDefinition> ExposedFuses => new[]
        {
            OutputFuse
        };

        public IEnumerable<string> SettableFuses => new[]
        {
            GetFullPortId(OutputFuse.id)
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
