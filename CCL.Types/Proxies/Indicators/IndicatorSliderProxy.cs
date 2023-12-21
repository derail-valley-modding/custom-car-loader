using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorSliderProxy : IndicatorProxy
    {
        public Transform pointer;
        public Vector3 startPosition = -Vector3.right;
        public Vector3 endPosition = Vector3.right;
    }
}
