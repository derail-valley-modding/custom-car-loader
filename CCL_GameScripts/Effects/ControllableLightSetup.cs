using System.Collections;
using CCL_GameScripts.Attributes;
using CCL_GameScripts.CabControls;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class ControllableLightSetup : IndicatorSetupBase
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.ControllableLight";
        public override bool DestroyAfterCreation => true;

        [ProxyField]
        public Light[] Lights;

        [ProxyField]
        public float MinLevel = 0;
        [ProxyField]
        public float MaxLevel = 1;

        [ProxyField]
        public float Lag = 0.05f;
    }
}