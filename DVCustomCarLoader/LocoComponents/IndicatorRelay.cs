using CCL_GameScripts.CabControls;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class IndicatorRelay : MonoBehaviour, ILocoEventAcceptor, ICabControlAcceptor
    {
        public SimEventType EventBinding;
        public CabInputType ControlBinding;

        public SimEventType[] EventTypes => new[] { EventBinding };
        public Indicator Indicator;

        public bool IsBoundToInterior { get; set; }

        public void Initialize(OutputBinding binding, Indicator indicator)
        {
            EventBinding = binding.SimEventType;
            ControlBinding = binding.CabInputType;
            Indicator = indicator;
        }

        [InitSpecAfterInit(typeof(IndicatorSetupBase))]
        public static void FinalizeIndicatorSetup(IndicatorSetupBase spec, Indicator realComp)
        {
            var spawnedObj = realComp.gameObject;
            var indicatorInfo = spawnedObj.AddComponent<IndicatorRelay>();
            indicatorInfo.Initialize(spec.Binding, realComp);
        }

        protected float target = 0;

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float value)
            {
                target = value;

                if (Indicator)
                {
                    Indicator.value = value;
                }
            }
        }

        protected void Update()
        {
            if (Indicator is IndicatorEmission)
            {
                Indicator.value = target;
            }
        }

        protected void HandleCabControlUpdated(object sender, float newValue)
        {
            target = newValue;
            if (Indicator)
            {
                Indicator.value = newValue;
            }
        }

        public void RegisterControl(CabInputRelay controlRelay)
        {
            controlRelay.AddListener(HandleCabControlUpdated);
        }

        public bool AcceptsControlOfType(CabInputType inputType)
        {
            return (ControlBinding != CabInputType.None) && (inputType == ControlBinding);
        }
    }
}
