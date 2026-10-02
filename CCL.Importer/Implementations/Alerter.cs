using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class Alerter : SimComponent
    {
        public readonly float MinimumSpeed = 1;
        public readonly float VigilanceTime = 30;
        public readonly float WarningTime = 5;

        public readonly Port Control;
        public readonly Port State;
        public readonly Port TimeLeft;
        public readonly Port ControllerActive;

        public readonly PortReference WheelSpeed;

        public readonly FuseReference? PowerFuseRef;

        private float _time;

        private bool IsPowered => PowerFuseRef == null || PowerFuseRef.State;
        private bool IsMoving => Mathf.Abs(WheelSpeed.Value) > MinimumSpeed;
        private bool IsActive => IsPowered && IsMoving && ControllerActive.Value > 0.5f;
        private bool WarningStateReached => _time < 0;
        private bool StoppingStateReached => _time < -WarningTime;

        public Alerter(AlerterDefinitionInternal def) : base(def.ID)
        {
            WarningTime = def.WarningTime;

            Control = AddPort(def.AcknowledgeControl);
            State = AddPort(def.StateReadout);
            TimeLeft = AddPort(def.TimeLeftReadout);
            ControllerActive = AddPort(def.ControllerActive, 1);

            WheelSpeed = AddPortReference(def.WheelSpeedReader);

            if (!string.IsNullOrEmpty(def.PowerFuseId))
            {
                PowerFuseRef = AddFuseReference(def.PowerFuseId);
            }

            Control.ValueUpdatedInternally += ControlChanged;
        }

        public override void Tick(float delta)
        {
            if (!IsActive)
            {
                Reset();
                return;
            }

            _time -= delta;

            if (StoppingStateReached)
            {
                State.Value = 2;
                TimeLeft.Value = 0;
            }
            else if (WarningStateReached)
            {
                State.Value = 1;
                TimeLeft.Value = WarningTime + _time;
            }
            else
            {
                State.Value = 0;
                TimeLeft.Value = _time;
            }
        }

        private void ControlChanged(float value)
        {
            if (value > 0.5)
            {
                Reset();
            }
        }

        private void Reset()
        {
            _time = VigilanceTime;
            TimeLeft.Value = VigilanceTime;
            State.Value = 0;
        }
    }
}
