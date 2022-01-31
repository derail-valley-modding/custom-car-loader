using DV;
using DV.JObjectExtstensions;
using DV.MultipleUnit;
using Newtonsoft.Json.Linq;

namespace DVCustomCarLoader.LocoComponents.DieselElectric
{
    public class CustomDieselSaveState : CustomLocoSaveState<CustomLocoSimDiesel, DamageControllerCustomDiesel, CustomLocoControllerDiesel>
    {
		protected MultipleUnitModule muModule;

        protected const string ENGINE_ON_SAVE_KEY = "engineOn";
        protected const string MU_CONNECTED_FRONT_KEY = "muF";
        protected const string MU_CONNECTED_REAR_KEY = "muR";

        public override void Initialize(CustomLocoSimDiesel sim, DamageControllerCustomDiesel dmg, CustomLocoControllerDiesel ctrl, CarVisitChecker checker)
        {
            base.Initialize(sim, dmg, ctrl, checker);
			muModule = ctrl.muModule;
        }

        public override JObject GetLocoStateSaveData()
        {
            var data = base.GetLocoStateSaveData();
            data.SetBool(ENGINE_ON_SAVE_KEY, locoSim.engineOn);

            if (muModule.frontCableAdapter.IsConnected)
            {
                data.SetBool(MU_CONNECTED_FRONT_KEY, true);
            }
            if (muModule.rearCableAdapter.IsConnected)
            {
                data.SetBool(MU_CONNECTED_REAR_KEY, true);
            }

			return data;
        }

        public override void SetLocoStateSaveData(JObject saveData)
        {
            base.SetLocoStateSaveData(saveData);

            controller.EngineRunning = saveData.GetBool(ENGINE_ON_SAVE_KEY) ?? false;

            bool muFront = saveData.GetBool(MU_CONNECTED_FRONT_KEY) ?? false;
            bool muRear = saveData.GetBool(MU_CONNECTED_REAR_KEY) ?? false;
            if (muFront || muRear)
            {
                muModule.MultipleUnitStateRestoreOnGameLoad(muFront, muRear);
            }
        }
    }
}
