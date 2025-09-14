using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;

namespace CCL.Importer.Implementations
{
    internal class FuseInverter : SimComponent
    {
        public class FusePair
        {
            private FuseReference _source;
            private FuseReference _target;

            public FusePair(FuseReference source, FuseReference target)
            {
                _source = source;
                _target = target;
            }

            public void Tick()
            {
                _target.ChangeState(!_source.State);
            }
        }

        public readonly FusePair[] FusesToInvert;

        public FuseInverter(FuseInverterDefinitionInternal def) : base(def.ID)
        {
            FusesToInvert = new FusePair[def.SourceFuses.Length];

            for (int i = 0; i < FusesToInvert.Length; i++)
            {
                FusesToInvert[i] = new FusePair(
                    AddFuseReference(def.SourceFuses[i]),
                    AddFuseReference(def.InvertedFuses[i]));
            }
        }

        public override void Tick(float delta)
        {
            foreach (var item in FusesToInvert)
            {
                item.Tick();
            }
        }
    }
}
