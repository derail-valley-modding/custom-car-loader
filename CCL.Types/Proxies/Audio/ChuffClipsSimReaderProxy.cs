using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    [AddComponentMenu("CCL/Proxies/Audio/Chuff Clips Sim Reader Proxy")]
    public class ChuffClipsSimReaderProxy : MonoBehaviour, ICanReplaceInstanced, IHasPortIdFields
    {
        [Header("Ports")]
        [PortId(DVPortValueType.STATE, false)]
        public string chuffEventPortId = string.Empty;
        [PortId(DVPortValueType.PRESSURE, false)]
        public string exhaustPressurePortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string chuffFrequencyPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string cylinderWaterNormalizedPortId = string.Empty;
        [PortId(DVPortValueType.CONTROL, false)]
        public string cylinderCockControlPortId = string.Empty;

        [Space]
        [Header("Individual chuffs - number of entries must match the number of chuffs per cycle")]
        public OrderedChuffClips[] lowPressureClips = new OrderedChuffClips[0];
        public OrderedChuffClips[] mediumPressureClips = new OrderedChuffClips[0];
        public OrderedChuffClips[] highPressureClips = new OrderedChuffClips[0];
        public IndividualChuffAudioSourceConfig regularChuffConfig = null!;
        public AnimationCurve pressureToVolumeCurve = null!;
        public float mediumPressureThreshold;
        public float highPressureThreshold;

        [Header("Individual water chuffs")]
        public AudioClip[] waterChuffClips = new AudioClip[0];
        public IndividualChuffAudioSourceConfig waterChuffConfig = null!;

        [Header("Individual ash chuffs")]
        public AudioClip[] ashChuffClips = new AudioClip[0];
        public IndividualChuffAudioSourceConfig ashChuffConfig = null!;
        public AnimationCurve ashChuffPressureToVolumeCurve = null!;

        [Space]
        [Header("Loop chuffs")]
        public ChuffLoop[] chuffLoops = new ChuffLoop[0];
        public ChuffLoop[] waterChuffLoops = new ChuffLoop[0];
        public ChuffLoop[] ashChuffLoops = new ChuffLoop[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(chuffEventPortId), chuffEventPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(exhaustPressurePortId), exhaustPressurePortId, DVPortValueType.PRESSURE),
            new PortIdField(this, nameof(chuffFrequencyPortId), chuffFrequencyPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylinderWaterNormalizedPortId), cylinderWaterNormalizedPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylinderCockControlPortId), cylinderCockControlPortId, DVPortValueType.CONTROL),
        };

        public void DoFieldReplacing()
        {
            foreach (var item in chuffLoops)
            {
                if (item.chuffLoop.TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
                {
                    item.chuffLoop = go.InstancedObject!;
                }
            }

            foreach (var item in waterChuffLoops)
            {
                if (item.chuffLoop.TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
                {
                    item.chuffLoop = go.InstancedObject!;
                }
            }

            foreach (var item in ashChuffLoops)
            {
                if (item.chuffLoop.TryGetComponent(out IInstancedObject<GameObject> go) && go.CanReplace)
                {
                    item.chuffLoop = go.InstancedObject!;
                }
            }
        }
    }

    [AddComponentMenu("CCL/Proxies/Audio/Chuff Loop")]
    public class ChuffLoop : MonoBehaviour
    {
        public GameObject chuffLoop = null!;
        public AnimationCurve chuffFrequencyToMasterVolume = null!;
    }

    [AddComponentMenu("CCL/Proxies/Audio/Ordered Chuff Clips")]
    public class OrderedChuffClips : MonoBehaviour
    {
        public AudioClip[] chuffVariations = new AudioClip[0];
    }

    [AddComponentMenu("CCL/Proxies/Audio/Individual Chuff Audio Source Config")]
    public class IndividualChuffAudioSourceConfig : MonoBehaviour
    {
        public Transform parent = null!;
        public AnimationCurve chuffFrequencyToMasterVolume = null!;
        public float pitch = 1f;
        public AnimationCurve spatialCurve = null!;
        public float spread;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public DVAudioMixerGroup mixerGroup;
    }
}
