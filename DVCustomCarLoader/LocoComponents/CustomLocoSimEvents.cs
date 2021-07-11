using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoSimEvents<TDmg,TSim> : LocoSimulationEvents
        where TDmg : DamageControllerCustomLoco
        where TSim : CustomLocoSimulation
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