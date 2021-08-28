using System.Collections;
using UnityEngine;
using DV.Util.EventWrapper;
using CCL_GameScripts.CabControls;
using System;
using System.Collections.Generic;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomFuseController : MonoBehaviour, ILocoEventProvider, ICabControlAcceptor
    {
        protected IFusedLocoController locoController;

        public List<CabInputRelay> SideFuses { get; protected set; } = new List<CabInputRelay>();

        protected float mainFuseValue = 0f;
        public float GetMainFusePostion() => mainFuseValue;

        protected float starterValue = 0f;
        public float GetStarterPosition() => starterValue;

        protected const float SWITCH_THRESHOLD = 0.5f;
        protected const float LATE_INIT_DELAY = 0.5f;
        protected const float MAIN_BREAKER_DELAY = 0.2f;

        public event_<bool> MasterPowerChanged;

        protected bool AreAllSideFusesOn()
        {
            return SideFuses.TrueForAll(fuse => fuse.Value > SWITCH_THRESHOLD);
        }

        public void SetMasterPower( bool on )
        {
            float relayPos = on ? 1 : 0;

            foreach( CabInputRelay sideFuse in SideFuses )
            {
                sideFuse.Value = relayPos;
            }

            mainFuseValue = relayPos;

            MasterPowerChanged.Invoke(on);
            if( on )
            {
                TryStarter();
            }
            else
            {
                KillEngine();
            }
        }

        public void TryStarter()
        {
            if( locoController.CanEngineStart )
            {
                locoController.EngineRunning = true;
                // TODO: VRTK haptics
            }
        }

        public void KillEngine()
        {
            locoController.EngineRunning = false;
        }

        public void OnEnable()
        {
            var car = TrainCar.Resolve(gameObject);
            if( car == null || !car )
            {
                Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
                return;
            }

            locoController = car.gameObject.GetComponentByInterface<IFusedLocoController>();
            if( locoController == null )
            {
                Main.Error("Couldn't find loco controller for fuse box");
                return;
            }

            StartCoroutine(DelayedEnable());
        }

        private IEnumerator DelayedEnable()
        {
            yield return WaitFor.SecondsRealtime(LATE_INIT_DELAY);
            SetMasterPower(locoController.EngineRunning);
            yield break;
        }

        private IEnumerator DelayedMainFuseOff()
        {
            yield return WaitFor.SecondsRealtime(MAIN_BREAKER_DELAY);
            if( !AreAllSideFusesOn() )
            {
                mainFuseValue = 0;
                MasterPowerChanged.Invoke(false);
            }
            yield break;
        }

        //-------------------------------------------------------------------------------------
        #region ILocoEventProvider

        public SimEventWrapper GetEvent( SimEventType eventType )
        {
            if( eventType == SimEventType.PowerOn )
            {
                return MasterPowerChanged;
            }
            return SimEventWrapper.Empty;
        }

        #endregion

        //-------------------------------------------------------------------------------------
        #region ICabControlAcceptor

        protected void OnSideFuseChanged( float newVal )
        {
            if( newVal <= SWITCH_THRESHOLD )
            {
                if( locoController.EngineRunning ) KillEngine();
                StartCoroutine(DelayedMainFuseOff());
            }
        }

        protected void OnMainFuseChanged( float newVal )
        {
            bool wasOn = (mainFuseValue > SWITCH_THRESHOLD);
            bool nowOn = (newVal > SWITCH_THRESHOLD);

            if( nowOn && !wasOn )
            {
                if( !AreAllSideFusesOn() )
                {
                    StartCoroutine(DelayedMainFuseOff());
                    return;
                }
                MasterPowerChanged.Invoke(nowOn);
            }
            if( !nowOn && wasOn )
            {
                MasterPowerChanged.Invoke(nowOn);
            }
        }

        protected void OnStarterChanged( float newVal )
        {
            if( (newVal > SWITCH_THRESHOLD) && !locoController.EngineRunning )
            {
                TryStarter();
            }
        }

        protected void OnEStopChanged( float newVal )
        {
            if( (newVal > SWITCH_THRESHOLD) && locoController.EngineRunning )
            {
                KillEngine();
            }
        }

        public bool AcceptsControlOfType( CabInputType inputType )
        {
            return inputType.EqualsOneOf(
                CabInputType.Fuse,
                CabInputType.MainFuse,
                CabInputType.Starter,
                CabInputType.EngineStop
            );
        }

        public void RegisterControl( CabInputRelay inputRelay )
        {
            switch( inputRelay.Binding )
            {
                case CabInputType.Fuse:
                    SideFuses.Add(inputRelay);
                    inputRelay.SetIOHandlers(OnSideFuseChanged);
                    break;

                case CabInputType.MainFuse:
                    inputRelay.SetIOHandlers(OnMainFuseChanged, GetMainFusePostion);
                    break;

                case CabInputType.Starter:
                    inputRelay.SetIOHandlers(OnStarterChanged, GetStarterPosition);
                    break;

                case CabInputType.EngineStop:
                    inputRelay.SetIOHandlers(OnEStopChanged, null);
                    break;
            }
        }

        #endregion
    }
}