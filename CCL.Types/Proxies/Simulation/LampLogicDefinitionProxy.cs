using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class LampLogicDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields
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
    }
}
