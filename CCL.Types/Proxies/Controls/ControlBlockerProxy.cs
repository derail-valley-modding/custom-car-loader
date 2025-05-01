using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class ControlBlockerProxy : MonoBehaviour, IHasPortIdFields, ICustomSerialized
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string blockedControlPortId = string.Empty;
        public bool resetToZeroOnBlock;
        public BlockerDefinition[] blockers = new BlockerDefinition[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] { new PortIdField(this, nameof(blockedControlPortId), blockedControlPortId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL) };

        [SerializeField, HideInInspector]
        private string? _json;

        [Serializable]
        public class BlockerDefinition
        {
            public enum BlockType
            {
                BLOCK_ON_ABOVE_THRESHOLD,
                BLOCK_ON_BELOW_THRESHOLD,
                BLOCK_ON_EQUAL_TO_THRESHOLD
            }

            [PortId(null, null, false)]
            public string blockerPortId = string.Empty;
            public float thresholdValue;
            public BlockType blockType;
        }

        public void OnValidate()
        {
            _json = JSONObject.ToJson(blockers);
        }

        public void AfterImport()
        {
            blockers = JSONObject.FromJson(_json, () => new BlockerDefinition[0]);
        }
    }
}
