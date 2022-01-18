using System.Collections;
using System.Linq;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLocoPitStopParams : CarPitStopParametersBase
    {
        private IServicePenaltyProvider[] providers;
        private PitStopRefillable[] refillables;

        public void Initialize(params IServicePenaltyProvider[] pitStopProviders)
        {
            providers = pitStopProviders;
            InitPitStopParameters();
        }

        protected override void InitPitStopParameters()
        {
            refillables = providers.SelectMany(p => p.GetPitStopParameters()).ToArray();

            carPitStopParameters = refillables.ToDictionary(
                r => r.ResourceType,
                r => r.parameterData);
        }

        protected override void RefreshParameters()
        {
            foreach( var refillable in refillables )
            {
                refillable.RefreshLevel();
            }
        }

        public override void UpdateCarPitStopParameter( ResourceType parameter, float changeAmount )
        {
            foreach( var refillable in refillables )
            {
                if( refillable.ResourceType == parameter )
                {
                    refillable.UpdateLevel(changeAmount);
                    return;
                }
            }
        }
    }
}