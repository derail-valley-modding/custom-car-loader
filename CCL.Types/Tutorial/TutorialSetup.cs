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
        public class NonOverridableObject
        {
            public bool Show = true;
        }

        [Serializable]
        public class OverridableObject
        {
            public bool Show = true;
            [Tooltip("The ID of the TutorialObjectID to use instead of the original one\n" +
                "Object can be in other parts of a trainset")]
            public string Override = string.Empty;
        }

        [Serializable]
        public class SemanticRange
        {
            [Tooltip("The wording for interacting with the object")]
            public QTSemantic Semantic;
            [Tooltip("The minimum value allowed")]
            public float Minimum;
            [Tooltip("The maximum value allowed")]
            public float Maximum;

            public SemanticRange() : this(QTSemantic.Look, 0, 1) { }

            public SemanticRange(QTSemantic semantic, float minimum, float maximum)
            {
                Semantic = semantic;
                Minimum = minimum;
                Maximum = maximum;
            }
        }

        [Serializable]
        public class ControlsHolder
        {
            [Tooltip("The index of vehicle in the trainset that should display the handbrake\n" +
                "Should be left at -1 to use the default location")]
            public int HandbrakeTrainsetOverride = -1;

            public bool HeadlightsBeforeCabLight = false;

            [Header("Regular Headlights")]
            public OverridableObject FrontHeadlights = new OverridableObject();
            public SemanticRange FrontHeadlightsSettings = new SemanticRange(QTSemantic.EngageCW, 0.55f, 1.00f);
            public bool SingleHeadlightControl = false;

            [Space]
            public OverridableObject RearHeadlights = new OverridableObject();
            public SemanticRange RearHeadlightsSettings = new SemanticRange(QTSemantic.EngageCCW, 0.00f, 0.30f);

            [Header("DM3 Style Headlights")]
            public OverridableObject FrontIndHeadlightsType = new OverridableObject();
            public SemanticRange FrontIndHeadlightsTypeSettings = new SemanticRange(QTSemantic.Disengage, 0, 0);
            public OverridableObject FrontIndHeadlights1 = new OverridableObject();
            public SemanticRange FrontIndHeadlights1Settings = new SemanticRange(QTSemantic.Engage, 1, 1);
            public OverridableObject FrontIndHeadlights2 = new OverridableObject();

            [Space]
            public OverridableObject RearIndHeadlightsType = new OverridableObject();
            public SemanticRange RearIndHeadlightsTypeSettings = new SemanticRange(QTSemantic.Engage, 1, 1);
            public OverridableObject RearIndHeadlights1 = new OverridableObject();
            public SemanticRange RearIndHeadlights1Settings = new SemanticRange(QTSemantic.Engage, 1, 1);
            public OverridableObject RearIndHeadlights2 = new OverridableObject();

            [Header("Steam")]
            public OverridableObject Dynamo = new OverridableObject();
            public OverridableObject AirPump = new OverridableObject();

            [Header("Gears")]
            public NonOverridableObject GearboxA = new NonOverridableObject();
            public NonOverridableObject GearboxB = new NonOverridableObject();

            [Header("Other")]
            public NonOverridableObject FuelCutoff = new NonOverridableObject();
            public NonOverridableObject DynamicBrake = new NonOverridableObject();
            public NonOverridableObject Bell = new NonOverridableObject();
            public NonOverridableObject Horn = new NonOverridableObject();
            public bool MarkHornAsWhistle = false;
        }

        [Serializable]
        public class IndicatorsHolder
        {
            public OverridableObject Speedometer = new OverridableObject();
            public OverridableObject Sand = new OverridableObject();

            public OverridableObject Amps = new OverridableObject();
            public OverridableObject TMTemp = new OverridableObject();
            public OverridableObject OilTemp = new OverridableObject();

            public OverridableObject Wheelslip = new OverridableObject();
            public OverridableObject Battery = new OverridableObject();
            public OverridableObject Voltage = new OverridableObject();
            public OverridableObject Fuel = new OverridableObject();
            public OverridableObject Oil = new OverridableObject();
        }

        [Header("Conditions")]
        [Tooltip("Any shovel, oiler and lighter")]
        public bool RequireSteamerItems = false;
        public ResourceContainerType[] RequiredResources = new[]
        {
            ResourceContainerType.Fuel,
            ResourceContainerType.Oil,
            ResourceContainerType.ElectricCharge
        };
        [Range(30.0f, 300.0f), Tooltip("Maximum distance before the tutorial is automatically cancelled")]
        public float MaximumDistance = 30.0f;

        [Header("Conditions")]
        public bool IncludeDieselPrerequisites = true;
        public bool IncludeSteamerPrerequisites = false;

        // Steam.
        [Space]
        public bool PrepareSteam = false;
        [EnableIf(nameof(PrepareSteam))]
        public float TargetFireTemperature = 400.0f;
        [EnableIf(nameof(PrepareSteam)), Tooltip("Relative")]
        public float TargetSteamPressure = 1.0f;

        // Oiling. This could be part of the steam part but eh.
        [Space]
        public bool OilingPoints = false;

        // Cab.
        [Space]
        public bool ShowBasicCabControls = true;
        public bool CabLightIsGearLight = false;

        // Diesel starting.
        [Space]
        public bool StartDieselEngine = true;
        [EnableIf(nameof(StartDieselEngine))]
        public QTSemantic StarterSemantic = QTSemantic.EngageCW;

        // Misc.
        [Space]
        public bool ShowBrakes = true;
        public bool EngageThrottleForBrakeCharging = true;

        // Movement.
        [Space]
        public bool ShowMovement = true;
        [EnableIf(nameof(ShowMovement))]
        public bool TreatAsSteam = false;
        [EnableIf(nameof(MovementAndSteam)), Tooltip("Relative")]
        public float TargetChargedChestPressure = 6.0f;
        [EnableIf(nameof(MovementAndSteam)), Tooltip("Relative")]
        public float TargetChestPressure = 3.0f;

        // Controls and Indicators
        [Space]
        public ControlsHolder Controls = new ControlsHolder();
        public IndicatorsHolder Indicators = new IndicatorsHolder();

        [Space]
        public List<TutorialPhase> CustomPhases = new List<TutorialPhase>();

        [Header("Indicators Trainset Override")]
        public int[] TrainsetIndicesWithWater = new int[0];
        public int[] TrainsetIndicesWithCoal = new int[0];

        #region Serialization and Buttons

        [SerializeField, HideInInspector]
        private string? _controls;
        [SerializeField, HideInInspector]
        private string? _indicators;

        [SerializeField, RenderMethodButtons]
        [MethodButton(nameof(SetupSteam), "Setup Steam")]
        [MethodButton(nameof(SetupDiesel), "Setup Diesel")]
        private bool _buttons;

        #endregion

        public void OnValidate()
        {
            _controls = JSONObject.ToJson(Controls);
            _indicators = JSONObject.ToJson(Indicators);
        }

        public void AfterImport()
        {
            Controls = JSONObject.FromJson(_controls, () => new ControlsHolder());
            Indicators = JSONObject.FromJson(_indicators, () => new IndicatorsHolder());
        }

        private void SetupSteam()
        {
            RequireSteamerItems = true;
            RequiredResources = new[] { ResourceContainerType.Coal, ResourceContainerType.Water };

            IncludeDieselPrerequisites = false;
            IncludeSteamerPrerequisites = true;

            PrepareSteam = true;

            OilingPoints = true;

            ShowBasicCabControls = true;
            Controls.HeadlightsBeforeCabLight = true;
            CabLightIsGearLight = true;
            Controls.MarkHornAsWhistle = true;

            StartDieselEngine = false;

            ShowBrakes = true;
            EngageThrottleForBrakeCharging = false;

            ShowMovement = true;
            TreatAsSteam = true;
        }

        private void SetupDiesel()
        {
            RequireSteamerItems = false;
            RequiredResources = new[] { ResourceContainerType.Fuel, ResourceContainerType.Oil, ResourceContainerType.ElectricCharge };

            IncludeDieselPrerequisites = true;
            IncludeSteamerPrerequisites = false;

            PrepareSteam = false;

            OilingPoints = false;

            ShowBasicCabControls = true;
            Controls.HeadlightsBeforeCabLight = false;
            CabLightIsGearLight = false;
            Controls.MarkHornAsWhistle = false;

            StartDieselEngine = true;

            ShowBrakes = true;
            EngageThrottleForBrakeCharging = true;

            ShowMovement = true;
            TreatAsSteam = false;
        }

        private bool MovementAndSteam => ShowMovement && TreatAsSteam;
    }
}
