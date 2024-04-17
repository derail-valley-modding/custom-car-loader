using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class TechEntry
    {
        public TechIcon Icon = TechIcon.None;
        [Tooltip("Description of the tech (\"I8 Diesel Turbocharged\", \"14 bar Coal Burner Superheated\")")]
        public string Description = "";
        [Tooltip("Type of the tech (\"Steam Engine\", \"Electric Transmission\")")]
        public string Type = "";

        public static void TryToSetAppropriateType(TechEntry tech, VehicleType type)
        {
            if (!string.IsNullOrEmpty(tech.Type))
            {
                return;
            }
            
            tech.Type = tech.Icon switch
            {
                //TechIcon.None => throw new NotImplementedException(),
                //TechIcon.Generic => throw new NotImplementedException(),
                TechIcon.ClosedCab => "Closed Cab",
                TechIcon.OpenCab => "Open Cab",
                TechIcon.CrewCompartment => "Crew Compartment",
                TechIcon.CompressedAirBrakeSystem => "Compressed Air Brake System",
                TechIcon.DirectBrakeSystem => "Direct Brake System",
                TechIcon.DynamicBrakeSystem => "Dynamic Brake System",
                //TechIcon.ElectricPowerSupplyAndTransmission => throw new NotImplementedException(),
                TechIcon.ExternalControlInterface => "Aditional Control Interface",
                TechIcon.HeatManagement => "Heat Management",
                TechIcon.HydraulicTransmission => "Hydraulic Transmission",
                TechIcon.InternalCombustionEngine => "Internal Combustion Engine",
                TechIcon.MechanicalTransmission => "Mechanical Transmission",
                TechIcon.PassengerCompartment => "Passenger Compartment",
                TechIcon.SpecializedEquipment => "Specialized Equipment",
                TechIcon.SteamEngine => "Steam Engine",
                TechIcon.UnitEffect => TryToGuessUnitEffect(type),
                TechIcon.CrewDelivery => "Order with comms radio",
                _ => tech.Type
            };
        }

        private static string TryToGuessUnitEffect(VehicleType type)
        {
            switch (type)
            {
                case VehicleType.Locomotive:
                    return "Score includes secondary unit(s) effects";
                case VehicleType.Tender:
                    return "Primary unit score already includes effects below";
                case VehicleType.Slug:
                case VehicleType.Car:
                    return "Potential effect on other units shown below";
                default:
                    return "";
            }
        }
    }
}
