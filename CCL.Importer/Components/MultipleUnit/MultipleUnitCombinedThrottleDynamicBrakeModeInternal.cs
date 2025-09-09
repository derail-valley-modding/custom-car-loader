using LocoSim.Attributes;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Components.MultipleUnit
{
    public class MultipleUnitCombinedThrottleDynamicBrakeModeInternal :
        MultipleUnitExtraControlInternal<MultipleUnitCombinedThrottleDynamicBrakeModeInternal>
    {
        [PortId]
        public string ModePortId = string.Empty;

        private Port _modePort = null!;

        public override void Init(TrainCar car, SimulationFlow simFlow)
        {
            base.Init(car, simFlow);

            if (!simFlow.TryGetPort(ModePortId, out _modePort, false))
            {
                Debug.LogError($"(MultipleUnitCombinedThrottleDynamicBrakeMode) Could not find mode port!", this);
                Destroy(this);
                return;
            }

            _modePort.ValueUpdatedInternally += (x) => ValueChanged();
        }

        public override void SetValue(MultipleUnitCombinedThrottleDynamicBrakeModeInternal source)
        {
            _modePort.Value = source._modePort.Value;
        }

        protected override void ConnectionChanged(bool connected, bool playAudio)
        {
            _modePort.Value = 0;
        }
    }
}
