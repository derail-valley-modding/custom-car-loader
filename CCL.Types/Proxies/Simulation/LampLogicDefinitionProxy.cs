using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Lamp Logic Definition Proxy")]
    public class LampLogicDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized, IHasFuseIdFields
    {
        [Header("Behaviour setup")]
        public float offRangeMin;
        public float offRangeMax;

        [Space]
        public bool onRangeUsed;
        public float onRangeMin;
        public float onRangeMax;

        [Space]
        public bool blinkRangeUsed;
        public float blinkRangeMin;
        public float blinkRangeMax;

        [Space]
        public bool playAudioOnValueDrop;
        public bool playAudioOnValueRaise;

        public PortReferenceDefinition inputReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "INPUT");

        [Header("Optional")]
        [FuseId]
        public string powerFuseId = string.Empty;

        [SerializeField, HideInInspector]
        private string? _json;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "LAMP_STATE")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            inputReader
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId)
        };

        public string GetFullReaderPortRefId() => GetFullPortId(inputReader.ID);

        public void OnValidate()
        {
            _json = JSONObject.ToJson(inputReader);
        }

        public void AfterImport()
        {
            inputReader = JSONObject.FromJson(_json, () => new PortReferenceDefinition(DVPortValueType.GENERIC, "INPUT"));
        }
    }
}
