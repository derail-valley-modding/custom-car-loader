using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class SteamAutostoker : SimComponent
    {
        public const float MIN_WORKING_PRESSURE = 3.0f;

        public readonly float MaxTransferRate;
        public readonly float MaxSteamConsumption;
        public readonly float MaxWorkingPressure;
        public readonly float CoalConsumptionMultiplier;
        public readonly float SmoothTime;

        public readonly Port SteamConsumptionReadout;
        public readonly Port StokingNormalizedReadout;

        public readonly PortReference Control;
        public readonly PortReference SteamPressure;
        public readonly PortReference FireboxCoalLevel;
        public readonly PortReference FireboxCoalCapacity;
        public readonly PortReference FireboxCoalControl;
        public readonly PortReference CoalAmount;
        public readonly PortReference CoalConsumeExtIn;

        private float _rate;
        private float _vel;

        private float ConsumptionModifier => gameParams.ResourceConsumptionModifier;
        private float NormalizedRate => _rate / MaxTransferRate;
        private float SpaceForCoal => (FireboxCoalCapacity.Value - FireboxCoalLevel.Value) * CoalConsumptionMultiplier;
        private float CoalAvailable => ConsumptionModifier == 0f ? CoalAmount.Value : CoalAmount.Value / ConsumptionModifier;

        public SteamAutostoker(SteamAutostokerDefinitionInternal def) : base(def.ID)
        {
            _rate = 0;
            _vel = 0;

            MaxTransferRate = def.MaxTransferRate;
            MaxSteamConsumption = def.MaxSteamConsumption;
            MaxWorkingPressure = def.MaxWorkingPressure;
            CoalConsumptionMultiplier = def.FireboxCoalConsumptionMultiplier;
            SmoothTime = def.SmoothTime;

            SteamConsumptionReadout = AddPort(def.SteamConsumptionReadOut);
            StokingNormalizedReadout = AddPort(def.StokingNormalizedReadOut);

            Control = AddPortReference(def.Control);
            SteamPressure = AddPortReference(def.SteamPressure);
            FireboxCoalLevel = AddPortReference(def.FireboxCoalLevel);
            FireboxCoalCapacity = AddPortReference(def.FireboxCoalCapacity);
            FireboxCoalControl = AddPortReference(def.FireboxCoalControl);
            CoalAmount = AddPortReference(def.CoalAmount);
            CoalConsumeExtIn = AddPortReference(def.CoalConsume);
        }

        public override void Tick(float delta)
        {
            // Avoid calculating again.
            var available = CoalAvailable;
            var space = SpaceForCoal;

            // Scale to the max rate based on the available pressure.
            var targetRate = NumberUtil.MapClamp(SteamPressure.Value, MIN_WORKING_PRESSURE, MaxWorkingPressure, 0, MaxTransferRate * Control.Value);

            _rate = Mathf.SmoothDamp(_rate, targetRate, ref _vel, SmoothTime, float.PositiveInfinity, delta);

            // Ensure we can actually transfer coal.
            var transfer = Mathf.Min(_rate * delta, available, space);

            // Apply modifiers and send it.
            CoalConsumeExtIn.Value = transfer * ConsumptionModifier;
            FireboxCoalControl.Value = transfer * CoalConsumptionMultiplier;

            // Calculate readouts.
            StokingNormalizedReadout.Value = NormalizedRate;
            SteamConsumptionReadout.Value = StokingNormalizedReadout.Value * MaxSteamConsumption;
        }
    }
}
