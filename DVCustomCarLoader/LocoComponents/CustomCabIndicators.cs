using UnityEngine;
using CCL_GameScripts.CabControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomCabIndicators : MonoBehaviour
    {
        protected CustomLocoController locoController;

        public IndicatorRelay[] Indicators;

        protected virtual void Start()
        {
            locoController = TrainCar.Resolve(gameObject).GetComponent<CustomLocoController>();

            Indicators = GetComponentsInChildren<IndicatorRelay>(true);

            Main.Log($"CustomCabIndicators Start - {Indicators.Length} indicators");
            foreach( var indicator in Indicators )
            {
                indicator.GetValue = locoController.GetIndicatorFunc(indicator.Type);
            }
        }

        protected virtual void Update()
        {
            foreach( var indicator in Indicators )
            {
                indicator.Indicator.value = indicator.GetValue();
            }
        }
    }

    public class IndicatorRelay : MonoBehaviour
    {
        public CabIndicatorType Type;
        public Indicator Indicator;

        [NonSerialized]
        public Func<float> GetValue;

        public void Initialize( CabIndicatorType type, Indicator indicator )
        {
            Type = type;
            Indicator = indicator;
        }
    }
}