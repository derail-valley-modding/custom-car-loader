using CCL_GameScripts.CabControls;
using DV;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Utility
{
    public class BrakeCarController : MonoBehaviour, ICabControlAcceptor
    {
        public bool HasAutomaticBrakeHandle = false;
        public bool HasRemoteRangeExtender = false;

        protected TrainCar car;
        protected float targetIndependentBrake;

        protected float targetTrainBrake;


        protected virtual void Awake()
        {
            car = GetComponent<TrainCar>();
            if (car == null)
            {
                Main.Error("TrainCar not attached to BrakeCarController! Destroying self.");
                Destroy(this);
            }
        }

        protected virtual void Start()
        {
            car.brakeSystem.hasIndependentBrake = true;
            car.CarDamage.IgnoreDamage(true);

            if (HasRemoteRangeExtender)
            {
                gameObject.AddComponent<RemoteControllerSignalBooster>();
            }

            if (!VRManager.IsVREnabled())
            {
                var keyboardInput = gameObject.AddComponent<BrakeCarKeyboardInputRouter>();
                keyboardInput.Ctrl = this;
            }
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
            if (!HasAutomaticBrakeHandle) return;

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
            switch (inputType)
            {
                case CabInputType.IndependentBrake:
                    return true;

                case CabInputType.TrainBrake:
                    return HasAutomaticBrakeHandle;

                default:
                    return false;
            }
        }

        public void RegisterControl(CabInputRelay controlRelay)
        {
            switch (controlRelay.Binding)
            {
                case CabInputType.IndependentBrake:
                    controlRelay.SetIOHandlers(SetIndependentBrake, GetTargetIndependentBrake);
                    break;

                case CabInputType.TrainBrake:
                    if (HasAutomaticBrakeHandle)
                    {
                        controlRelay.SetIOHandlers(SetTrainBrake, GetTargetTrainBrake);
                    }
                    break;
            }
        }
    }
}