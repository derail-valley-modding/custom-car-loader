using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;

using static CCL.Types.Components.Simulation.FuseLogicDefinition;

namespace CCL.Importer.Implementations
{
    internal class FuseLogic : SimComponent
    {
        private readonly LogicType _logic;
        private readonly FuseReference _fuseA;
        private readonly FuseReference _fuseB;
        private readonly Fuse _output;

        public FuseLogic(FuseLogicDefinitionInternal def) : base(def.ID)
        {
            _logic = def.Logic;
            _fuseA = AddFuseReference(def.FuseA);
            _fuseB = AddFuseReference(def.FuseB);
            _output = AddFuse(def.OutputFuse);
        }

        public override void Tick(float delta)
        {
            switch (_logic)
            {
                case LogicType.AND:
                    _output.ChangeState(_fuseA.State & _fuseB.State);
                    break;
                case LogicType.OR:
                    _output.ChangeState(_fuseA.State | _fuseB.State);
                    break;
                case LogicType.XOR:
                    _output.ChangeState(_fuseA.State ^ _fuseB.State);
                    break;
                case LogicType.NOR:
                    _output.ChangeState(!(_fuseA.State | _fuseB.State));
                    break;
                case LogicType.NAND:
                    _output.ChangeState(!(_fuseA.State & _fuseB.State));
                    break;
                case LogicType.XNOR:
                    _output.ChangeState(_fuseA.State == _fuseB.State);
                    break;
                default:
                    throw new System.NotSupportedException($"Unknown logic {_logic}");
            }
        }
    }
}
