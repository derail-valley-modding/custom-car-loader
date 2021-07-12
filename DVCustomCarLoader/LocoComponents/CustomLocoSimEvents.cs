using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimEvents : LocoSimulationEvents
    {

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