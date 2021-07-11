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

        // Sanders
        public bool sandOn;
        public SimComponent sand;
        public SimComponent sandFlow = new SimComponent("SandFlow", 0f, 1f, 0.1f, 0f);

        protected override void InitComponents()
        {
            simParams = GetComponent<TParams>();
            sand = new SimComponent("Sand", 0f, simParams.SandCapacity, 40f, simParams.SandCapacity);
        }

        public override IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            return new[]
            {
                new DebtTrackingInfo(this, new DebtComponent(sand.value, ResourceType.Sand))
            };
        }

        public override void ResetDebt( DebtComponent debt )
        {
            if( debt.type == ResourceType.Sand )
            {
                debt.ResetComponent(sand.value);
            }
        }

        public override void UpdateDebtValue( DebtComponent debt )
        {
            if( debt.type == ResourceType.Sand )
            {
                debt.UpdateEndValue(sand.value);
            }
        }

        public override IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            return new[]
            {
                new PitStopRefillable(this, ResourceType.Sand, sand)
            };
        }

        public override void ChangePitStopLevel( ResourceType type, float changeAmount )
        {
            if( type == ResourceType.Sand )
            {
                sand.AddValue(changeAmount);
            }
        }

        public override float GetPitStopLevel( ResourceType type )
        {
            if( type == ResourceType.Sand )
            {
                return sand.value;
            }

            Main.Warning("Tried to get pit stop value this loco sim doesn't have");
            return 0;
        }
    }
}