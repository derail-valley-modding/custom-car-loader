using CCL_GameScripts;
using System.Collections.Generic;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Utility
{
    public class DummySegmentController : MonoBehaviour, ILocoEventProvider, IProvidesDriver
    {
        public TrainCar train;
        public string[] MainUnitIds;
        private readonly List<TrainCarType> _mainUnitTypes = new();

        public CarDirection AutoCoupleSide;

        public CustomLocoController LinkedLoco { get; private set; }

        private BridgedEventManager _bridgedEventManager;
        protected BridgedEventManager BridgedEventManager
        {
            get => _bridgedEventManager;
            set
            {
                _bridgedEventManager = value;
                UpdateWatchedLoco();
            }
        }

        public LocoEventManager EventManager
        {
            get => _bridgedEventManager;
            set => _bridgedEventManager = value as BridgedEventManager;
        }

        public IEnumerable<WatchableValue> Watchables => null;

        public float reverser => LinkedLoco ? LinkedLoco.reverser : 0f;
        public float wheelslip => LinkedLoco ? LinkedLoco.drivingForce.wheelslip : 0f;

        public void ForceDispatchAll() { }

        private void Awake()
        {
            train = GetComponent<TrainCar>();
            train.TrainsetChanged += UpdateWatchedLoco;

            GetMainUnitTypes();

            if (_mainUnitTypes.Count > 0)
            {
                gameObject.AddComponent<DummyAutoCoupler>();
            }
        }

        private void GetMainUnitTypes()
        {
            foreach (var id in MainUnitIds)
            {
                if (CarTypeInjector.TryGetCarTypeById(id, out var carType))
                {
                    _mainUnitTypes.Add(carType);
                }
                else
                {
                    Main.Warning($"Couldn't find main car type \"{id}\" for dummy segment {train.carType.DisplayName()}");
                }
            }
        }

        public bool ConnectsToCar(TrainCarType carType)
        {
            return _mainUnitTypes.Contains(carType);
        }

        private void UpdateWatchedLoco(Trainset _ = null)
        {
            var coupled = train.frontCoupler.coupledTo;
            if (coupled && ConnectsToCar(coupled.train.carType))
            {
                var manager = coupled.train.GetComponent<LocoEventManager>();
                if (manager)
                {
                    _bridgedEventManager.LinkedManager = manager;
                }
                else
                {
                    Main.Warning($"Main car {coupled.train.carType.DisplayName()} is missing LocoEventManager");
                }
            }

            coupled = train.rearCoupler.coupledTo;
            if (coupled && ConnectsToCar(coupled.train.carType))
            {
                var manager = coupled.train.GetComponent<LocoEventManager>();
                if (manager)
                {
                    _bridgedEventManager.LinkedManager = manager;
                }
                else
                {
                    Main.Warning($"Main car {coupled.train.carType.DisplayName()} is missing LocoEventManager");
                }
            }
        }
    }
}