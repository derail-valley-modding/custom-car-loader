﻿using CCL.Types.Json;
using DVLangHelper.Data;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [CreateAssetMenu(menuName = "CCL/Vehicle Catalog Page", order = MenuOrdering.Catalog)]
    public class CatalogPage : ScriptableObject, ICustomSerialized
    {
        public const string PageNameKeyPrefix = "ccl/vc/pagename/";
        public const string NicknameKeyPrefix = "ccl/vc/nickname/";

        [Header("Header")]
        public Color HeaderColour = Color.yellow;
        public TranslationData PageName = new TranslationData();
        public string ConsistUnits = "1/1";
        [Tooltip("Optional")]
        public bool HasNickname = false;
        public TranslationData Nickname = new TranslationData();
        public string ProductionYears = "1900-1999";

        [Header("Vehicle Type")]
        public VehicleType Type = VehicleType.Locomotive;
        public VehicleRole Role1 = VehicleRole.None;
        public VehicleRole Role2 = VehicleRole.None;

        [Header("Tonnage")]
        public bool ShowLoadRatings = true;
        public LoadRating LoadFlat = new LoadRating { Tonnage = 1200, Rating = TonnageRating.Medium };
        public LoadRating LoadIncline = new LoadRating { Tonnage = 300, Rating = TonnageRating.Bad };
        public LoadRating LoadInclineWet = new LoadRating { Tonnage = 250, Rating = TonnageRating.Bad };

        [Header("Vehicle Diagram")]
        public GameObject DiagramLayout = null!;
        public VehicleDiagramExtras DiagramExtras = new VehicleDiagramExtras();
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
        private string? _loadInclineWetJson;
        [SerializeField, HideInInspector]
        private string? _diagramJson;
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
            _loadInclineWetJson = JSONObject.ToJson(LoadInclineWet);

            _diagramJson = JSONObject.ToJson(DiagramExtras);

            _techJson = JSONObject.ToJson(TechList);

            _easeScore = JSONObject.ToJson(EaseOfOperation);
            _maintScore = JSONObject.ToJson(Maintenance);
            _haulScore = JSONObject.ToJson(Hauling);
            _shuntScore = JSONObject.ToJson(Shunting);
        }

        public void AfterImport()
        {
            LoadFlat = JSONObject.FromJson(_loadJson, () => new LoadRating());
            LoadIncline = JSONObject.FromJson(_loadInclineJson, () => new LoadRating());
            LoadInclineWet = JSONObject.FromJson(_loadInclineWetJson, () => new LoadRating());

            DiagramExtras = JSONObject.FromJson(_diagramJson, () => new VehicleDiagramExtras());

            TechList = JSONObject.FromJson(_techJson, () => new TechList());

            EaseOfOperation = JSONObject.FromJson(_easeScore, () => new EaseOfOperationScore());
            Maintenance = JSONObject.FromJson(_maintScore, () => new MaintenanceScore());
            Hauling = JSONObject.FromJson(_haulScore, () => new HaulingScore());
            Shunting = JSONObject.FromJson(_shuntScore, () => new ShuntingScore());
        }
    }
}
