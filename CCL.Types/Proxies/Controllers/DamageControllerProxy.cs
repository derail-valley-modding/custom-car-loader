using CCL.Types.Proxies.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class DamageControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        public AnimationCurve speedToBrakeDamageCurve;

        //[Header("Windows - set to null if unused")]
        //public WindowsBreakingController windows;

        [Header("Body Damage")]
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] bodyDamagerPortIds;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] bodyHealthStateExternalInPortIds;

        [Header("Mechanical Damage")]
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] mechanicalPTDamagerPortIds;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] mechanicalPTHealthStateExternalInPortIds;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] mechanicalPTOffExternalInPortIds;

        [Header("Electrical Damage")]
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] electricalPTDamagerPortIds;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] electricalPTHealthStateExternalInPortIds;

        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] electricalPTOffExternalInPortIds;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(bodyDamagerPortIds), bodyDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(bodyHealthStateExternalInPortIds), bodyHealthStateExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            
            new PortIdField(this, nameof(mechanicalPTDamagerPortIds), mechanicalPTDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(mechanicalPTHealthStateExternalInPortIds), mechanicalPTHealthStateExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            new PortIdField(this, nameof(mechanicalPTOffExternalInPortIds), mechanicalPTOffExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),

            new PortIdField(this, nameof(electricalPTDamagerPortIds), electricalPTDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(electricalPTHealthStateExternalInPortIds), electricalPTHealthStateExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            new PortIdField(this, nameof(electricalPTOffExternalInPortIds), electricalPTOffExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
        };
    }
}