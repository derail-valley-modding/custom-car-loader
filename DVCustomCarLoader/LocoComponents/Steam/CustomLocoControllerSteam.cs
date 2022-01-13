using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomLocoControllerSteam :
        CustomLocoController<CustomLocoSimSteam, CustomDamageControllerSteam, CustomLocoSimEventsSteam>
    {
        public override void SetNeutralState()
        {
            throw new NotImplementedException();
        }
    }
}
