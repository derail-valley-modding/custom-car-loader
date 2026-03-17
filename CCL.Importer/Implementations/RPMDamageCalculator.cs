using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class RPMDamageCalculator : SimComponent
    {
        private readonly float _maxRpm;
        private readonly float _damagePerSecond;
        private readonly float _overspeedDamagePerSecond;
        private readonly float _scaling;
        private readonly PortReference _rpm;
        private readonly Port _overspeedSound;
        private readonly Port _damageReadout;
        private readonly Port _rpmNormalised;

        public RPMDamageCalculator(RPMDamageCalculatorDefinitionInternal def) : base(def.ID)
        {
            _maxRpm = def.MaxRPM;
            _damagePerSecond = def.DamagePerSecond;
            _overspeedDamagePerSecond = def.OverspeedDamagePerSecond;
            _scaling = def.ScalingFactor;

            _rpm = AddPortReference(def.RPMReader);

            _overspeedSound = AddPort(def.OverspeedSound);
            _damageReadout = AddPort(def.GeneratedDamage);
            _rpmNormalised = AddPort(def.RPMNormalised);
        }

        public override void Tick(float delta)
        {
            var failures = gameParams.DrivetrainFailuresAllowed;

            var rpm = Mathf.Abs(_rpm.Value);
            var rpmNormalised = rpm / _maxRpm;

            _overspeedSound.Value = Mathf.InverseLerp(0.9f, 1f, rpmNormalised);
            _rpmNormalised.Value = rpmNormalised;

            if (!failures)
            {
                _damageReadout.Value = 0;
                return;
            }

            var damage = rpmNormalised * _damagePerSecond;

            if (rpmNormalised > 1)
            {
                damage += _overspeedDamagePerSecond * 0.1f * Mathf.Pow(rpmNormalised, _scaling);
            }

            _damageReadout.Value = damage * delta;
        }
    }
}
