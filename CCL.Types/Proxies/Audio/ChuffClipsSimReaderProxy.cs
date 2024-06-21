using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    public class ChuffClipsSimReaderProxy : MonoBehaviour, ICanReplaceInstanced, IHasPortIdFields
    {
        [Header("Ports")]
        [PortId(DVPortValueType.STATE, false)]
        public string chuffEventPortId;
        [PortId(DVPortValueType.PRESSURE, false)]
        public string exhaustPressurePortId;
        [PortId(DVPortValueType.STATE, false)]
        public string chuffFrequencyPortId;
        [PortId(DVPortValueType.STATE, false)]
        public string cylinderWaterNormalizedPortId;
        [PortId(DVPortValueType.CONTROL, false)]
        public string cylinderCockControlPortId;

        [Space]
        [Header("Individual chuffs - number of entries must match the number of chuffs per cycle")]
        public OrderedChuffClips[] lowPressureClips = new OrderedChuffClips[0];
        public OrderedChuffClips[] mediumPressureClips = new OrderedChuffClips[0];
        public OrderedChuffClips[] highPressureClips = new OrderedChuffClips[0];
        public IndividualChuffAudioSourceConfig regularChuffConfig;
        public AnimationCurve pressureToVolumeCurve;
        public float mediumPressureThreshold;
        public float highPressureThreshold;

        [Header("Individual water chuffs")]
        public AudioClip[] waterChuffClips = new AudioClip[0];
        public IndividualChuffAudioSourceConfig waterChuffConfig;

        [Header("Individual ash chuffs")]
        public AudioClip[] ashChuffClips = new AudioClip[0];
        public IndividualChuffAudioSourceConfig ashChuffConfig;
        public AnimationCurve ashChuffPressureToVolumeCurve;

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

    public class ChuffLoop : MonoBehaviour
    {
        public GameObject chuffLoop;
        public AnimationCurve chuffFrequencyToMasterVolume;
    }

    public class OrderedChuffClips : MonoBehaviour
    {
        public AudioClip[] chuffVariations;
    }

    public class IndividualChuffAudioSourceConfig : MonoBehaviour
    {
        public Transform parent;
        public AnimationCurve chuffFrequencyToMasterVolume;
        public float pitch = 1f;
        public AnimationCurve spatialCurve;
        public float spread;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public DVAudioMixerGroup mixerGroup;
    }
}
