using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public abstract class IndicatorSetupBase : ComponentInitSpec, IBoundIndicator
    {
        public override bool DestroyAfterCreation => true;

        public SimEventType OutputBinding;
        public CabInputType ControlBinding;

        public OutputBinding Binding => new OutputBinding(OutputBinding, ControlBinding);

        [ProxyField("minValue")]
        public float MinValue = 0;
        [ProxyField("maxValue")]
        public float MaxValue = 1;

        [SerializeField]
        [HideInInspector]
        private SimEventType _lastEventType;
        [SerializeField]
        [HideInInspector]
        private CabInputType _lastControlBinding;

        public void OnValidate()
        {
            if ((OutputBinding != SimEventType.None) && (OutputBinding != _lastEventType))
            {
                ControlBinding = CabInputType.None;
            }
            else if ((ControlBinding != CabInputType.None) && (ControlBinding != _lastControlBinding))
            {
                OutputBinding = SimEventType.None;
            }

            _lastEventType = OutputBinding;
            _lastControlBinding = ControlBinding;
        }
    }
}