using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class TechEntry
    {
        public TechIcon Icon = TechIcon.None;
        [Tooltip("Description of the tech"), TechDescription]
        public string Description = string.Empty;
        [Tooltip("Type of the tech"), TechType]
        public string Type = string.Empty;

        public static void TryToSetAppropriateType(TechEntry tech, VehicleType type)
        {
            if (!string.IsNullOrEmpty(tech.Type))
            {
                return;
            }
            
            tech.Type = tech.Icon switch
            {
                TechIcon.None => "",
                //TechIcon.Generic => throw new NotImplementedException(),
                TechIcon.ClosedCab => "vc/techtype/closed_cab",
                TechIcon.OpenCab => "vc/techtype/open_cab",
                TechIcon.CrewCompartment => "vc/techtype/crew_quarters",
                TechIcon.CompressedAirBrakeSystem => "vc/techtype/air_brake",
                TechIcon.DirectBrakeSystem => "vc/techtype/direct_brake",
                TechIcon.DynamicBrakeSystem => "vc/techtype/dynamic_brake",
                //TechIcon.ElectricPowerSupplyAndTransmission => throw new NotImplementedException(),
                TechIcon.ExternalControlInterface => "vc/techtype/additional_control_interface",
                TechIcon.HeatManagement => "vc/techtype/heat_management",
                TechIcon.HydraulicTransmission => "vc/techtype/hydro_trans",
                TechIcon.InternalCombustionEngine => "vc/techtype/engine",
                TechIcon.MechanicalTransmission => "vc/techtype/mech_trans",
                TechIcon.PassengerCompartment => "vc/techtype/passenger_compartment",
                TechIcon.SpecializedEquipment => "vc/techtype/specialized_equipment",
                TechIcon.SteamEngine => "vc/techtype/steam",
                TechIcon.UnitEffect => TryToGuessUnitEffect(type),
                TechIcon.CrewDelivery => "vc/techdesc/delivery/by_dv_crew",
                _ => tech.Type
            };

            // Unit effect is tied to the icon too so try to fill it.
            if (tech.Icon == TechIcon.UnitEffect && string.IsNullOrEmpty(tech.Description))
            {
                tech.Description = TryToGuessUnitEffectDesc(type);
            }
        }

        private static string TryToGuessUnitEffect(VehicleType type) => type switch
        {
            VehicleType.Locomotive => "vc/techdesc/unit/primary_score",
            VehicleType.Tender => "vc/techdesc/unit/secondary_score",
            _ => "vc/techdesc/unit/optional_score",
        };

        private static string TryToGuessUnitEffectDesc(VehicleType type) => type switch
        {
            VehicleType.Locomotive => "vc/techdesc/unit/primary",
            VehicleType.Tender => "vc/techdesc/unit/secondary",
            _ => "vc/techdesc/unit/optional",
        };
    }
}
