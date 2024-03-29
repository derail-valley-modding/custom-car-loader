﻿using CCL.Types.Proxies.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class WheelslipControllerProxy : MonoBehaviour, IHasPortIdFields, IDM3Defaults, IDH4Defaults
    {
        public bool preventWheelslip;

        public AnimationCurve wheelslipToAdhesionDrop;
        public float maxWheelslipRpm = 600f;

        [PortId(DVPortValueType.GENERIC, false)]
        public string numberOfPoweredAxlesPortId;

        [PortId(DVPortValueType.STATE, false)]
        public string sandCoefPortId;

        [PortId(DVPortValueType.STATE, false)]
        public string engineBrakingActivePortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(numberOfPoweredAxlesPortId), numberOfPoweredAxlesPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(sandCoefPortId), sandCoefPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(engineBrakingActivePortId), engineBrakingActivePortId, DVPortValueType.STATE),
        };

        private AnimationCurve DefaultAdhesionCurve => new AnimationCurve(
            new Keyframe(0, 1, 0, 0, 0.3333f, 1),
            new Keyframe(0.25f, 0.4f, -0.0085f, -0.0085f, 0.3333f, 0.3333f)
        );

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
    }
}
