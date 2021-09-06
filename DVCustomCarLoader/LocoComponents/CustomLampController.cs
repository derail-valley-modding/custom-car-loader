using System;
using System.Collections;
using System.Linq;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLampController : MonoBehaviour
    {
        protected ILocoEventProvider[] eventProviders;
        public DashboardLampRelay[] Relays;

        protected virtual void Start()
        {
            var car = TrainCar.Resolve(gameObject);
            if( car == null || !car )
            {
                Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
                return;
            }

            eventProviders = 
                car.gameObject.GetComponentsByInterface<ILocoEventProvider>()
                .Concat(gameObject.GetComponentsByInterface<ILocoEventProvider>())
                .ToArray();

            if( eventProviders.Length == 0 )
            {
                Main.Error("Couldn't find any event providers for lamp controller");
                return;
            }

            Relays = GetComponentsInChildren<DashboardLampRelay>(true);

            Main.Log($"CustomDashboardLamps Start - {Relays.Length} lamps");
            foreach( var lampController in Relays )
            {
                // search for a loco component/event to bind to
                foreach( var provider in eventProviders )
                {
                    if( provider.Bind(lampController.SimBinding, lampController) )
                    {
#if DEBUG
                        Main.Log($"Bind {lampController.name} to {((MonoBehaviour)provider).name}");
#endif
                        break;
                    }
                }
            }
        }
    }

    public class DashboardLampRelay : MonoBehaviour, ILocoEventAcceptor
    {
        protected static AudioClip WarningSound;

        public SimEventType SimBinding;
        public LampControl Lamp;

        public SimThresholdDirection ThresholdDirection;
        public LocoSimulationEvents.Amount SolidThreshold;
        public bool UseBlinkMode;
        public LocoSimulationEvents.Amount BlinkThreshold;

        public Action<LocoSimulationEvents.Amount> AmountHandler => OnAmountChanged;
        private void OnAmountChanged( LocoSimulationEvents.Amount amount )
        {
            if( ThresholdDirection == SimThresholdDirection.Above )
            {
                if( UseBlinkMode && (amount >= BlinkThreshold) )
                {
                    UpdateLampState(LampControl.LampState.Blinking, true);
                }
                else if( amount >= SolidThreshold )
                {
                    UpdateLampState(LampControl.LampState.On, true);
                }
                else
                {
                    UpdateLampState(LampControl.LampState.Off, false);
                }
            }
            else
            {
                // below threshold
                if( UseBlinkMode && (amount <= BlinkThreshold) )
                {
                    UpdateLampState(LampControl.LampState.Blinking, true);
                }
                else if( amount <= SolidThreshold )
                {
                    UpdateLampState(LampControl.LampState.On, true);
                }
                else
                {
                    UpdateLampState(LampControl.LampState.Off, false);
                }
            }
        }

        public Action<bool> BoolHandler => OnBoolChanged;
        private void OnBoolChanged( bool newValue )
        {
            // below, true -> false
            // below, false -> true
            // above, true -> true
            // above, false -> false

            if( (ThresholdDirection == SimThresholdDirection.Below) ^ newValue )
            {
                UpdateLampState(UseBlinkMode ? LampControl.LampState.Blinking : LampControl.LampState.On, true);
            }
            else
            {
                UpdateLampState(LampControl.LampState.Off, false);
            }
        }

        public Action<LocoSimulationEvents.CouplingIntegrityInfo> CouplingHandler => OnCouplingChanged;
        private void OnCouplingChanged( LocoSimulationEvents.CouplingIntegrityInfo integrityInfo )
        {
            LampControl.LampState state = (integrityInfo == LocoSimulationEvents.CouplingIntegrityInfo.OK) ? 
                LampControl.LampState.Off : 
                LampControl.LampState.Blinking;

            UpdateLampState(state, true);
        }

        private void UpdateLampState( LampControl.LampState state, bool playWarningSound = true )
        {
            if( playWarningSound && (state == LampControl.LampState.On || state == LampControl.LampState.Blinking) )
            {
                if( WarningSound )
                {
                    WarningSound.Play(Lamp.transform.position, 1f, 1f, 0f, 1f, 500f, default(AudioSourceCurves), AudioManager.e.cabGroup, Lamp.transform);
                }
            }

            Lamp.SetLampState(state);
        }
    }
}