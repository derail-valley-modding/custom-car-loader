using System;
using System.Collections;
using System.Linq;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class DashboardLampRelay : MonoBehaviour, ILocoEventAcceptor
    {
        protected static AudioClip WarningSound;

        public bool IsBoundToInterior { get; set; }
        public SimEventType EventType;
        public SimEventType[] EventTypes => new[] { EventType };

        public LampControl Lamp;
        public SimThresholdDirection ThresholdDirection;
        public LocoSimulationEvents.Amount SolidThreshold;
        public bool UseBlinkMode;
        public LocoSimulationEvents.Amount BlinkThreshold;

        [CopySpecAfterInit(typeof(CopiedLamp))]
        public static void FinalizeLampSetup(CopiedLamp spec, GameObject newObject)
        {
            var realLamp = newObject.GetComponentInChildren<LampControl>(true);
            var lampRelay = newObject.AddComponent<DashboardLampRelay>();
            lampRelay.EventType = spec.SimBinding;
            lampRelay.Lamp = realLamp;

            lampRelay.ThresholdDirection = spec.ThresholdDirection;
            lampRelay.SolidThreshold = (LocoSimulationEvents.Amount)spec.SolidThreshold;
            lampRelay.UseBlinkMode = spec.UseBlinkMode;
            lampRelay.BlinkThreshold = (LocoSimulationEvents.Amount)spec.BlinkThreshold;
        }

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is LocoSimulationEvents.Amount newAmount)
            {
                HandleChange(newAmount);
            }
            else if (eventInfo.NewValue is bool newBool)
            {
                HandleChange(newBool);
            }
            else if (eventInfo.NewValue is LocoSimulationEvents.CouplingIntegrityInfo newIntegrity)
            {
                HandleChange(newIntegrity);
            }
        }

        public void HandleChange(LocoSimulationEvents.Amount amount)
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

        public void HandleChange(bool newValue)
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

        public void HandleChange(LocoSimulationEvents.CouplingIntegrityInfo integrityInfo)
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