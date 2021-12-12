using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class DirectionalLightSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.DirectionalLightController";
        public override bool DestroyAfterCreation => true;
    }
}