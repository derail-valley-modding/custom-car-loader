using System.Collections;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimEvents : LocoSimulationEvents, ILocoEventProvider
    {
        public LocoEventManager EventManager { get; set; }
        protected const float FUEL_OIL_DMG_CHECK_PERIOD = 5f;

        protected LocoEventWrapper<Amount> SandEvent;
        protected LocoEventWrapper<bool> WheelslipEvent;
        protected LocoEventWrapper<CouplingIntegrityInfo> CoupleEvent;

        protected CustomLocoSimEvents()
        {
            SandEvent = LocoEventWrapper<Amount>.Create(ref SandChanged, this, SimEventType.Sand);
            WheelslipEvent = LocoEventWrapper<bool>.Create(ref WheelslipChanged, this, SimEventType.Wheelslip);
            CoupleEvent = LocoEventWrapper<CouplingIntegrityInfo>.Create(ref CouplingIntegrityChanged, this, SimEventType.Couplers);
        }

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

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
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