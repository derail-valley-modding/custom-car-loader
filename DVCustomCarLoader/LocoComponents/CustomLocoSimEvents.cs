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
            StartCoroutine(CheckWheelslip(WHEELSLIP_CHECK_PERIOD));
            StartCoroutine(CheckCouplingIntegrity(COUPLING_INTEGRITY_CHECK_PERIOD));
        }

        /// <summary>Gets an event_&lt;T&gt; from the controller</summary>
        public virtual bool TryBind(ILocoEventAcceptor listener)
        {
            switch (listener.EventType)
            {
                case SimEventType.Fuel:
                    return listener.TryBindGeneric(FuelChanged);

                case SimEventType.Oil:
                    return listener.TryBindGeneric(OilChanged);

                case SimEventType.Sand:
                    return listener.TryBindGeneric(SandChanged);

                case SimEventType.Wheelslip:
                    return listener.TryBindGeneric(WheelslipChanged);

                case SimEventType.Couplers:
                    return listener.TryBindGeneric(CouplingIntegrityChanged);

                default:
                    return false;
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