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

            if (this.GetComponentInParentInactive<CarLightsOptimizerProxy>() == null)
            {
                message = $"missing {nameof(CarLightsOptimizerProxy)}";
                return SelfValidationResult.Fail;
            }

            if (!headlightSetupsFront.Any(x => x.mainOffSetup))
            {
                message = "no main off setup for front";
                return SelfValidationResult.Fail;
            }

            if (!headlightSetupsRear.Any(x => x.mainOffSetup))
            {
                message = "no main off setup for rear";
                return SelfValidationResult.Fail;
            }

            float offFront = headlightSetupsFront.FirstIndexMatch(x => x.mainOffSetup);
            float offRear = headlightSetupsRear.FirstIndexMatch(x => x.mainOffSetup);

            if (headlightSetupsFront.Length > 1 && headlightSetupsRear.Length > 1)
            {
                message = $"Make sure {FrontText()} and {RearText()}";
            }
            else if (headlightSetupsFront.Length > 1)
            {
                message = $"Make sure {FrontText()}";
            }
            else if (headlightSetupsRear.Length > 1)
            {
                message = $"Make sure {RearText()}";
            }
            else
            {
                message = "no valid number of setups";
                return SelfValidationResult.Fail;
            }

            return SelfValidationResult.Info;

            string FrontText() => $"the default value for {headlightControlFrontId} is {offFront / (headlightSetupsFront.Length - 1)}";
            string RearText() => $"the default value for {headlightControlRearId} is {offRear / (headlightSetupsRear.Length - 1)}";
        }
    }
}
