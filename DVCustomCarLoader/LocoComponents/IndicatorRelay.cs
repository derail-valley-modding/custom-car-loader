using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class IndicatorRelay : MonoBehaviour, ILocoValueWatcher
    {
        [field: SerializeField]
        public CabIndicatorType ValueBinding { get; protected set; }
        public Indicator Indicator;

        public bool IsBound { get; set; } = false;
        public bool IsBoundToInterior { get; set; }
        public Func<float> ValueFunction { get; set; } = null;

        public void Initialize(CabIndicatorType type, Indicator indicator)
        {
            ValueBinding = type;
            Indicator = indicator;
        }

        protected virtual void Update()
        {
            if (ValueFunction != null)
            {
                Indicator.value = ValueFunction();
            }
        }

        [InitSpecAfterInit(typeof(IndicatorSetupBase))]
        public static void FinalizeIndicatorSetup(IndicatorSetupBase spec, Indicator realComp)
        {
            var spawnedObj = realComp.gameObject;
            var indicatorInfo = spawnedObj.AddComponent<IndicatorRelay>();
            indicatorInfo.Initialize(spec.OutputBinding, realComp);
        }
    }
}
