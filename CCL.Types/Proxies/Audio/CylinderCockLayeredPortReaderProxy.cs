using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    public class CylinderCockLayeredPortReaderProxy : MonoBehaviour, ICanReplaceInstanced, IHasPortIdFields
    {
        [PortId(DVPortValueType.STATE, false)]
        public string crankRotationPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string cylindersSteamInjectionPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string cylinderCockFlowNormalizedPortId;

        // Using gameobjects to allow the audio replacer to work.
        [Header("order matters, need to match with cylinder index")]
        public GameObject[] cylCockAudio = new GameObject[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(crankRotationPortId), crankRotationPortId),
            new PortIdField(this, nameof(cylindersSteamInjectionPortId), cylindersSteamInjectionPortId),
            new PortIdField(this, nameof(cylinderCockFlowNormalizedPortId), cylinderCockFlowNormalizedPortId),
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
