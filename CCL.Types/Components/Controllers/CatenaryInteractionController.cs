using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCL.Types.Proxies.Ports;

using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Catenary Interaction Controller")]
    public class CatenaryInteractionController : MonoBehaviour, IHasPortIdFields
    {
        public Transform? pantographBase;
        public Transform? contactStripFirstEnd, contactStripSecondEnd;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, true)]
        public string wireHeightPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        { 
            new PortIdField(this, nameof(wireHeightPortId), wireHeightPortId, DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC)
        };
    }
}
