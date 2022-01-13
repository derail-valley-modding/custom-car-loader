using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomLocoSimEventsSteam : CustomLocoSimEvents<CustomLocoSimSteam, CustomDamageControllerSteam>
    {
        protected override void CheckTankAndDamageLevels()
        {
            throw new NotImplementedException();
        }

        protected override void InitThresholds()
        {
            throw new NotImplementedException();
        }
    }
}
