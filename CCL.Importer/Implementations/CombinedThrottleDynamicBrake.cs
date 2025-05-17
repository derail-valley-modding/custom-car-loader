using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class CombinedThrottleDynamicBrake : SimComponent
    {
        public readonly bool UseToggleMode;
        public readonly Port ThrottleIn;
        public readonly Port DynBrakeIn;
        public readonly Port CombinedIn;
        public readonly Port SelectorIn;
        public readonly Port CurrentMode;

        private int _mode;

        public CombinedThrottleDynamicBrake(CombinedThrottleDynamicBrakeDefinitionInternal def) : base(def.ID)
        {
            UseToggleMode = def.UseToggleMode;

            ThrottleIn = AddPort(def.ThrottleIn);
            DynBrakeIn = AddPort(def.DynBrakeIn);
            CombinedIn = AddPort(def.CombinedIn);
            SelectorIn = AddPort(def.SelectorIn, 0.5f);
            CurrentMode = AddPort(def.CurrentMode);

            ThrottleIn.ValueUpdatedInternally += ThrottleUpdated;
            DynBrakeIn.ValueUpdatedInternally += DynBrakeUpdated;
            CombinedIn.ValueUpdatedInternally += CombinedUpdated;
            SelectorIn.ValueUpdatedInternally += SelectorUpdated;
            CurrentMode.ValueUpdatedInternally += ModeUpdated;

            _mode = 0;
        }

        public override void Tick(float delta) { }

        private void ThrottleUpdated(float value)
        {
            if (_mode == 1)
            {
                CombinedIn.Value = value;
            }
        }

        private void DynBrakeUpdated(float value)
        {
            if (_mode == -1)
            {
                CombinedIn.Value = value;
            }
        }

        private void CombinedUpdated(float value)
        {
            switch (_mode)
            {
                case 1:
                    ThrottleIn.Value = value;
                    DynBrakeIn.Value = 0;
                    break;
                case -1:
                    ThrottleIn.Value = 0;
                    DynBrakeIn.Value = value;
                    break;
                default:
                    ThrottleIn.Value = 0;
                    DynBrakeIn.Value = 0;
                    break;
            }
        }

        private void SelectorUpdated(float value)
        {
            if (UseToggleMode)
            {
                // Toggle goes up when set forwards, then it goes back to neutral.
                // Same for the opposite direction, and clamp to [-1..1].
                if (value > 0.75f)
                {
                    _mode++;

                    if (_mode > 1)
                    {
                        _mode = 1;
                    }
                }
                else if (value < 0.25f)
                {
                    _mode--;

                    if (_mode < -1)
                    {
                        _mode = -1;
                    }
                }
            }
            else
            {
                if (value > 0.75f)
                {
                    _mode = 1;
                }
                else if (value < 0.25f)
                {
                    _mode = -1;
                }
                else
                {
                    _mode = 0;
                }
            }

            CurrentMode.Value = _mode;
        }

        private void ModeUpdated(float value)
        {
            int mode = Mathf.RoundToInt(Mathf.Clamp(value, -1, 1));

            if (mode == _mode) return;

            _mode = mode;

            if (!UseToggleMode)
            {
                SelectorIn.Value = _mode + 1.0f / 2.0f;
            }
        }
    }
}
