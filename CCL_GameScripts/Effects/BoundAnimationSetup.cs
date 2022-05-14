using CCL_GameScripts.Attributes;
using CCL_GameScripts.CabControls;
using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class BoundAnimationSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.BoundAnimation";
        public override bool DestroyAfterCreation => true;

        [ProxyField]
        public Animator Animator;
        [ProxyField]
        public SimEventType[] EventBindings;
        [ProxyField]
        public CabInputType[] InputBindings;
    }
}