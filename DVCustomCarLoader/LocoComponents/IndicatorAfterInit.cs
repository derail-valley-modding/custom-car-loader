using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.CabControls;

namespace DVCustomCarLoader.LocoComponents
{
    static class IndicatorAfterInit
    {
        [InitSpecAfterInit(typeof(IndicatorSetupBase))]
        public static void FinalizeIndicatorSetup(IndicatorSetupBase spec, Indicator realComp)
        {
            var spawnedObj = realComp.gameObject;
            var realIndicator = spawnedObj.GetComponent<Indicator>();
            var indicatorInfo = spawnedObj.AddComponent<IndicatorRelay>();
            indicatorInfo.Type = spec.OutputBinding;
            indicatorInfo.Indicator = realIndicator;
        }
    }
}
