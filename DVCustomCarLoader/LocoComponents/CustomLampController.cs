using System;
using System.Collections;
using CCL_GameScripts.CabControls;
using DV.Util.EventWrapper;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomLampController : MonoBehaviour
    {
        protected CustomLocoSimEvents simEvents;
        public DashboardLampRelay[] Relays;

        protected virtual void OnEnable()
        {
            var car = TrainCar.Resolve(gameObject);
            if( car == null || !car )
            {
                Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
                return;
            }

            simEvents = car.gameObject.GetComponent<CustomLocoSimEvents>();
            if( !simEvents )
            {
                Main.Error("Couldn't find custom simEvents for lamp controller");
                return;
            }

            Relays = GetComponentsInChildren<DashboardLampRelay>();

            Main.Log($"CustomDashboardLamps Start - {Relays.Length} lamps");
            foreach( var lampController in Relays )
            {
                object changeEvent = simEvents.GetEvent(lampController.SimBinding);
                if( changeEvent != null )
                {
                    lampController.SetupListener(changeEvent);
                }
            }
        }
    }

    public class DashboardLampRelay : MonoBehaviour
    {
        protected static AudioClip WarningSound;

        public SimEventType SimBinding;
        public LampControl Lamp;

        public SimThresholdDirection ThresholdDirection;
        public LocoSimulationEvents.Amount SolidThreshold;
        public bool UseBlinkMode;
        public LocoSimulationEvents.Amount BlinkThreshold;

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
                WarningSound.Play(Lamp.transform.position, 1f, 1f, 0f, 1f, 500f, default(AudioSourceCurves), AudioManager.e.cabGroup, Lamp.transform);
            }

            Lamp.SetLampState(state);
        }

        public void SetupListener( object toAttach )
        {
            if( toAttach is event_<LocoSimulationEvents.Amount> amountEvent )
            {
                amountEvent.Manage(OnAmountChanged, true);
            }
            else if( toAttach is event_<LocoSimulationEvents.CouplingIntegrityInfo> coupleEvent )
            {
                coupleEvent.Manage(OnCouplingChanged, true);
            }
            else if( toAttach is event_<bool> boolEvent )
            {
                boolEvent.Manage(OnBoolChanged, true);
            }
            else
            {
                Main.Warning($"DashboardLampRelay - unknown sim event type {toAttach.GetType().Name}");
            }
        }
    }
}