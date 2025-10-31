using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [AddComponentMenu("CCL/Proxies/Headlights/Headlights Main Controller Proxy")]
    public class HeadlightsMainControllerProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields, ISelfValidation
    {
        [PortId(DVPortValueType.CONTROL, false)]
        public string headlightControlFrontId = string.Empty;
        [PortId(DVPortValueType.CONTROL, false)]
        public string headlightControlRearId = string.Empty;
        [FuseId]
        public string powerFuseId = string.Empty;

        public float damagedThresholdPercentage = 0.5f;
        public HeadlightSetup[] headlightSetupsFront = new HeadlightSetup[0];
        public HeadlightSetup[] headlightSetupsRear = new HeadlightSetup[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(headlightControlFrontId), headlightControlFrontId, DVPortValueType.CONTROL),
            new PortIdField(this, nameof(headlightControlRearId), headlightControlRearId, DVPortValueType.CONTROL)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId)
        };

        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(AutoSetupSetups), "Auto Setup")]
        private bool _renderButton;

        private void AutoSetupSetups()
        {
            var t = transform.Find("FrontSide");

            if (t)
            {
                headlightSetupsFront = t.GetComponentsInChildren<HeadlightSetup>();
            }

            t = transform.Find("RearSide");

            if (t)
            {
                headlightSetupsRear = t.GetComponentsInChildren<HeadlightSetup>();
            }
        }

        public SelfValidationResult Validate(out string message)
        {
            if (headlightSetupsFront.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(headlightSetupsFront), out message);
            }

            if (headlightSetupsRear.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(headlightSetupsRear), out message);
            }

            return this.Pass(out message);
        }
    }
}
