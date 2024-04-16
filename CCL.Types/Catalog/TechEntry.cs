using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class TechEntry
    {
        public TechIcon Icon;
        [Tooltip("Description of the tech (\"I8 Diesel Turbocharged\", \"14 bar Coal Burner Superheated\")")]
        public string Description = "";
        [Tooltip("Type of the tech (\"Steam Engine\", \"Electric Transmission\")")]
        public string Type = "";

        public static void TryToSetAppropriateType(TechEntry tech) => tech.Type = tech.Icon switch
        {
            //TechIcon.None => throw new NotImplementedException(),
            TechIcon.ClosedCab => "Closed Cab",
            TechIcon.OpenCab => "Open Cab",
            TechIcon.CrewCompartment => "Crew Compartment",
            TechIcon.CompressedAirBrakeSystem => "Compressed Air Brake System",
            TechIcon.DirectBrakeSystem => "Direct Brake System",
            TechIcon.DynamicBrakeSystem => "Dynamic Brake System",
            TechIcon.ElectricPowerSupplyAndTransmission => DebugWarningWithInlineOutput(
                "Electric Power Supply And Transmission can be used with Electric Power Supply or Electric Transmission.", tech.Type),
            TechIcon.ExternalControlInterface => "Aditional Control Interface",
            TechIcon.HeatManagement => "Heat Management",
            TechIcon.HydraulicTransmission => "Hydraulic Transmission",
            TechIcon.InternalCombustionEngine => "Internal Combustion Engine",
            TechIcon.MechanicalTransmission => "Mechanical Transmission",
            TechIcon.PassengerCompartment => "Passenger Compartment",
            TechIcon.SpecializedEquipment => "Specialized Equipment",
            TechIcon.SteamEngine => "Steam Engine",
            TechIcon.UnitEffect => DebugWarningWithInlineOutput(
                "Unit Effect usage varies with unit type.", tech.Type),
            TechIcon.CrewDelivery => "Order with comms radio",
            _ => tech.Type
        };

        private static string DebugWarningWithInlineOutput(string warning, string output)
        {
            Debug.LogWarning(warning);
            return output;
        }
    }
}
