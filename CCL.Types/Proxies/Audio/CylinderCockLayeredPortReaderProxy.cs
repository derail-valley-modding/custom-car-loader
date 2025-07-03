using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    public class CylinderCockLayeredPortReaderProxy : MonoBehaviour, ICanReplaceInstanced, IHasPortIdFields
    {
        [PortId(DVPortValueType.STATE, false)]
        public string crankRotationPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string cylindersInletValveOpenPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string cylinderCockFlowNormalizedPortId = string.Empty;
        [PortId(DVPortValueType.CONTROL, false)]
        public string cylinderCockControlPortId = string.Empty;

        // Using gameobjects to allow the audio replacer to work.
        [Header("Order matters, must match with cylinder index")]
        public GameObject[] cylCockAudio = new GameObject[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(crankRotationPortId), crankRotationPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylindersInletValveOpenPortId), cylindersInletValveOpenPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylinderCockFlowNormalizedPortId), cylinderCockFlowNormalizedPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylinderCockControlPortId), cylinderCockControlPortId, DVPortValueType.CONTROL),
        };

        public void DoFieldReplacing()
        {
            for (int i = 0; i < cylCockAudio.Length; i++)
            {
                if (cylCockAudio[i].TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
                {
                    cylCockAudio[i] = go.InstancedObject!;
                }
            }
        }
    }
}
