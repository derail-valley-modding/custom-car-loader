using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;

namespace CCL.Importer.Implementations
{
    internal class ConstantMultiplierOffset : SimComponent
    {
        private readonly float _multiplier;
        private readonly float _offset;
        private readonly PortReference _input;
        private readonly Port _output;

        public ConstantMultiplierOffset(ConstantMultiplierOffsetDefinitionInternal def) : base(def.ID)
        {
            _multiplier = def.Multiplier;
            _offset = def.Offset;
            _input = AddPortReference(def.Input);
            _output = AddPort(def.Output);
        }

        public override void Tick(float delta)
        {
            _output.Value = _input.Value * _multiplier + _offset;
        }
    }
}
