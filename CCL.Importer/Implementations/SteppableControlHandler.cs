using CCL.Importer.Components.Simulation;
using CCL.Types;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class SteppableControlHandler : SimComponent
    {
        private enum ModeEnum
        {
            SetTo0,
            StepDown,
            Stop,
            StepUp,
            FastStep
        }

        private const float NotchValue = 1.0f / 4.0f;
        private const float Bound01 = NotchValue / 2.0f;
        private const float Bound12 = Bound01 + NotchValue;
        private const float Bound23 = Bound12 + NotchValue;
        private const float Bound34 = Bound23 + NotchValue;

        public readonly float Step;
        public readonly float FastStepTime;
        public readonly Port ControlIn;
        public readonly PortReference OtherControlIn;

        private ModeEnum _mode;
        private ModeEnum _prevMode;
        private float _lastValue;
        private float _time = 0;
        private bool _lastSet = false;

        private bool Changed => _prevMode != _mode;

        public SteppableControlHandler(SteppableControlHandlerDefinitionInternal def) : base(def.ID)
        {
            Step = def.Step;
            FastStepTime = def.FastStepTime;

            ControlIn = AddPort(def.ControlIn);
            OtherControlIn = AddPortReference(def.OtherControlIn);

            _mode = _prevMode = ModeEnum.SetTo0;
        }

        public override void Tick(float delta)
        {
            // If the other control changed through other means, the mode must be
            // changed to STOP or else that change will be overwritten.
            // There's some tolerance in the check due to floating point rounding
            // issues when setting a control and reading its value afterwards.
            var other = OtherControlIn.Value;
            if (!MathHelper.WithinTolerance(other, _lastValue, 0.0001f) && _lastSet)
            {
                ControlIn.Value = 0.5f;
            }

            UpdateMode();

            switch (_mode)
            {
                case ModeEnum.SetTo0:
                    UpdateOtherControl(0);
                    break;
                case ModeEnum.StepDown:
                    if (Changed)
                    {
                        UpdateOtherControl(other - Step);
                    }
                    break;
                case ModeEnum.StepUp:
                    // Don't add one more step when coming down from fast stepping.
                    if (Changed && _prevMode != ModeEnum.FastStep)
                    {
                        UpdateOtherControl(other + Step);
                    }
                    break;
                case ModeEnum.FastStep:
                    if (Changed)
                    {
                        _time = 0;
                    }
                    else
                    {
                        _time += delta;
                    }

                    if (_time >= FastStepTime)
                    {
                        UpdateOtherControl(other + Step);
                        _time = 0;
                    }
                    break;
                default:
                    break;
            }

            _lastValue = OtherControlIn.Value;
            _lastSet = true;
        }

        private void UpdateMode()
        {
            _prevMode = _mode;
            _mode = ControlIn.Value switch
            {
                > Bound34 => ModeEnum.FastStep,
                > Bound23 => ModeEnum.StepUp,
                > Bound12 => ModeEnum.Stop,
                > Bound01 => ModeEnum.StepDown,
                _ => ModeEnum.SetTo0,
            };
        }

        private void UpdateOtherControl(float value)
        {
            OtherControlIn.Value = Mathf.Clamp01(value);
        }
    }
}
