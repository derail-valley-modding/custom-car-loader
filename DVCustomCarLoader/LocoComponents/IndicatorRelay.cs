using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class IndicatorRelay : MonoBehaviour, ILocoEventAcceptor
    {
        public SimEventType EventType;
        public SimEventType[] EventTypes => new[] { EventType };
        public Indicator Indicator;

        public bool IsBoundToInterior { get; set; }

        public void Initialize(SimEventType type, Indicator indicator)
        {
            EventType = type;
            Indicator = indicator;
        }

        [InitSpecAfterInit(typeof(IndicatorSetupBase))]
        public static void FinalizeIndicatorSetup(IndicatorSetupBase spec, Indicator realComp)
        {
            var spawnedObj = realComp.gameObject;
            var indicatorInfo = spawnedObj.AddComponent<IndicatorRelay>();
            indicatorInfo.Initialize(spec.OutputBinding, realComp);
        }

        protected float target = 0;

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float value)
            {
                target = value;

                if (Indicator)
                {
                    Indicator.value = value;
                }
            }
        }

        protected void Update()
        {
            if (Indicator is IndicatorEmission)
            {
                Indicator.value = target;
            }
        }
    }
}
