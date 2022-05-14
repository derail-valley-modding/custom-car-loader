using System;
using System.Collections.Generic;
using CCL_GameScripts;
using DV.ServicePenalty;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimulation : LocoSimulation, IServicePenaltyProvider
    {
        protected const string TOTAL_FUEL_CONSUMED_SAVE_KEY = "fuelConsumed";
        public float TotalFuelConsumed { get; protected set; }

        public abstract IEnumerable<DebtTrackingInfo> GetDebtComponents();
        public abstract void ResetDebt( DebtComponent debt );
        public abstract void UpdateDebtValue( DebtComponent debt );
        public virtual void ResetFuelConsumption() => TotalFuelConsumed = 0f;
        public abstract IEnumerable<PitStopRefillable> GetPitStopParameters();
        public abstract void ChangePitStopLevel( ResourceType type, float changeAmount );
        public abstract float GetPitStopLevel( ResourceType type );
    }

    public abstract class CustomLocoSimulation<TParams, TDmg> : CustomLocoSimulation
        where TParams : SimParamsBase
        where TDmg : DamageControllerCustomLoco
    {
        public TParams simParams;
        public TDmg dmgController;

        protected override void InitComponents()
        {
            simParams = GetComponent<TParams>();
            if( !simParams ) Main.Error($"Missing {typeof(TParams).Name} on {gameObject.name}");

            dmgController = GetComponent<TDmg>();
            if( !dmgController ) Main.Error($"Missing {typeof(TDmg).Name} on {gameObject.name}");
        }
    }
}