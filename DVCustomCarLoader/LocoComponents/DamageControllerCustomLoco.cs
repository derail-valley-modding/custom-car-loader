using System.Collections.Generic;
using DV.ServicePenalty;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class DamageControllerCustomLoco : DamageController, IServicePenaltyProvider
    {
        public abstract IEnumerable<DebtTrackingInfo> GetDebtComponents();
        public abstract void ResetDebt( DebtComponent debt );
        public abstract void UpdateDebtValue( DebtComponent debt );
        public abstract IEnumerable<PitStopRefillable> GetPitStopParameters();
        public abstract float GetPitStopLevel( ResourceType type );
        public abstract void ChangePitStopLevel( ResourceType type, float changeAmount );
    }
}