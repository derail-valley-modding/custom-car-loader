using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public class HeadlightsMainControllerProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId(DVPortValueType.CONTROL, false)]
        public string headlightControlFrontId;
        [PortId(DVPortValueType.CONTROL, false)]
        public string headlightControlRearId;
        [FuseId]
        public string powerFuseId;

        public float damagedThresholdPercentage = 0.5f;
        public HeadlightSetup[] headlightSetupsFront;
        public HeadlightSetup[] headlightSetupsRear;

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
    }
}
