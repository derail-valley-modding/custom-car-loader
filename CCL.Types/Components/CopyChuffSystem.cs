using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Copiers/Copy Chuff System")]
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
        public string chuffEventPortId = string.Empty;
        [PortId(DVPortValueType.PRESSURE, false)]
        public string exhaustPressurePortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string chuffFrequencyPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string cylinderWaterNormalizedPortId = string.Empty;
        [PortId(DVPortValueType.CONTROL, false)]
        public string cylinderCockControlPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false)]
        public string ashesInPipesPortId = string.Empty;

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => InstancedObject != null;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
{
            new PortIdField(this, nameof(chuffEventPortId), chuffEventPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(exhaustPressurePortId), exhaustPressurePortId, DVPortValueType.PRESSURE),
            new PortIdField(this, nameof(chuffFrequencyPortId), chuffFrequencyPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylinderWaterNormalizedPortId), cylinderWaterNormalizedPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(cylinderCockControlPortId), cylinderCockControlPortId, DVPortValueType.CONTROL),
            new PortIdField(this, nameof(ashesInPipesPortId), ashesInPipesPortId, DVPortValueType.STATE),
        };
    }
}
