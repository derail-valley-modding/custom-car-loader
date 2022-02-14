using System.Collections;
using System.Linq;
using DV.Logic.Job;
using DV.ServicePenalty;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class DebtTrackerCustomLoco : LocoDebtTrackerBase
    {
        private CustomLocoController locoController;
        private DamageControllerCustomLoco damageController;
        private CustomLocoSimulation simulation;
        private DebtTrackingInfo[] debtTrackers;

        public DebtTrackerCustomLoco(
            string id, TrainCarType carType,
            CustomLocoController ctrl,
            DamageControllerCustomLoco dmg, CustomLocoSimulation sim )
        {
            locoController = ctrl;
            damageController = dmg;
            simulation = sim;

            debtTrackers =
                damageController.GetDebtComponents()
                .Concat(simulation.GetDebtComponents())
                .ToArray();

            debtData = new CarDebtData(id, carType, InitializeDebtComponents(), CargoType.None);
        }

        public override DebtComponent[] InitializeDebtComponents()
        {
            return debtTrackers.Select(dt => dt.Debt).ToArray();
        }

        public override bool IsDebtOnlyEnvironmental()
        {
            bool foundEnviron = false;
            bool foundOther = false;

            foreach( var debt in debtData.GetTrackedDebts() )
            {
                if( debt.StartToEndDiff <= 0f ) continue;

                if( debt.type.IsEnvironmental() ) foundEnviron = true;
                else foundOther = true;
            }

            return foundEnviron && !foundOther;
        }

        public override void ResetState()
        {
            TurnOffDebtSources();
            damageController.RepairAll();
            simulation.ResetRefillableSimulationParams();
            simulation.ResetFuelConsumption();

            foreach( var tracker in debtTrackers )
            {
                tracker.ResetDebt();
            }
        }

        public override void TurnOffDebtSources()
        {
            locoController?.SetNeutralState();
        }

        public override void UpdateDebtValues()
        {
            foreach( var tracker in debtTrackers )
            {
                tracker.UpdateDebtValue();
            }
        }
    }
}