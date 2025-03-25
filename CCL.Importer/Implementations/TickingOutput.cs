using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using System;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class TickingOutput : SimComponent
    {
        public readonly float TickingTime;
        public readonly float AbsoluteValueDifference;
        public readonly FuseReference? PowerFuseRef;
        public readonly PortReference Input;
        public readonly Port OutReadOut;
        public readonly Port TickReadOut;

        private float _time;

        public TickingOutput(TickingOutputDefinitionInternal def) : base(def.ID)
        {
            TickingTime = def.TickingTime;
            AbsoluteValueDifference = def.AbsoluteValueDifference;

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

            if (Math.Abs(input) < AbsoluteValueDifference)
            {
                input = 0;
            }

            input = ProcessValuePower(input);

            if (TickingTime <= 0)
            {
                OutReadOut.Value = input;
                return;
            }

            _time += delta;
            TickReadOut.Value = 0;

            if (_time < TickingTime) return;

            _time -= TickingTime;
            OutReadOut.Value = input;
            TickReadOut.Value = ProcessValuePower(1);
        }

        private float ProcessValuePower(float value)
        {
            return PowerFuseRef != null ? PowerFuseRef.ProcessInput(value) : value;
        }
    }
}
