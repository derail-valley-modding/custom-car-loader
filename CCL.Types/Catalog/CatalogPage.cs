using UnityEngine;

namespace CCL.Types.Catalog
{
    [CreateAssetMenu(menuName = "CCL/Vehicle Catalog Page")]
    public class CatalogPage : ScriptableObject
    {
        [Header("Header")]
        public Color HeaderColour = Color.yellow;
        public string Name = "";
        public string Units = "1/1";
        public Sprite Icon = null!;
        public string ProductionYears = "1900-1999";
        public bool UnlockedByGarage = false;
        // Licenses here

        [Header("Vehicle Type")]
        public VehicleType Type = VehicleType.Locomotive;
        public VehicleRole Role1 = VehicleRole.None;
        public VehicleRole Role2 = VehicleRole.None;

        [Header("Vehicle Diagram")]
        public VehicleDiagram Diagram = null!;

        [Header("Scores")]
        public EaseOfOperationScore EaseOfOperationScore = new EaseOfOperationScore();
        public MaintenanceScore MaintenanceScore = new MaintenanceScore();
        public HaulingScore HaulingScore = new HaulingScore();
        public ShuntingScore ShuntingScore = new ShuntingScore();
    }
}
