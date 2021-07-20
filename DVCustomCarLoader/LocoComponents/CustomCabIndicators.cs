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

        public IndicatorInfo[] Indicators;

        //public void AddIndicators( IEnumerable<(CabIndicatorType, Indicator)> comps )
        //{
        //    if( Indicators == null ) Indicators = new List<IndicatorInfo>();

        //    Indicators.AddRange(comps.Select(c => new IndicatorInfo(c.Item1, c.Item2)));
        //}

        protected virtual void Start()
        {
            locoController = TrainCar.Resolve(gameObject).GetComponent<CustomLocoController>();

            Indicators = GetComponentsInChildren<IndicatorInfo>();

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

    public class IndicatorInfo : MonoBehaviour
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