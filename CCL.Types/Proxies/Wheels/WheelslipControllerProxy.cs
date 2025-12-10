using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Wheelslip Controller Proxy")]
    public class WheelslipControllerProxy : MonoBehaviourWithVehicleDefaults, IHasPortIdFields,
        IDE2Defaults, IDE6Defaults, IDH4Defaults, IDM3Defaults, IDM1UDefaults, IBE2Defaults, IH1Defaults,
        IS060Defaults, IS282Defaults
    {
        public bool preventWheelslip;

        public AnimationCurve wheelslipToAdhesionDrop = null!;
        public float maxWheelslipRpm = 600f;
        [PortId(DVPortValueType.GENERIC, false)]
        public string numberOfPoweredAxlesPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false, false)]
        public string sandCoefPortId = string.Empty;
        [PortId(DVPortValueType.STATE, false, false)]
        public string engineBrakingActivePortId = string.Empty;

        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(SetCurveToDefault), "Set curve to default")]
        private bool _buttons;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(numberOfPoweredAxlesPortId), numberOfPoweredAxlesPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(sandCoefPortId), sandCoefPortId, DVPortValueType.STATE, false),
            new PortIdField(this, nameof(engineBrakingActivePortId), engineBrakingActivePortId, DVPortValueType.STATE, false),
        };

        private AnimationCurve DefaultAdhesionCurve => new AnimationCurve(
            new Keyframe(0, 1, 0, 0, 0.3333f, 1),
            new Keyframe(0.25f, 0.4f, -0.0085f, -0.0085f, 0.3333f, 0.3333f)
        );

        private void SetCurveToDefault()
        {
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
        }

        #region Defaults

        public void ApplyDE2Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 370;
        }

        public void ApplyDE6Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 600;
        }

        public void ApplyDH4Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 300;
        }

        public void ApplyDM3Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 600;
        }

        public void ApplyDM1UDefaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 600;
        }

        public void ApplyBE2Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 370;
        }

        public void ApplyH1Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 300;
        }

        public void ApplyS060Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 370;
        }

        public void ApplyS282Defaults()
        {
            preventWheelslip = false;
            wheelslipToAdhesionDrop = DefaultAdhesionCurve;
            maxWheelslipRpm = 370;
        }

        #endregion
    }
}
