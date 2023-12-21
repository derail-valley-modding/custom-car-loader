using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public abstract class IndicatorProxy : MonoBehaviour
    {
        public float minValue;
        public float maxValue = 1f;
    }
}
