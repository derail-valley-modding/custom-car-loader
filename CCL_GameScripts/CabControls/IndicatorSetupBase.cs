using CCL_GameScripts.Attributes;

namespace CCL_GameScripts.CabControls
{
    public abstract class IndicatorSetupBase : ComponentInitSpec
    {
        public override bool DestroyAfterCreation => true;

        public SimEventType OutputBinding;

        [ProxyField("minValue")]
        public float MinValue = 0;
        [ProxyField("maxValue")]
        public float MaxValue = 1;
    }
}