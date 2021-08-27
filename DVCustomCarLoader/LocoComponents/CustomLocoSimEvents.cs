using System.Collections;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimEvents : LocoSimulationEvents, ILocoEventProvider
    {
        protected const float FUEL_OIL_DMG_CHECK_PERIOD = 5f;

        protected virtual void Start()
        {
            InitThresholds();
        }

        protected abstract void InitThresholds();

        protected virtual void OnEnable()
        {
            StartCoroutine(CheckTankDamageStateRoutine());
        }

        /// <summary>Gets an event_&lt;T&gt; from the controller</summary>
        public virtual SimEventWrapper GetEvent( SimEventType indicatorType )
        {
            switch( indicatorType )
            {
                case SimEventType.Fuel:
                    return FuelChanged;

                case SimEventType.Oil:
                    return OilChanged;

                case SimEventType.Sand:
                    return SandChanged;

                case SimEventType.Wheelslip:
                    return WheelslipChanged;

                case SimEventType.Couplers:
                    return CouplingIntegrityChanged;

                default:
                    return SimEventWrapper.Empty;
            }
        }

        private IEnumerator CheckTankDamageStateRoutine()
        {
            WaitForSeconds waitTimeout = WaitFor.Seconds(FUEL_OIL_DMG_CHECK_PERIOD);

            while( true )
            {
                yield return waitTimeout;
                CheckTankAndDamageLevels();
            }
        }

        protected abstract void CheckTankAndDamageLevels();
    }

    public abstract class CustomLocoSimEvents<TSim,TDmg> : CustomLocoSimEvents
        where TSim : CustomLocoSimulation
        where TDmg : DamageControllerCustomLoco
    {
        protected TDmg dmgController;
        protected TSim sim;

        protected override void Awake()
        {
            base.Awake();
            sim = GetComponent<TSim>();
            dmgController = GetComponent<TDmg>();
        }
    }
}