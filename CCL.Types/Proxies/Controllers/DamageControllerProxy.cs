using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Damage Controller Proxy")]
    public class DamageControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        public AnimationCurve speedToBrakeDamageCurve = null!;

        //[Header("Windows - set to null if unused")]
        //public WindowsBreakingController windows;

        [Header("Body Damage")]
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] bodyDamagerPortIds = Array.Empty<string>();
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] bodyHealthStateExternalInPortIds = Array.Empty<string>();

        [Header("Mechanical Damage")]
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] mechanicalPTDamagerPortIds = Array.Empty<string>();
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] mechanicalPTPercentualDamagerPortIds = Array.Empty<string>();
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] mechanicalPTHealthStateExternalInPortIds = Array.Empty<string>();
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] mechanicalPTOffExternalInPortIds = Array.Empty<string>();

        [Header("Electrical Damage")]
        [PortId(DVPortValueType.DAMAGE, false)]
        public string[] electricalPTDamagerPortIds = Array.Empty<string>();
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] electricalPTHealthStateExternalInPortIds = Array.Empty<string>();
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string[] electricalPTOffExternalInPortIds = Array.Empty<string>();

        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(SetCurveToDefault), "Set curve to default")]
        private bool _buttons;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(bodyDamagerPortIds), bodyDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(bodyHealthStateExternalInPortIds), bodyHealthStateExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            
            new PortIdField(this, nameof(mechanicalPTDamagerPortIds), mechanicalPTDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(mechanicalPTPercentualDamagerPortIds), mechanicalPTPercentualDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(mechanicalPTHealthStateExternalInPortIds), mechanicalPTHealthStateExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            new PortIdField(this, nameof(mechanicalPTOffExternalInPortIds), mechanicalPTOffExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),

            new PortIdField(this, nameof(electricalPTDamagerPortIds), electricalPTDamagerPortIds, DVPortValueType.DAMAGE),
            new PortIdField(this, nameof(electricalPTHealthStateExternalInPortIds), electricalPTHealthStateExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            new PortIdField(this, nameof(electricalPTOffExternalInPortIds), electricalPTOffExternalInPortIds, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
        };

        private AnimationCurve DefaultDamageCurve => new AnimationCurve(
            new Keyframe(0, 0, 0, 0, 0.333f, 0.333f),
            new Keyframe(2, 0, 0, 0, 0.333f, 0.333f),
            new Keyframe(7.868f, 0.127f, 0.028f, 0.028f, 0.333f, 0.333f),
            new Keyframe(29.032f, 0.671f, 0.015f, 0.015f, 0.333f, 0.333f),
            new Keyframe(100, 1, 0, 0, 0.333f, 0.333f));

        public void SetCurveToDefault()
        {
            speedToBrakeDamageCurve = DefaultDamageCurve;
        }
    }
}