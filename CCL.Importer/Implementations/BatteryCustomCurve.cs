using CCL.Importer.Components.Simulation.Electric;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class BatteryCustomCurve : SimComponent
    {
        private readonly int numSeriesCells = 36;
        private readonly float internalResistance;
        private readonly float baseConsumptionMultiplier;
        private readonly AnimationCurve chargeToVoltageCurve;
        private readonly float minVoltage;
        private readonly float maxVoltage;

        private readonly FuseReference powerFuseRef;
        private readonly Port voltageReadOut;
        private readonly Port voltageNormalizedReadOut;
        private readonly PortReference chargeNormalized;
        private readonly PortReference chargeConsumption;
        private readonly PortReference powerReader;

        public BatteryCustomCurve(BatteryCustomCurveDefinitionInternal def) : base(def.ID)
        {
            numSeriesCells = def.numSeriesCells;
            internalResistance = def.internalResistance;
            baseConsumptionMultiplier = def.baseConsumptionMultiplier;
            chargeToVoltageCurve = def.chargeToVoltageCurve;

            powerFuseRef = AddFuseReference(def.powerFuseId);
            chargeNormalized = AddPortReference(def.chargeNormalized, 0f);
            chargeConsumption = AddPortReference(def.chargeConsumption, 0f);
            voltageReadOut = AddPort(def.voltageReadOut, 0f);
            voltageNormalizedReadOut = AddPort(def.voltageNormalizedReadOut, 0f);
            powerReader = AddPortReference(def.powerReader, 0f);

            minVoltage = numSeriesCells * chargeToVoltageCurve[0].value;
            maxVoltage = numSeriesCells * chargeToVoltageCurve[chargeToVoltageCurve.length - 1].value;
        }

        public override void Tick(float delta)
        {
            float voltage = numSeriesCells * chargeToVoltageCurve.Evaluate(chargeNormalized.Value);
            float power = voltage * voltage - 4f * powerReader.Value * internalResistance;

            if (chargeNormalized.Value <= 0f || power <= 0f)
            {
                powerFuseRef.ChangeState(false);
                voltageReadOut.Value = 0f;
                voltageNormalizedReadOut.Value = 0f;
                chargeConsumption.Value = 0f;
                return;
            }

            float afterPowered = powerFuseRef.ProcessInput(0.5f * (voltage + Mathf.Sqrt(power)));
            voltageReadOut.Value = afterPowered;
            voltageNormalizedReadOut.Value = Mathf.InverseLerp(minVoltage, maxVoltage, afterPowered);
            float consumption = gameParams.ResourceConsumptionModifier * baseConsumptionMultiplier * powerReader.Value;
            chargeConsumption.Value = consumption * delta / 1000000f;
        }
    }
}
