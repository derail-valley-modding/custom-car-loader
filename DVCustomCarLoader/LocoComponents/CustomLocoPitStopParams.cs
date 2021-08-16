using System.Collections;
using System.Linq;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLocoPitStopParams : CarPitStopParametersBase
    {
        private DamageControllerCustomLoco dmgController;
        private CustomLocoSimulation sim;

        private PitStopRefillable[] refillables;

        public void Initialize( CustomLocoSimulation sim, DamageControllerCustomLoco dmg )
        {
            this.sim = sim;
            dmgController = dmg;
            InitPitStopParameters();
        }

        protected override void InitPitStopParameters()
        {
            refillables = 
                sim.GetPitStopParameters()
                .Concat(dmgController.GetPitStopParameters())
                .ToArray();

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