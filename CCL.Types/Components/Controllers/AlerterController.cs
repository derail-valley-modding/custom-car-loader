using CCL.Types.Components.Simulation;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Alerter Controller")]
    public class AlerterController : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.STATE)]
        public string AlerterStatePortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN)]
        public string AlerterActivePortId = string.Empty;
        public float ControlsImpulseInterval = 0.2f;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(AlerterActivePortId), AlerterActivePortId, DVPortType.EXTERNAL_IN),
            new PortIdField(this, nameof(AlerterStatePortId), AlerterStatePortId, DVPortValueType.STATE)
        };

        private void Reset()
        {
            if (!TryGetComponent(out AlerterDefinition def)) return;

            AlerterStatePortId = def.GetFullPortId("STATE");
            AlerterActivePortId = def.GetFullPortId("ACTIVE");
        }
    }
}
