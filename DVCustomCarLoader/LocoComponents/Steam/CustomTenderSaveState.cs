using DV;
using DV.JObjectExtstensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomTenderSaveState : LocoStateSave
    {
        protected const string SIMULATION_SAVE_KEY = "sim";
        protected const string VISIT_SAVE_KEY = "visit";

        protected CustomTenderSimulation sim;
        protected CarVisitChecker visitChecker;

        public void Initialize(CustomTenderSimulation sim, CarVisitChecker visitChecker)
        {
            this.sim = sim;
            this.visitChecker = visitChecker;
        }

        public override JObject GetLocoStateSaveData()
        {
            var simData = sim.GetComponentsSaveData();
            var data = new JObject()
            {
                { SIMULATION_SAVE_KEY, simData }
            };

            if (visitChecker.RecentlyVisitedRemainingTime > 0)
            {
                data.SetFloat(VISIT_SAVE_KEY, visitChecker.RecentlyVisitedRemainingTime);
            }
            return data;
        }

        public override void SetLocoStateSaveData(JObject saveData)
        {
            if (saveData.GetJObject(SIMULATION_SAVE_KEY) is JObject simData)
            {
                sim.LoadComponentsState(simData);
            }
            else
            {
                Main.Error("Failed to load sim data for TenderSimulation");
            }

            float? visitTime = saveData.GetFloat(VISIT_SAVE_KEY);
            if (visitTime.HasValue)
            {
                visitChecker.LoadData(visitTime.Value);
            }
        }
    }
}
