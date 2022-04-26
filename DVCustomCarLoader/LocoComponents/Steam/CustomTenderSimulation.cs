using CCL_GameScripts;
using DV;
using DV.ServicePenalty;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomTenderSimulation : MonoBehaviour, IServicePenaltyProvider
    {
        public ResourceType FuelType;

        public float WaterCapacityL;
        public float FuelCapacity;

        private TrainCar train;
        private CustomTenderDebtTracker tenderDebt;

        public SimComponent tenderWater;
        public SimComponent tenderFuel;

        protected void Awake()
        {
            train = GetComponent<TrainCar>();
            var visitChecker = gameObject.AddComponent<CarVisitChecker>();
            visitChecker.Initialize(train);

            tenderWater = new SimComponent("TenderWater", 0, WaterCapacityL, 4500, WaterCapacityL);
            tenderFuel = new SimComponent("TenderFuel", 0, FuelCapacity, 300, FuelCapacity);

            gameObject.AddComponent<CustomTenderSaveState>().Initialize(this, visitChecker);
            train.LogicCarInitialized += OnLogicCarInitialized;
        }

        private void OnLogicCarInitialized()
        {
            train.LogicCarInitialized -= OnLogicCarInitialized;
            CarDamageModel component = GetComponent<CarDamageModel>();
            if (!train.playerSpawnedCar || Main.Settings.FeesForCCLLocos)
            {
                tenderDebt = new CustomTenderDebtTracker(this, component, train.ID, train.carType);
                SingletonBehaviour<LocoDebtController>.Instance.RegisterLocoDebtTracker(tenderDebt);
            }
            train.OnDestroyCar += OnLocoDestroyed;
            gameObject.AddComponent<CustomLocoPitStopParams>().Initialize(this);
        }

        private void OnLocoDestroyed(TrainCar _)
        {
            train.OnDestroyCar -= OnLocoDestroyed;
            if ((!train.playerSpawnedCar || Main.Settings.FeesForCCLLocos) && (tenderDebt != null))
            {
                SingletonBehaviour<LocoDebtController>.Instance.StageLocoDebtOnLocoDestroy(tenderDebt);
            }
        }

        public void ResetRefillableSimulationParams()
        {
            tenderWater.SetValue(tenderWater.max);
            tenderFuel.SetValue(tenderFuel.max);
        }

        public IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            throw new NotImplementedException();
        }

        public void ResetDebt(DebtComponent debt)
        {
            throw new NotImplementedException();
        }

        public void UpdateDebtValue(DebtComponent debt)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            return new[]
            {
                new PitStopRefillable(this, ResourceType.Water, tenderWater),
                new PitStopRefillable(this, FuelType, tenderFuel)
            };
        }

        public float GetPitStopLevel(ResourceType type)
        {
            if (type == FuelType)
            {
                return tenderFuel.value;
            }
            else if (type == ResourceType.Water)
            {
                return tenderWater.value;
            }
            Main.Warning("Tried to get pit stop value this loco sim doesn't have");
            return 0;
        }

        public void ChangePitStopLevel(ResourceType type, float changeAmount)
        {
            if (type == FuelType)
            {
                tenderFuel.AddValue(changeAmount);
            }
            else if (type == ResourceType.Water)
            {
                tenderWater.AddValue(changeAmount);
            }
            Main.Warning("Trying to refill/repair something that is not part of this loco");
        }

        #region Save Data

        public JObject GetComponentsSaveData()
        {
            var simData = new JObject();
            SimComponent.SaveComponentState(tenderFuel, simData);
            SimComponent.SaveComponentState(tenderWater, simData);
            return simData;
        }

        public void LoadComponentsState(JObject stateData)
        {
            SimComponent.LoadComponentState(tenderFuel, stateData);
            SimComponent.LoadComponentState(tenderWater, stateData);
        }

        #endregion
    }
}
