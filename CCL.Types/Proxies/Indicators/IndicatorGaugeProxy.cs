using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorGaugeProxy : IndicatorProxy
    {
        public Transform needle;
        public float minAngle = -180f;
        public float maxAngle = 180f;
        public bool unclamped;
        public Vector3 rotationAxis = Vector3.forward;
        public float gizmoRadius = 0.1f;
    }
}
