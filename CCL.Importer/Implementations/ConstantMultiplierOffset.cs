using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using System;

namespace CCL.Importer.Implementations
{
    internal class ConstantMultiplierOffset : SimComponent
    {
        private readonly float _multiplier;
        private readonly float _offset;
        private readonly bool _abs;
        private readonly PortReference _input;
        private readonly Port _output;

        public ConstantMultiplierOffset(ConstantMultiplierOffsetDefinitionInternal def) : base(def.ID)
        {
            _multiplier = def.Multiplier;
            _offset = def.Offset;
            _abs = def.Absolute;
            _input = AddPortReference(def.Input);
            _output = AddPort(def.Output);
        }

        public override void Tick(float delta)
        {
            var result = _input.Value * _multiplier + _offset;
            _output.Value = _abs ? Math.Abs(result) : result;
        }
    }
}
