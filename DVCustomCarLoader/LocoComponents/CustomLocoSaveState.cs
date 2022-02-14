using DV;
using DV.JObjectExtstensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSaveState : LocoStateSave
    {
        public virtual void Initialize(CarVisitChecker checker)
        {

        }
    }

    public abstract class CustomLocoSaveState<TSim, TDmg, TCtrl> : CustomLocoSaveState
        where TSim : CustomLocoSimulation
        where TDmg : DamageControllerCustomLoco
        where TCtrl : CustomLocoController
    {
        protected const string SIMULATION_SAVE_KEY = "sim";
        protected const string DAMAGE_SAVE_KEY = "dmg";
        protected const string VISIT_CHECKER_KEY = "visit";

        protected TSim locoSim;
        protected TDmg locoDmg;
        protected TCtrl controller;
        protected CarVisitChecker visitChecker;

        public override void Initialize(CarVisitChecker checker)
        {
            locoSim = GetComponent<TSim>();
            locoDmg = GetComponent<TDmg>();
            controller = GetComponent<TCtrl>();
            visitChecker = checker;
        }

        public override JObject GetLocoStateSaveData()
        {
            var data = new JObject();
            data.SetJObject(SIMULATION_SAVE_KEY, locoSim.GetComponentsSaveData());
            data.SetJObject(DAMAGE_SAVE_KEY, locoDmg.GetDamageSaveData());

            if (visitChecker.RecentlyVisitedRemainingTime > 0)
            {
                data.SetFloat(VISIT_CHECKER_KEY, visitChecker.RecentlyVisitedRemainingTime);
            }
            return data;
        }

        public override void SetLocoStateSaveData(JObject saveData)
        {
            if (saveData.GetJObject(SIMULATION_SAVE_KEY) is JObject simData)
            {
                locoSim.LoadComponentsState(simData);
            }
            else Main.Error($"Failed to load sim data for {GetType().Name}");

            if (saveData.GetJObject(DAMAGE_SAVE_KEY) is JObject dmgData)
            {
                locoDmg.LoadDamagesState(dmgData);
            }
            else Main.Error($"Failed to load dmg data for {GetType().Name}");

            float? visitTime = saveData.GetFloat(VISIT_CHECKER_KEY);
            if (visitTime.HasValue)
            {
                visitChecker.LoadData(visitTime.Value);
            }
        }
    }
}
