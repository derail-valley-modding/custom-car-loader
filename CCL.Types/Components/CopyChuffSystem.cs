using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyChuffSystem : MonoBehaviour, IInstancedObject<GameObject>, IHasPortIdFields
    {
        public enum Locomotive
        {
            S060,
            S282
        }

        public Locomotive LocomotiveType = Locomotive.S282;

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

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => InstancedObject != null;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
{
            new PortIdField(this, nameof(chuffEventPortId), chuffEventPortId),
            new PortIdField(this, nameof(exhaustPressurePortId), exhaustPressurePortId),
            new PortIdField(this, nameof(chuffFrequencyPortId), chuffFrequencyPortId),
            new PortIdField(this, nameof(cylinderWaterNormalizedPortId), cylinderWaterNormalizedPortId),
            new PortIdField(this, nameof(cylinderCockControlPortId), cylinderCockControlPortId)
        };
    }
}
