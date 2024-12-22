using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    public class TrainCarCustomizationProxy : MonoBehaviour, ICustomSerialized
    {
        public enum STDSimPort : byte
        {
            WheelSpeedKMH,
            TractionMotorAmps,
            TractionMotorAmpLimit,
            TractionMotorAmpLimitEffect,
            Temperature
        }

        [Serializable]
        public class STDPortDefinition
        {
            public STDSimPort port;
            [PortId(null, null, false)]
            public string name;
            public bool readOnly;
        }

        [FuseId]
        public string electronicsFuseID;
        public STDPortDefinition[] Ports = new STDPortDefinition[0];

        [SerializeField, HideInInspector]
        private string? _json;

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
