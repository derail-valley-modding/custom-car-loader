using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;

using static CCL.Types.Components.Simulation.FuseLogicDefinition;

namespace CCL.Importer.Implementations
{
    internal class MultipleFuseLogic : SimComponent
    {
        private readonly LogicType _logic;
        private readonly FuseReference[] _fuses;
        private readonly Fuse _output;

        public MultipleFuseLogic(MultipleFuseLogicDefinitionInternal def) : base(def.ID)
        {
            _logic = def.Logic;
            _output = AddFuse(def.OutputFuse);

            _fuses = new FuseReference[def.Fuses.Length];

            for (int i = 0; i < _fuses.Length; i++)
            {
                _fuses[i] = AddFuseReference(def.Fuses[i]);
            }
        }

        public override void Tick(float delta)
        {
            var state = _fuses[0].State;

            for (int i = 0; i < _fuses.Length; i++)
            {
                var next = _fuses[i].State;

                state = _logic switch
                {
                    LogicType.AND or LogicType.NAND => state & next,
                    LogicType.OR or LogicType.NOR => state | next,
                    LogicType.XOR or LogicType.XNOR => state ^ next,
                    _ => throw new System.NotSupportedException($"Unknown logic {_logic}"),
                };
            }

            if (_logic is LogicType.NAND or LogicType.XOR or LogicType.XNOR)
            {
                state = !state;
            }

            _output.ChangeState(state);
        }
    }
}
