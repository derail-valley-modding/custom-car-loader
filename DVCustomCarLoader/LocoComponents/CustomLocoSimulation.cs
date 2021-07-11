using System;
using System.Collections.Generic;
using CCL_GameScripts;
using DV.ServicePenalty;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimulation : LocoSimulation, IServicePenaltyProvider
    {
        public abstract IEnumerable<DebtTrackingInfo> GetDebtComponents();
        public abstract void ResetDebt( DebtComponent debt );
        public abstract void UpdateDebtValue( DebtComponent debt );
        public virtual void ResetFuelConsumption() { }
        public abstract IEnumerable<PitStopRefillable> GetPitStopParameters();
        public abstract void ChangePitStopLevel( ResourceType type, float changeAmount );
        public abstract float GetPitStopLevel( ResourceType type );
    }

    public abstract class CustomLocoSimulation<TParams> : CustomLocoSimulation
        where TParams : SimParamsBase
    {
        public TParams simParams;

        protected override void InitComponents()
        {
            simParams = GetComponent<TParams>();
        }
    }
}