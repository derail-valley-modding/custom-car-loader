using CCL_GameScripts;
using DV.ServicePenalty;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomLocoSimSteam : CustomLocoSimulation<SimParamsSteam, CustomDamageControllerSteam>
    {
        public override void ChangePitStopLevel(ResourceType type, float changeAmount)
        {
            throw new NotImplementedException();
        }

        public override JObject GetComponentsSaveData()
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

        public override void LoadComponentsState(JObject stateData)
        {
            throw new NotImplementedException();
        }

        public override void ResetDebt(DebtComponent debt)
        {
            throw new NotImplementedException();
        }

        public override void ResetRefillableSimulationParams()
        {
            throw new NotImplementedException();
        }

        public override void UpdateDebtValue(DebtComponent debt)
        {
            throw new NotImplementedException();
        }

        protected override void SimulateTick(float delta)
        {
            throw new NotImplementedException();
        }
    }
}
