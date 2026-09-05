using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCL.Types.Components.Simulation.Electric;
using CCL.Types.Proxies.Ports;

using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Pantograph Sim Controller")]
    public class PantographSimController : MonoBehaviour, IHasPortIdFields
    {
        public Transform? pantographBase;
        public Transform? contactStripFirstEnd, contactStripSecondEnd;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string initialHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string headHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string wireHeightPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, true)]
        public string wireVoltagePortId = string.Empty;
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

        public void ConnectPantograph(PantographDefinition pantographProxy)
        {
            initialHeightPortId = pantographProxy.GetFullPortId("INITIAL_HEAD_HEIGHT");
            headHeightPortId = pantographProxy.GetFullPortId("HEAD_HEIGHT");
            wireHeightPortId = pantographProxy.GetFullPortId("WIRE_HEIGHT");
            wireVoltagePortId = pantographProxy.GetFullPortId("WIRE_VOLTAGE");
        }

        private void Reset()
        {
            if (gameObject.TryGetComponent<PantographDefinition>(out PantographDefinition pantographProxy))
                ConnectPantograph(pantographProxy);
        }
    }
}
