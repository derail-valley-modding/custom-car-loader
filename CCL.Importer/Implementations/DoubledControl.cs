using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;

namespace CCL.Importer.Implementations
{
    internal class DoubledControl : SimComponent
    {
        public readonly Port ControlAIn;
        public readonly Port ControlBIn;
        public readonly Port CombinedIn;
        public readonly Port EffectA;
        public readonly Port EffectB;

        public readonly PortReference Effect;

        private bool _lockDoubles;
        private bool _lockCombined;

        public DoubledControl(DoubledControlDefinitionInternal def) : base(def.ID)
        {
            ControlAIn = AddPort(def.ControlAIn, def.DefaultValue);
            ControlBIn = AddPort(def.ControlBIn, def.DefaultValue);
            CombinedIn = AddPort(def.CombinedIn, def.DefaultValue);
            EffectA = AddPort(def.EffectA);
            EffectB = AddPort(def.EffectB);

            Effect = AddPortReference(def.EffectReader);

            ControlAIn.ValueUpdatedInternally += AnyUpdated;
            ControlBIn.ValueUpdatedInternally += AnyUpdated;
            CombinedIn.ValueUpdatedInternally += CombinedUpdated;
        }

        public override void Tick(float delta)
        {
            var effect = Effect.Value;
            EffectA.Value = effect * ControlAIn.Value;
            EffectB.Value = effect * ControlBIn.Value;
        }

        private void AnyUpdated(float value)
        {
            if (_lockCombined) return;

            _lockDoubles = true;
            CombinedIn.Value = ControlAIn.Value * 0.5f + ControlBIn.Value * 0.5f;
            _lockDoubles = false;
        }

        private void CombinedUpdated(float value)
        {
            if (_lockDoubles) return;

            _lockCombined = true;
            ControlAIn.Value = value;
            ControlBIn.Value = value;
            _lockCombined = false;
        }
    }
}
