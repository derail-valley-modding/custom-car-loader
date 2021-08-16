using System.Collections;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimEvents : LocoSimulationEvents
    {
        /// <summary>Gets an event_&lt;T&gt; from the controller</summary>
        public virtual object GetEvent( SimEventType indicatorType )
        {
            switch( indicatorType )
            {
                case SimEventType.Fuel:
                    return FuelChanged;

                case SimEventType.Oil:
                    return OilChanged;

                case SimEventType.Sand:
                    return SandChanged;

                case SimEventType.EngineTemp:
                    return EngineTempChanged;

                case SimEventType.EngineDamage:
                    return EngineDamageChanged;

                case SimEventType.Wheelslip:
                    return WheelslipChanged;

                case SimEventType.Couplers:
                    return CouplingIntegrityChanged;

                default:
                    return null;
            }
        }
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

        protected virtual void Start()
        {
            InitThresholds();
        }

        protected abstract void InitThresholds();
    }
}