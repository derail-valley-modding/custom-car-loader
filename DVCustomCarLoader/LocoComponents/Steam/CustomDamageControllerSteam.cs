using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts;
using DV.ServicePenalty;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomDamageControllerSteam :
        DamageControllerCustomLoco<CustomLocoControllerSteam, CustomLocoSimEventsSteam, DamageConfigSteam>
    {
        public override void ChangePitStopLevel(ResourceType type, float changeAmount)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            throw new NotImplementedException();
        }

        public override float GetPitStopLevel(ResourceType type)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            throw new NotImplementedException();
        }

        public override void ResetDebt(DebtComponent debt)
        {
            throw new NotImplementedException();
        }

        public override void UpdateDebtValue(DebtComponent debt)
        {
            throw new NotImplementedException();
        }
    }
}
