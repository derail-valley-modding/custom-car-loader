using CCL.Types.Json;
using CCL.Types.Proxies.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Tutorial
{
    [CreateAssetMenu(menuName = "CCL/Custom Tutorial", order = MenuOrdering.Tutorial)]
    public class TutorialSetup : ScriptableObject, ICustomSerialized
    {
        public enum QTSemantic
        {
            Look,
            Monitor,
            Engage,
            EngageCW,
            EngageCCW,
            GentlyEngage,
            FullyEngage,
            Disengage,
            GoForward,
            GoBackward,
            SetToNeutral,
            SetToNotch1,
            SetToNotch2,
            SetCloserToNeutral,
            Open,
            Close,
            Ignite
        }

        [Serializable]
        public class SemanticRange
        {
            public QTSemantic Semantic;
            public float Minimum;
            public float Maximum;

            public SemanticRange() : this(QTSemantic.Look, 0, 1) { }

            public SemanticRange(QTSemantic semantic, float minimum, float maximum)
            {
                Semantic = semantic;
                Minimum = minimum;
                Maximum = maximum;
            }
        }

        // Defaults.
        private static SemanticRange FrontHeadlightsDefault => new SemanticRange(QTSemantic.EngageCW, 0.55f, 1.00f);
        private static SemanticRange RearHeadlightsDefault => new SemanticRange(QTSemantic.EngageCCW, 0.00f, 0.30f);
        private static SemanticRange FrontIndHeadlightsTypeDefault => new SemanticRange(QTSemantic.Disengage, 0, 0);
        private static SemanticRange FrontIndHeadlights1Default => new SemanticRange(QTSemantic.Engage, 1, 1);
        private static SemanticRange RearIndHeadlightsTypeDefault => new SemanticRange(QTSemantic.Engage, 1, 1);
        private static SemanticRange RearIndHeadlights1Default => new SemanticRange(QTSemantic.Engage, 1, 1);

        [Header("Conditions")]
        [Tooltip("Any shovel, oiler and lighter")]
        public bool RequireSteamerItems = false;
        public ResourceContainerType[] RequiredResources = new[]
        {
            ResourceContainerType.Fuel,
            ResourceContainerType.Oil,
            ResourceContainerType.ElectricCharge
        };
        [Range(30.0f, 300.0f)]
        public float MaximumDistance = 30.0f;

        [Header("Steps")]
        public List<TutorialPhase> CustomPhases1 = new List<TutorialPhase>();

        // Steam.
        [Space]
        public bool PrepareSteam = false;
        [EnableIf(nameof(PrepareSteam))]
        public float TargetFireTemperature = 400.0f;
        [EnableIf(nameof(PrepareSteam)), Tooltip("Relative")]
        public float TargetSteamPressure = 1.0f;

        [Space]
        public List<TutorialPhase> CustomPhases2 = new List<TutorialPhase>();

        // Oiling. This could be part of the steam part but eh.
        [Space]
        public bool OilingPoints = false;
        [EnableIf(nameof(OilingPoints))]
        public bool ShowTrainsetLubricators = true;
        [EnableIf(nameof(AnyOil))]
        public string OilOverride = string.Empty;

        [Space]
        public List<TutorialPhase> CustomPhases3 = new List<TutorialPhase>();

        // Cab.
        [Space]
        public bool ShowBasicCabControls = true;
        [EnableIf(nameof(ShowBasicCabControls))]
        public bool HeadlightsBeforeCab = false;
        [EnableIf(nameof(ShowBasicCabControls))]
        public SemanticRange FrontHeadlights = FrontHeadlightsDefault;
        [EnableIf(nameof(ShowBasicCabControls))]
        public SemanticRange FrontIndHeadlightsType = FrontIndHeadlightsTypeDefault;
        [EnableIf(nameof(ShowBasicCabControls))]
        public SemanticRange FrontIndHeadlights1 = FrontIndHeadlights1Default;
        [EnableIf(nameof(ShowBasicCabControls))]
        public SemanticRange RearHeadlights = RearHeadlightsDefault;
        [EnableIf(nameof(ShowBasicCabControls))]
        public SemanticRange RearIndHeadlightsType = RearIndHeadlightsTypeDefault;
        [EnableIf(nameof(ShowBasicCabControls))]
        public SemanticRange RearIndHeadlights1 = RearIndHeadlights1Default;
        public bool MarkCabLightAsSteam = false;
        public bool MarkHornAsSteam = false;

        [Space]
        public List<TutorialPhase> CustomPhases4 = new List<TutorialPhase>();

        // Diesel starting.
        [Space]
        public bool StartDieselEngine = true;
        [EnableIf(nameof(StartDieselEngine))]
        public QTSemantic StarterSemantic = QTSemantic.EngageCW;

        public List<TutorialPhase> CustomPhases5 = new List<TutorialPhase>();

        // Misc.
        [Space]
        public bool ShowBrakes = true;
        public bool EngageThrottleForBrakeCharging = true;
        public bool ShowSpeedometer = true;
        public bool ShowSand = true;
        [EnableIf(nameof(ShowSand))]
        public string SandOverride = string.Empty;
        public bool HasGearboxA = true;
        public bool HasGearboxB = true;

        [Space]
        public List<TutorialPhase> CustomPhases6 = new List<TutorialPhase>();

        // Movement.
        [Space]
        public bool ShowMovement = true;
        [EnableIf(nameof(ShowMovement))]
        public bool TreatAsSteam = false;
        [EnableIf(nameof(MovementAndSteam)), Tooltip("Relative")]
        public float TargetChargedChestPressure = 6.0f;
        [EnableIf(nameof(MovementAndSteam)), Tooltip("Relative")]
        public float TargetChestPressure = 3.0f;

        [Space]
        public List<TutorialPhase> CustomPhases7 = new List<TutorialPhase>();

        // Indicators.
        [Space]
        public bool ShowWheelslip = true;
        [EnableIf(nameof(ShowWheelslip))]
        public string WheelslipOverride = string.Empty;
        public bool ShowBattery = true;
        [EnableIf(nameof(ShowBattery))]
        public string BatteryOverride = string.Empty;
        public bool ShowVoltage = true;
        [EnableIf(nameof(ShowVoltage))]
        public string VoltageOverride = string.Empty;
        public bool ShowFuel = true;
        [EnableIf(nameof(ShowFuel))]
        public string FuelOverride = string.Empty;
        [EnableIf(nameof(OilingPoints), true), Tooltip("If it hasn't been shown through the oiling points part\n" +
            "Override is on that section")]
        public bool ShowOil = true;

        [Space]
        public List<TutorialPhase> CustomPhases8 = new List<TutorialPhase>();

        [Header("Controls Trainset Override")]
        public int Dynamo = -1;
        public int AirPump = -1;
        public int Handbrake = -1;

        [Header("Indicators Trainset Override")]
        public int[] CarsWithWater = new int[0];
        public int[] CarsWithCoal = new int[0];

        #region Serialization and Buttons

        [SerializeField, HideInInspector]
        private string? _frontHeadlights;
        [SerializeField, HideInInspector]
        private string? _frontIndHeadlightsType;
        [SerializeField, HideInInspector]
        private string? _frontIndHeadlights1;
        [SerializeField, HideInInspector]
        private string? _rearHeadlights;
        [SerializeField, HideInInspector]
        private string? _rearIndHeadlightsType;
        [SerializeField, HideInInspector]
        private string? _rearIndHeadlights1;
        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(SetupSteam), "Setup Steam")]
        [MethodButton(nameof(SetupDiesel), "Setup Diesel")]
        private bool _buttons;

        #endregion

        public void OnValidate()
        {
            _frontHeadlights = JSONObject.ToJson(FrontHeadlights);
            _frontIndHeadlightsType = JSONObject.ToJson(FrontIndHeadlightsType);
            _frontIndHeadlights1 = JSONObject.ToJson(FrontIndHeadlights1);
            _rearHeadlights = JSONObject.ToJson(RearHeadlights);
            _rearIndHeadlightsType = JSONObject.ToJson(RearIndHeadlightsType);
            _rearIndHeadlights1 = JSONObject.ToJson(RearIndHeadlights1);
        }

        public void AfterImport()
        {
            FrontHeadlights = JSONObject.FromJson(_frontHeadlights, () => FrontHeadlightsDefault);
            FrontIndHeadlightsType = JSONObject.FromJson(_frontIndHeadlightsType, () => FrontIndHeadlightsTypeDefault);
            FrontIndHeadlights1 = JSONObject.FromJson(_frontIndHeadlights1, () => FrontIndHeadlights1Default);
            RearHeadlights = JSONObject.FromJson(_rearHeadlights, () => RearHeadlightsDefault);
            RearIndHeadlightsType = JSONObject.FromJson(_rearIndHeadlightsType, () => RearIndHeadlightsTypeDefault);
            RearIndHeadlights1 = JSONObject.FromJson(_rearIndHeadlights1, () => RearIndHeadlights1Default);
        }

        private void SetupSteam()
        {
            RequiredResources = new[] { ResourceContainerType.Coal, ResourceContainerType.Water };

            RequireSteamerItems = true;

            PrepareSteam = true;

            OilingPoints = true;

            ShowBasicCabControls = true;
            HeadlightsBeforeCab = true;
            MarkCabLightAsSteam = true;
            MarkHornAsSteam = true;

            StartDieselEngine = false;

            ShowBrakes = true;
            EngageThrottleForBrakeCharging = false;

            ShowMovement = true;
            TreatAsSteam = true;
        }

        private void SetupDiesel()
        {
            RequiredResources = new[] { ResourceContainerType.Fuel, ResourceContainerType.Oil, ResourceContainerType.ElectricCharge };
            RequireSteamerItems = false;

            PrepareSteam = false;

            OilingPoints = false;

            ShowBasicCabControls = true;
            HeadlightsBeforeCab = false;
            MarkCabLightAsSteam = false;
            MarkHornAsSteam = false;

            StartDieselEngine = true;

            ShowBrakes = true;
            EngageThrottleForBrakeCharging = true;

            ShowMovement = true;
            TreatAsSteam = false;
        }

        private bool AnyOil => OilingPoints || ShowOil;
        private bool MovementAndSteam => ShowMovement && TreatAsSteam;
    }
}
