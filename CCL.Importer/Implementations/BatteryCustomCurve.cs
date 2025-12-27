using CCL.Importer.Components.Simulation.Electric;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class BatteryCustomCurve : SimComponent
    {
        private readonly int _numSeriesCells = 36;
        private readonly float _internalResistance;
        private readonly float _baseConsumptionMultiplier;
        private readonly AnimationCurve _chargeToVoltageCurve;
        private readonly float _minVoltage;
        private readonly float _maxVoltage;

        private readonly FuseReference _powerFuseRef;
        private readonly Port _voltageReadOut;
        private readonly Port _voltageNormalizedReadOut;
        private readonly PortReference _chargeNormalized;
        private readonly PortReference _chargeConsumption;
        private readonly PortReference _powerReader;

        public BatteryCustomCurve(BatteryCustomCurveDefinitionInternal def) : base(def.ID)
        {
            _numSeriesCells = def.numSeriesCells;
            _internalResistance = def.internalResistance;
            _baseConsumptionMultiplier = def.baseConsumptionMultiplier;
            _chargeToVoltageCurve = def.chargeToVoltageCurve;

            _powerFuseRef = AddFuseReference(def.powerFuseId);
            _chargeNormalized = AddPortReference(def.chargeNormalized, 0f);
            _chargeConsumption = AddPortReference(def.chargeConsumption, 0f);
            _voltageReadOut = AddPort(def.voltageReadOut, 0f);
            _voltageNormalizedReadOut = AddPort(def.voltageNormalizedReadOut, 0f);
            _powerReader = AddPortReference(def.powerReader, 0f);

            _minVoltage = _numSeriesCells * _chargeToVoltageCurve[0].value;
            _maxVoltage = _numSeriesCells * _chargeToVoltageCurve[_chargeToVoltageCurve.length - 1].value;
        }

        public override void Tick(float delta)
        {
            float voltage = _numSeriesCells * _chargeToVoltageCurve.Evaluate(_chargeNormalized.Value);
            float num = voltage * voltage - 4f * _powerReader.Value * _internalResistance;

            if (_chargeNormalized.Value <= 0f || num <= 0f)
            {
                _powerFuseRef.ChangeState(false);
                _voltageReadOut.Value = 0f;
                _voltageNormalizedReadOut.Value = 0f;
                _chargeConsumption.Value = 0f;
                return;
            }

            voltage = _powerFuseRef.ProcessInput(0.5f * (voltage + Mathf.Sqrt(num)));
            _voltageReadOut.Value = voltage;
            _voltageNormalizedReadOut.Value = Mathf.InverseLerp(_minVoltage, _maxVoltage, voltage);
            float consumption = gameParams.ResourceConsumptionModifier * _baseConsumptionMultiplier * _powerReader.Value;
            _chargeConsumption.Value = consumption * delta / 1000000f;
        }
    }
}
