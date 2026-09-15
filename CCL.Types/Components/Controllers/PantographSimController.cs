using System.Collections.Generic;

using UnityEngine;

using CCL.Types.Components.Simulation.Electric;
using CCL.Types.Proxies.Ports;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Pantograph Sim Controller")]
    public class PantographSimController : MonoBehaviour, IHasPortIdFields, ISelfValidation
    {
        public Transform? pantographBase;
        public Transform? contactStripFirstEnd;
        public Transform? contactStripSecondEnd;
        
        [Min(0.01f), Tooltip("Maximum vertical offset between wire and strip midpoint for a contact to register")]
        public float contactTolerance = 0.2f;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string initialHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string headHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string wireHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, true)]
        public string wireVoltagePortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, true)]
        public string isInContactPortId = string.Empty;
        [PortId(DVPortValueType.AMPS)]
        public string inputCurrentPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        { 
            new PortIdField(this, nameof(initialHeightPortId), initialHeightPortId, DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(headHeightPortId), headHeightPortId, DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(wireHeightPortId), wireHeightPortId, DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(wireVoltagePortId), wireVoltagePortId, DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS),
            new PortIdField(this, nameof(inputCurrentPortId), inputCurrentPortId, DVPortValueType.AMPS)
        };

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (pantographBase == null)
            { 
                return this.FailForNull(nameof(pantographBase), out message, out highlight); 
            }
            if (contactStripFirstEnd == null)
            { 
                return this.FailForNull(nameof(contactStripFirstEnd), out message, out highlight); 
            }
            if (contactStripSecondEnd == null)
            { 
                return this.FailForNull(nameof(contactStripSecondEnd), out message, out highlight); 
            }
            if (pantographBase == contactStripFirstEnd || pantographBase == contactStripSecondEnd)
            {
                message = $"{nameof(pantographBase)} should not be identical to {nameof(contactStripFirstEnd)} or {nameof(contactStripSecondEnd)}";
                highlight = nameof(pantographBase);
                return SelfValidationResult.Warning;
            }
            if (contactStripFirstEnd == contactStripSecondEnd)
            {
                message = $"{nameof(contactStripFirstEnd)} and {nameof(contactStripSecondEnd)} should not be identical";
                highlight = nameof(contactStripSecondEnd);
                return SelfValidationResult.Warning;
            }
            return this.Pass(out message, out highlight);
        }

        public void ConnectPantograph(PowerCollectorCommonPortsDefinition powerCollectorProxy)
        {
            initialHeightPortId = powerCollectorProxy.GetFullPortId("INITIAL_HEAD_HEIGHT");
            headHeightPortId = powerCollectorProxy.GetFullPortId("HEAD_HEIGHT");
            wireHeightPortId = powerCollectorProxy.GetFullPortId("WIRE_HEIGHT");
            wireVoltagePortId = powerCollectorProxy.GetFullPortId("WIRE_VOLTAGE");
            isInContactPortId = powerCollectorProxy.GetFullPortId("PANTOGRAPH_IN_CONTACT");
        }

        private void Reset()
        {
            if (gameObject.TryGetComponent<PowerCollectorCommonPortsDefinition>(out PowerCollectorCommonPortsDefinition powerCollectorProxy))
                ConnectPantograph(powerCollectorProxy);
        }
    }
}
