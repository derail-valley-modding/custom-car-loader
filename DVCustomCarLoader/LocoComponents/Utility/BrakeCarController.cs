using CCL_GameScripts.CabControls;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Utility
{
    public class BrakeCarController : MonoBehaviour, ILocoEventProvider, ICabControlAcceptor
    {
        public LocoEventManager EventManager { get; set; }

        public bool HasRemoteRangeExtender = false;
        protected RemoteControllerSignalBooster signalBooster = null;

        protected TrainCar car;
        protected float targetIndependentBrake;

        protected float targetTrainBrake;

        protected const float MAX_BOOSTER_RANGE = 2000;

        protected float signalBoosterLevel = 1;
        public void SetSignalBoosterLevel(float level)
        {
            signalBoosterLevel = level;
            if (signalBooster)
            {
                signalBooster.range = Mathf.Lerp(0, MAX_BOOSTER_RANGE, signalBoosterLevel);
            }
        }
        public float GetSignalBoosterLevel() => signalBoosterLevel;

        public IEnumerable<WatchableValue> Watchables { get; protected set; }

        protected virtual void Awake()
        {
            car = GetComponent<TrainCar>();
            if (car == null)
            {
                Main.Error("TrainCar not attached to BrakeCarController! Destroying self.");
                Destroy(this);
            }

            Watchables = new[]
            {
                new WatchableValue<float>(this, SimEventType.BrakePipe, () => car.brakeSystem.brakePipePressure),
                new WatchableValue<float>(this, SimEventType.SignalBoosterPower, GetSignalBoosterLevel),
            };
        }

        protected virtual void Start()
        {
            car.brakeSystem.hasIndependentBrake = true;
            car.brakeSystem.hasCompressor = false;
            car.brakeSystem.compressorProductionRate = 0;
            car.CarDamage.IgnoreDamage(true);

            if (HasRemoteRangeExtender)
            {
                signalBooster = gameObject.AddComponent<RemoteControllerSignalBooster>();
            }

            if (!VRManager.IsVREnabled())
            {
                var keyboardInput = gameObject.AddComponent<BrakeCarKeyboardInputRouter>();
                keyboardInput.Ctrl = this;
            }

            StartCoroutine(EngageBrakesOnStart());
        }

        private bool IsConnectedToAny()
        {
            return car.trainset.cars.Count > 1;
        }

        private bool IsConnectedToLoco()
        {
            return car.trainset.locoIndices.Any();
        }

        private IEnumerator EngageBrakesOnStart()
        {
            SetIndependentBrake(1f);
            yield return WaitFor.Seconds(1.5f);

            // leave just independent for lone caboose
            if (!IsConnectedToAny()) yield break;

            SetIndependentBrake(0f);

            // allow loco to take brake control
            float trainBrakeLevel = IsConnectedToLoco() ? 0f : 1f;
            SetTrainBrake(trainBrakeLevel);

            yield break;
        }

        public void SetIndependentBrake(float newTarget)
        {
            if (targetIndependentBrake != newTarget)
            {
                if (car.isEligibleForSleep)
                {
                    car.ForceOptimizationState(false, false);
                }
                targetIndependentBrake = Mathf.Clamp01(newTarget);
            }

            car.brakeSystem.independentBrakePosition = targetIndependentBrake;
        }

        public float GetTargetIndependentBrake() => targetIndependentBrake;
        
        public void SetTrainBrake(float newTarget)
        {
            if (targetTrainBrake != newTarget)
            {
                if (car.isEligibleForSleep)
                {
                    car.ForceOptimizationState(false, false);
                }
                targetTrainBrake = Mathf.Clamp01(newTarget);
            }

            car.brakeSystem.trainBrakePosition = targetTrainBrake;
        }

        public float GetTargetTrainBrake() => targetTrainBrake;

        public bool AcceptsControlOfType(CabInputType inputType)
        {
            return inputType.EqualsOneOf(CabInputType.IndependentBrake, CabInputType.TrainBrake, CabInputType.SignalBoosterPower);
        }

        public void RegisterControl(CabInputRelay controlRelay)
        {
            switch (controlRelay.Binding)
            {
                case CabInputType.IndependentBrake:
                    controlRelay.SetIOHandlers(SetIndependentBrake, GetTargetIndependentBrake);
                    break;

                case CabInputType.TrainBrake:
                    controlRelay.SetIOHandlers(SetTrainBrake, GetTargetTrainBrake);
                    break;

                case CabInputType.SignalBoosterPower:
                    controlRelay.SetIOHandlers(SetSignalBoosterLevel, GetSignalBoosterLevel);
                    break;
            }
        }

        public void ForceDispatchAll() { }
    }
}