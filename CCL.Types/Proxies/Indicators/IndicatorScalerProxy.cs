using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorScalerProxy : IndicatorProxy
    {
        public Transform indicatorToScale;
        public Vector3 startScale = Vector3.one;
        public Vector3 endScale = Vector3.one;
    }
}
