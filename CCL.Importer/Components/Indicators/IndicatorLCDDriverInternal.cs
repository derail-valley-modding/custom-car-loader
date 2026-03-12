using LocoSim.Implementations;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Components.Indicators
{
    internal class IndicatorLCDDriverInternal : IndicatorWithModeAndNamesInternal
    {
        public LCDDriver LCD = null!;
        public bool PadLeft = true;
        public bool PadWithZeros = false;
        public string FuseId = string.Empty;

        private Fuse? _fuse;

        private bool PowerState => _fuse == null || _fuse.State;

        protected override void Start()
        {
            base.Start();

            if (!string.IsNullOrEmpty(FuseId))
            {
                var car = TrainCar.Resolve(gameObject);

                if (car == null)
                {
                    Debug.LogError("Couldn't find car, FuseId on IndicatorLCDDriver won't be functional!");
                    return;
                }

                var simController = car.SimController;
                var simulationFlow = (simController != null) ? simController.simFlow : null;

                if (simulationFlow == null)
                {
                    Debug.LogError("Couldn't find SimFlow, FuseId on IndicatorLCDDriver won't be functional!");
                    return;
                }
                if (!simulationFlow.TryGetFuse(FuseId, out _fuse))
                {
                    Debug.LogError("[" + gameObject.GetPath() + "]: IndicatorLCDDriver isn't initialized properly, fuse won't be set!");
                    return;
                }

                SetupPowerListeners(true);
                FuseUpdate(true);
            }
        }

        protected override void OnDestroy()
        {
            SetupPowerListeners(false);
            base.OnDestroy();
        }

        protected override void OnValueSet()
        {
            LCD.Display(PowerState ? PaddedText() : string.Empty);
        }

        private string PaddedText()
        {
            var text = GetDisplayText().ToUpperInvariant();

            if (PadLeft)
            {
                // Dots and colons are part of the digits so pad them.
                return text.PadLeft(LCD.numDigits + text.Count(c => c == '.') + text.Count(c => c == ':'), PadWithZeros ? '0' : ' ');
            }

            return text;
        }

        private void SetupPowerListeners(bool on)
        {
            if (_fuse == null) return;

            if (on)
            {
                _fuse.StateUpdated += FuseUpdate;
            }
            else
            {
                _fuse.StateUpdated -= FuseUpdate;
            }
        }

        private void FuseUpdate(bool state)
        {
            Value = Value;
        }
    }
}
