using DV.ServicePenalty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomTenderDebtTracker : LocoDebtTrackerBase
    {
        private CustomTenderSimulation sim;
        private CarDamageModel carDmg;

        public CustomTenderDebtTracker(CustomTenderSimulation tenderSim, CarDamageModel damageModel, string id, TrainCarType carType)
        {
            sim = tenderSim;
            carDmg = damageModel;
            debtData = new CarDebtData(id, carType, InitializeDebtComponents());
        }

        public override DebtComponent[] InitializeDebtComponents()
        {
            return new[]
            {
                new DebtComponent(carDmg.EffectiveHealthPercentage100Notation, ResourceType.Car_DMG),
                new DebtComponent(sim.tenderFuel.value, sim.FuelType),
                new DebtComponent(sim.tenderWater.value, ResourceType.Water)
            };
        }

        public override void UpdateDebtValues()
        {
            DebtComponent[] trackedDebts = debtData.GetTrackedDebts();
            trackedDebts[0].UpdateEndValue(carDmg.EffectiveHealthPercentage100Notation);
            trackedDebts[1].UpdateEndValue(sim.tenderFuel.value);
            trackedDebts[2].UpdateEndValue(sim.tenderWater.value);
        }

        public override void ResetState()
        {
            TurnOffDebtSources();
            sim.ResetRefillableSimulationParams();
            carDmg.RepairCar(carDmg.maxHealth - carDmg.currentHealth);
            DebtComponent[] trackedDebts = debtData.GetTrackedDebts();
            trackedDebts[0].ResetComponent(carDmg.EffectiveHealthPercentage100Notation);
            trackedDebts[1].ResetComponent(sim.tenderFuel.value);
            trackedDebts[2].ResetComponent(sim.tenderWater.value);
        }

        public override void TurnOffDebtSources()
        {
            TrainCar trainCar = carDmg.trainCar;
            if (trainCar.frontCoupler.IsCoupled())
            {
                var component = trainCar.frontCoupler.coupledTo.train.GetComponent<LocoControllerBase>();
                if (component is LocoControllerSteam baseSteamCtrl)
                {
                    baseSteamCtrl.SetInjector(0);
                }
                else if (component is CustomLocoControllerSteam customSteamCtrl)
                {
                    customSteamCtrl.SetInjector(0);
                }
            }
        }

        public override bool IsDebtOnlyEnvironmental() => false;
    }
}
