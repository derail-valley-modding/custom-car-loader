using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    public class TrainCarCustomizationProxy : MonoBehaviour, ICustomSerialized, IHasFuseIdFields
    {
        public enum STDSimPort : byte
        {
            WheelSpeedKMH,
            TractionMotorAmps,
            TractionMotorAmpLimit,
            TractionMotorAmpLimitEffect,
            Temperature
        }

        [Serializable, NotProxied]
        public class STDPortDefinition
        {
            public STDSimPort port;
            [PortId(null, null, false)]
            public string name = string.Empty;
            public bool readOnly;
        }

        [FuseId]
        public string electronicsFuseID = string.Empty;
        public STDPortDefinition[] Ports = new STDPortDefinition[0];

        [SerializeField, HideInInspector]
        private string? _json;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(electronicsFuseID), electronicsFuseID)
        };

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Ports);
        }

        public void AfterImport()
        {
            Ports = JSONObject.FromJson(_json, () => new STDPortDefinition[0]);
        }
    }
}
