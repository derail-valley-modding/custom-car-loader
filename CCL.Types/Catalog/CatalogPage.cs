using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [CreateAssetMenu(menuName = "CCL/Vehicle Catalog Page")]
    public class CatalogPage : ScriptableObject
    {
        public CustomCarType CorrespondingVehicle = null!;

        [Header("Header")]
        public Color HeaderColour = Color.yellow;
        public string PageName = "";
        public string ConsistUnits = "1/1";
        public Sprite Icon = null!;
        public string ProductionYears = "1900-1999";
        public bool UnlockedByGarage = false;
        // Licenses here

        [Header("Vehicle Type")]
        public VehicleType Type = VehicleType.Locomotive;
        public VehicleRole Role1 = VehicleRole.None;
        public VehicleRole Role2 = VehicleRole.None;

        [Header("Properties And Tonnage")]
        public bool SummonableByRemote = false;
        public bool ShowLoadRatings = true;
        public LoadColor LoadFlat = new LoadColor { Tonnage = 1200, Color = CatalogColor.Yellow };
        public LoadColor LoadIncline = new LoadColor { Tonnage = 300, Color = CatalogColor.Red };
        public LoadColor LoadInclineRain = new LoadColor { Tonnage = 250, Color = CatalogColor.Red };

        [Header("Vehicle Diagram")]
        public VehicleDiagram Diagram = new VehicleDiagram();
        public TechList TechList = new TechList();

        [Header("Scores")]
        public EaseOfOperationScore EaseOfOperation = new EaseOfOperationScore();
        public MaintenanceScore Maintenance = new MaintenanceScore();
        public HaulingScore Hauling = new HaulingScore();
        public ShuntingScore Shunting = new ShuntingScore();

        public ScoreList GetScoreList(int i) => i switch
        {
            0 => EaseOfOperation,
            1 => Maintenance,
            2 => Hauling,
            3 => Shunting,
            _ => throw new System.ArgumentOutOfRangeException(nameof(i)),
        };

        public IEnumerable<ScoreList> AllScoreLists
        {
            get
            {
                yield return EaseOfOperation;
                yield return Maintenance;
                yield return Hauling;
                yield return Shunting;
            }
        }
    }
}
