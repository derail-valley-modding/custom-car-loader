using CCL.Importer.Components.Simulation;
using DV.JObjectExtstensions;
using LocoSim.Implementations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class DoubledControl : SimComponent
    {
        private const string SaveKeyA = "valA";
        private const string SaveKeyB = "valB";

        public readonly Port ControlAIn;
        public readonly Port ControlBIn;
        public readonly Port CombinedIn;
        public readonly Port EffectA;
        public readonly Port EffectB;

        public readonly PortReference Effect;

        private bool _lockDoubles;
        private bool _lockCombined;
        private bool _save;

        public override bool HasSaveData => _save;

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
            var a = ControlAIn.Value;
            var b = ControlBIn.Value;
            var total = a + b;

            if (total < 0.001f)
            {
                EffectA.Value = 0;
                EffectB.Value = 0;
            }
            else
            {
                EffectA.Value = 2 * effect * (a / total);
                EffectB.Value = 2 * effect * (b / total);
            }
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

        public override JObject GetSaveStateData()
        {
            var jo = new JObject();
            jo.SetFloat(SaveKeyA, ControlAIn.Value);
            jo.SetFloat(SaveKeyB, ControlBIn.Value);
            return jo;
        }

        public override void SetSaveStateData(JObject savedData)
        {
            var a = savedData.GetFloat(SaveKeyA);
            var b = savedData.GetFloat(SaveKeyB);

            if (a.HasValue)
            {
                ControlAIn.Value = a.Value;
            }
            else
            {
                Debug.LogError($"Unexpected state: Missing save data for {id} (control A). Loading ignored for this parameter.");
            }

            if (b.HasValue)
            {
                ControlBIn.Value = b.Value;
            }
            else
            {
                Debug.LogError($"Unexpected state: Missing save data for {id} (control B). Loading ignored for this parameter.");
            }
        }
    }
}
