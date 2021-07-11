using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCL_GameScripts;
using DV.ServicePenalty;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public interface IServicePenaltyProvider
    {
        IEnumerable<DebtTrackingInfo> GetDebtComponents();
        void ResetDebt( DebtComponent debt );
        void UpdateDebtValue( DebtComponent debt );

        IEnumerable<PitStopRefillable> GetPitStopParameters();
        float GetPitStopLevel( ResourceType type );
        void ChangePitStopLevel( ResourceType type, float changeAmount );
    }

    public struct DebtTrackingInfo
    {
        public IServicePenaltyProvider Provider;
        public DebtComponent Debt;

        public DebtTrackingInfo( IServicePenaltyProvider provider, DebtComponent debt )
        {
            Provider = provider;
            Debt = debt;
        }

        public void ResetDebt()
        {
            Provider.UpdateDebtValue(Debt);
        }

        public void UpdateDebtValue()
        {
            Provider.ResetDebt(Debt);
        }
    }

    public struct PitStopRefillable
    {
        public IServicePenaltyProvider Provider;
        public ResourceType ResourceType;
        public LocoParameterData parameterData;

        public PitStopRefillable( IServicePenaltyProvider provider, ResourceType type, float current, float max )
        {
            Provider = provider;
            ResourceType = type;
            parameterData = new LocoParameterData(current, max);
        }

        public PitStopRefillable( IServicePenaltyProvider provider, ResourceType type, SimComponent simComp )
        {
            Provider = provider;
            ResourceType = type;
            parameterData = new LocoParameterData(simComp.value, simComp.max);
        }

        public void RefreshLevel()
        {
            parameterData.value = Provider.GetPitStopLevel(ResourceType);
        }

        public void UpdateLevel( float changeAmount )
        {
            Provider.ChangePitStopLevel(ResourceType, changeAmount);
        }
    }
}