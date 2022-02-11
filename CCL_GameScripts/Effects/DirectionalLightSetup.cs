using System;
using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    [Obsolete("Directional lights are now part of base loco controller")]
    public class DirectionalLightSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.DirectionalLightController";
        public override bool DestroyAfterCreation => true;
    }
}