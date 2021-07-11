using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader
{
    public static class Extensions
    {
        public static bool IsEnvironmental( this ResourceType type )
        {
            return
                (type == ResourceType.EnvironmentDamageCargo) ||
                (type == ResourceType.EnvironmentDamageFuel) ||
                (type == ResourceType.EnvironmentDamageCoal);
        }
    }
}
