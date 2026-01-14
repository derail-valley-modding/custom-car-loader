using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class CombinedControl : SimComponent
    {
        public readonly float NeutralValue;
        public readonly Port ControlAIn;
        public readonly Port ControlBIn;
        public readonly Port CombinedIn;
        public readonly Port CurrentMode;

        public CombinedControl(CombinedControlDefinitionInternal def) : base(def.ID)
        {
            NeutralValue = def.NeutralValue;
            ControlAIn = AddPort(def.ControlAIn);
            ControlBIn = AddPort(def.ControlBIn);
            CombinedIn = AddPort(def.CombinedIn, NeutralValue);
            CurrentMode = AddPort(def.CurrentMode);

            ControlAIn.ValueUpdatedInternally += AUpdated;
            ControlBIn.ValueUpdatedInternally += BUpdated;
            CombinedIn.ValueUpdatedInternally += CombinedUpdated;
        }

        public override void Tick(float delta) { }

        private void AUpdated(float value)
        {
            if (value > 0)
            {
                CombinedIn.Value = Mathf.Lerp(NeutralValue, 1, value);
                CurrentMode.Value = 1;
            }
            else if (value == 0)
            {
                CurrentMode.Value = 0;
            }
        }

        private void BUpdated(float value)
        {
            if (value > 0)
            {
                CombinedIn.Value = Mathf.Lerp(NeutralValue, 0, value);
                CurrentMode.Value = -1;
            }
            else if (value == 0)
            {
                CurrentMode.Value = 0;
            }
        }

        private void CombinedUpdated(float value)
        {
            if (value == NeutralValue)
            {
                ControlAIn.Value = 0;
                ControlBIn.Value = 0;
                CurrentMode.Value = 0;
                return;
            }

            if (value > NeutralValue)
            {
                ControlAIn.Value = Mathf.InverseLerp(NeutralValue, 1, value);
                ControlBIn.Value = 0;
                CurrentMode.Value = 1;
                return;
            }

            if (value < NeutralValue)
            {
                ControlAIn.Value = 0;
                ControlBIn.Value = Mathf.InverseLerp(NeutralValue, 0, value);
                CurrentMode.Value = -1;
                return;
            }
        }
    }
}
