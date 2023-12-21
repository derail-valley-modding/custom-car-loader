using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorModelChangerProxy : IndicatorProxy
    {
        [Tooltip("Ordered list of different models that indicator will switch on/off depending on the indicator value")]
        public GameObject[] indicatorModels;

        [Tooltip("Specific ordered low to high percentages, that tell us when the model switch will occur. Number of switchPercentages should always be indicatorModels.Count - 1, because we have implicit 0 percentage")]
        public float[] switchPercentage;
    }
}
