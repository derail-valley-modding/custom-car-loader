using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;

namespace CCL.Importer.Implementations
{
    internal class ManualLapBrakeSplitter : SimComponent
    {
        private const float RegularToCombined = 3.0f / 4.0f;

        public readonly Port RegularIn;
        public readonly Port EmergencyIn;
        public readonly Port CombinedIn;

        public ManualLapBrakeSplitter(ManualLapBrakeSplitterDefinitionInternal def) : base(def.ID)
        {
            RegularIn = AddPort(def.RegularIn);
            EmergencyIn = AddPort(def.EmergencyIn);
            CombinedIn = AddPort(def.CombinedIn);

            RegularIn.ValueUpdatedInternally += RegularUpdated;
            EmergencyIn.ValueUpdatedInternally += EmergencyUpdated;
            CombinedIn.ValueUpdatedInternally += CombinedUpdated;
        }

        public override void Tick(float delta) { }

        private void RegularUpdated(float value)
        {
            // Emergency takes priority, so don't need to do anything else.
            if (EmergencyIn.Value > 0.5f) return;

            CombinedIn.Value = value * RegularToCombined;
        }

        private void EmergencyUpdated(float value)
        {
            if (value > 0.5f)
            {
                CombinedIn.Value = 1.0f;
            }
            else
            {
                CombinedIn.Value = RegularIn.Value * RegularToCombined;
            }
        }

        private void CombinedUpdated(float value)
        {
            // Thresholds match brake system values.
            switch (value)
            {
                case > 0.9f:
                    EmergencyIn.Value = 1.0f;
                    break;
                case > 0.5f:
                    EmergencyIn.Value = 0.0f;
                    RegularIn.Value = 1.0f;
                    break;
                case > 0.1f:
                    EmergencyIn.Value = 0.0f;
                    RegularIn.Value = 0.5f;
                    break;
                default:
                    EmergencyIn.Value = 0.0f;
                    RegularIn.Value = 0.0f;
                    break;
            }
        }
    }
}
