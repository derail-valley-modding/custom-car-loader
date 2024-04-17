using CCL.Types.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [CreateAssetMenu(menuName = "CCL/Vehicle Catalog Page")]
    public class CatalogPage : ScriptableObject, ICustomSerialized
    {
        public CustomCarType CorrespondingVehicle = null!;

        [Header("Header")]
        public Color HeaderColour = Color.yellow;
        public string PageName = "";
        public string ConsistUnits = "1/1";
        [Tooltip("Optional")]
        public string Nickname = "";
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

        [SerializeField, HideInInspector]
        private string? _loadJson;
        [SerializeField, HideInInspector]
        private string? _loadInclineJson;
        [SerializeField, HideInInspector]
        private string? _loadInclineRainJson;
        [SerializeField, HideInInspector]
        private string? _techJson;
        [SerializeField, HideInInspector]
        private string? _easeScore;
        [SerializeField, HideInInspector]
        private string? _maintScore;
        [SerializeField, HideInInspector]
        private string? _haulScore;
        [SerializeField, HideInInspector]
        private string? _shuntScore;

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

        public void OnValidate()
        {
            _loadJson = JSONObject.ToJson(LoadFlat);
            _loadInclineJson = JSONObject.ToJson(LoadIncline);
            _loadInclineRainJson = JSONObject.ToJson(LoadInclineRain);

            _techJson = JSONObject.ToJson(TechList);

            _easeScore = JSONObject.ToJson(EaseOfOperation);
            _maintScore = JSONObject.ToJson(Maintenance);
            _haulScore = JSONObject.ToJson(Hauling);
            _shuntScore = JSONObject.ToJson(Shunting);
        }

        public void AfterImport()
        {
            LoadFlat = JSONObject.FromJson(_loadJson, () => new LoadColor());
            LoadIncline = JSONObject.FromJson(_loadInclineJson, () => new LoadColor());
            LoadInclineRain = JSONObject.FromJson(_loadInclineRainJson, () => new LoadColor());

            TechList = JSONObject.FromJson(_techJson, () => new TechList());

            EaseOfOperation = JSONObject.FromJson(_easeScore, () => new EaseOfOperationScore());
            Maintenance = JSONObject.FromJson(_maintScore, () => new MaintenanceScore());
            Hauling = JSONObject.FromJson(_haulScore, () => new HaulingScore());
            Shunting = JSONObject.FromJson(_shuntScore, () => new ShuntingScore());
        }
    }
}
