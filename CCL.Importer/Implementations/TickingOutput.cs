using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using System;

namespace CCL.Importer.Implementations
{
    internal class TickingOutput : SimComponent
    {
        public readonly float TickingTime;
        public readonly float AbsoluteValueThreshold;
        public readonly FuseReference? PowerFuseRef;
        public readonly PortReference Input;
        public readonly Port OutReadOut;
        public readonly Port TickReadOut;

        private float _time;

        public TickingOutput(TickingOutputDefinitionInternal def) : base(def.ID)
        {
            TickingTime = def.TickingTime;
            AbsoluteValueThreshold = def.AbsoluteValueThreshold;

            if (!string.IsNullOrEmpty(def.PowerFuseId))
            {
                PowerFuseRef = AddFuseReference(def.PowerFuseId);
            }

            Input = AddPortReference(def.Input);
            OutReadOut = AddPort(def.OutReadOut);
            TickReadOut = AddPort(def.TickReadOut);

            _time = 0;
        }

        public override void Tick(float delta)
        {
            float input = Input.Value;
            bool thresh = Math.Abs(input) >= AbsoluteValueThreshold;
            input = ProcessValuePower(input);

            if (TickingTime <= 0)
            {
                OutReadOut.Value = thresh ? input : 0;
                return;
            }

            _time += delta;
            TickReadOut.Value = 0;

            if (_time < TickingTime) return;

            _time -= TickingTime;
            OutReadOut.Value = thresh ? input : 0;
            TickReadOut.Value = ProcessValuePower(thresh ? 1 : 0);
        }

        private float ProcessValuePower(float value)
        {
            return PowerFuseRef != null ? PowerFuseRef.ProcessInput(value) : value;
        }
    }
}
