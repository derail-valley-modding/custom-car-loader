using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Wheels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    public class SimCreatorWindow : EditorWindow
    {
        public enum SimulationType
        {
            DieselMechanical,
            DieselElectric,
            DieselHydraulic,
            BatteryElectric,
            Slug,
            Steam,
            Tender,
            Caboose
        }

        private static SimCreatorWindow? _instance;

        [MenuItem("GameObject/CCL/Create Loco Simulation", false, MenuOrdering.Simulation)]
        public static void OnContextMenu(MenuCommand command)
        {
            var selection = (GameObject)command.context;

            _instance = GetWindow<SimCreatorWindow>();
            _instance.SetTarget(selection.transform.root.gameObject);
            _instance.titleContent = new GUIContent("CCL - Simulation Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Create Loco Simulation", true, MenuOrdering.Simulation)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }


        private GameObject _targetRoot = null!;
        private bool _simItemsExist;

        private SimulationType _selectedType;
        private SimulationType SelectedType
        {
            get => _selectedType;
            set
            {
                if ((_creator == null) || (value != _selectedType))
                {
                    _creator = value switch
                    {
                        SimulationType.DieselMechanical => new DieselMechSimCreator(_targetRoot),
                        SimulationType.DieselElectric => new DieselElectricSimCreator(_targetRoot),
                        SimulationType.DieselHydraulic => new DieselHydraulicSimCreator(_targetRoot),
                        SimulationType.BatteryElectric => new BatteryElectricSimCreator(_targetRoot),
                        SimulationType.Slug => new SlugSimCreator(_targetRoot),
                        SimulationType.Steam => new SteamerSimCreator(_targetRoot),
                        SimulationType.Tender => new TenderSimCreator(_targetRoot),
                        SimulationType.Caboose => new CabooseSimCreator(_targetRoot),
                        _ => throw new NotImplementedException(),
                    };
                    _selectedType = value;
                    _basisIndex = 0;
                }
            }
        }

        private SimCreator? _creator = null;
        private int _basisIndex = 0;

        public void SetTarget(GameObject target)
        {
            _targetRoot = target;
            _simItemsExist = _targetRoot.transform.Find("[sim]");
            SelectedType = SimulationType.DieselMechanical;
        }

        public void OnGUI()
        {
            using (new WordWrapScope(true))
            {
                EditorGUILayout.BeginVertical("box");

                if (_simItemsExist)
                {
                    EditorGUILayout.LabelField("There is already a [sim] implementation on this prefab, this wizard should only be used on a newly created car");

                    if (GUILayout.Button("OK"))
                    {
                        Close();
                    }

                    EditorGUILayout.EndVertical();
                    return;
                }

                SelectedType = EditorHelpers.EnumPopup("Simulation Type", SelectedType);

                string[] basisOptions = _creator!.SimBasisOptions;
                _basisIndex = EditorGUILayout.Popup("Default Values", _basisIndex, basisOptions);

                EditorGUILayout.Space(18);

                if (GUILayout.Button("Add Simulation"))
                {
                    _creator.CreateSimForBasis(_basisIndex);
                    Close();
                }

                EditorHelpers.DrawHeader("Features");

                foreach (var feature in _creator.GetSimFeatures(_basisIndex))
                {
                    EditorGUILayout.LabelField($"• {feature}");
                }

                EditorGUILayout.EndVertical();
            }
        }
    }

    internal abstract partial class SimCreator
    {
        public abstract string[] SimBasisOptions { get; }

        protected GameObject _root;
        protected GameObject _sim = null!;
        protected DamageControllerProxy _damageController = null!;
        protected SimConnectionsDefinitionProxy _connectionDef = null!;
        protected BaseControlsOverriderProxy _baseControls = null!;

        public SimCreator(GameObject prefabRoot)
        {
            _root = prefabRoot;
        }

        public void CreateSimForBasis(int basisIndex)
        {
            try
            {
                CreateBaseComponents();
                CreateSimForBasisImpl(basisIndex);

                _connectionDef.executionOrder = _sim.GetComponentsInChildren<SimComponentDefinitionProxy>().ToList();
                Selection.activeGameObject = _sim;

                EditorUtility.SetDirty(_root);
                EditorHelpers.SaveAndRefresh();
            }
            catch
            {
                AbortAndCleanup();
                throw;
            }
        }

        protected void CreateBaseComponents()
        {
            _damageController = _root.GetComponent<DamageControllerProxy>();
            if (_damageController)
            {
                UnityEngine.Object.DestroyImmediate(_damageController);
            }
            
            _damageController = _root.AddComponent<DamageControllerProxy>();
            _damageController.SetCurveToDefault();

            _sim = new GameObject("[sim]");
            _sim.transform.parent = _root.transform;
            _sim.transform.SetAsFirstSibling();
            _sim.transform.localPosition = Vector3.zero;
            _sim.transform.localRotation = Quaternion.identity;

            _connectionDef = _sim.AddComponent<SimConnectionsDefinitionProxy>();
            _connectionDef.executionOrder = _sim.GetComponentsInChildren<SimComponentDefinitionProxy>().ToList();
            _connectionDef.connections = new List<PortConnectionProxy>();
            _connectionDef.portReferenceConnections = new List<PortReferenceConnectionProxy>();

            _baseControls = _sim.AddComponent<BaseControlsOverriderProxy>();
        }

        private void AbortAndCleanup()
        {
            if (_damageController) UnityEngine.Object.DestroyImmediate(_damageController);
            if (_sim) UnityEngine.Object.DestroyImmediate(_sim);
        }

        protected void ApplyMethodToAll<TScript>(Action<TScript> action)
        {
            foreach (TScript script in _root.GetComponentsInChildren<TScript>())
            {
                action(script);
            }
        }

        protected void ReparentComponents(MonoBehaviour parent, params MonoBehaviour[] children)
        {
            foreach (var child in children)
            {
                child.transform.parent = parent.transform;
            }
        }

        protected WheelSlideTrainsetObserverProxy AddWheelSlideObserver()
        {
            if (!_root.TryGetComponent(out WheelSlideTrainsetObserverProxy wheelslide))
            {
                wheelslide = _root.AddComponent<WheelSlideTrainsetObserverProxy>();
            }

            return wheelslide;
        }

        protected BasePortsOverriderProxy AddBasePortsOverrider()
        {
            if (!_baseControls.TryGetComponent(out BasePortsOverriderProxy overrider))
            {
                overrider = _baseControls.gameObject.AddComponent<BasePortsOverriderProxy>();
            }

            return overrider;
        }

        #region Generic Components

        protected TScript CreateSimComponent<TScript>(string id) 
            where TScript : SimComponentDefinitionProxy
        {
            var holder = new GameObject(id);
            holder.transform.parent = _sim.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            var comp = holder.AddComponent<TScript>();
            return comp;
        }

        protected TScript CreateSibling<TScript>(Component existing, string id)
            where TScript : SimComponentDefinitionProxy
        {
            var sibling = existing.gameObject.AddComponent<TScript>();
            sibling.ID = id;
            return sibling;
        }

        protected TScript CreateSibling<TScript>(Component existing)
            where TScript : MonoBehaviour
        {
            return existing.gameObject.AddComponent<TScript>();
        }

        #endregion

        #region One-off Components

        protected ResourceContainerProxy CreateResourceContainer(ResourceContainerType type, string? idOverride = null)
        {
            idOverride ??= Enum.GetName(typeof(ResourceContainerType), type).ToLower();
            var container = CreateSimComponent<ResourceContainerProxy>(idOverride);
            container.type = type;
            return container;
        }

        protected WaterDetectorDefinitionProxy CreateWaterDetector(string? idOverride = null)
        {
            idOverride ??= "waterDetector";
            var waterDetector = CreateSimComponent<WaterDetectorDefinitionProxy>(idOverride);
            var waterPortFeeder = CreateSibling<WaterDetectorPortFeederProxy>(waterDetector);
            waterPortFeeder.statePortId = FullPortId(waterDetector, "STATE_EXT_IN");
            return waterDetector;
        }

        protected CompressorSimControllerProxy CreateCompressorSim(SimComponentDefinitionProxy compressor)
        {
            var airController = CreateSibling<CompressorSimControllerProxy>(compressor);
            airController.activationSignalExtInPortId = FullPortId(compressor, "ACTIVATION_SIGNAL_EXT_IN");
            airController.productionRateOutPortId = FullPortId(compressor, "PRODUCTION_RATE");
            airController.mainReservoirVolumePortId = FullPortId(compressor, "MAIN_RES_VOLUME");
            airController.activationPressureThresholdPortId = FullPortId(compressor, "ACTIVATION_PRESSURE_THRESHOLD");
            airController.compressorHealthStatePortId = FullPortId(compressor, "COMPRESSOR_HEALTH_EXT_IN");

            return airController;
        }

        protected BroadcastPortValueProviderProxy CreateBroadcastProvider(SimComponentDefinitionProxy existing, string id,
            DVPortForwardConnectionType connectionType, string tag)
        {
            var provider = CreateSibling<BroadcastPortValueProviderProxy>(existing);
            provider.providerPortId = FullPortId(existing, id);
            provider.connection = connectionType;
            provider.connectionTag = tag;
            return provider;
        }

        protected BroadcastPortValueConsumerProxy CreateBroadcastConsumer(SimComponentDefinitionProxy existing, string id,
            DVPortForwardConnectionType connectionType, string tag, float disconnectValue, bool propagateChangeBack)
        {
            var consumer = CreateSibling<BroadcastPortValueConsumerProxy>(existing);
            consumer.consumerPortId = FullPortId(existing, id);
            consumer.connection = connectionType;
            consumer.connectionTag = tag;
            consumer.disconnectedValue = disconnectValue;
            consumer.propagateConsumerValueChangeBackToProvider = propagateChangeBack;
            return consumer;
        }

        protected TractionPortFeedersProxy CreateTractionFeeders(TractionDefinitionProxy traction)
        {
            var tractionFeeders = CreateSibling<TractionPortFeedersProxy>(traction);
            tractionFeeders.forwardSpeedPortId = FullPortId(traction, "FORWARD_SPEED_EXT_IN");
            tractionFeeders.wheelRpmPortId = FullPortId(traction, "WHEEL_RPM_EXT_IN");
            tractionFeeders.wheelSpeedKmhPortId = FullPortId(traction, "WHEEL_SPEED_KMH_EXT_IN");
            return tractionFeeders;
        }

        protected void AddGearShifter(SimComponentDefinitionProxy comp, bool isGearboxA)
        {
            var shifter = comp.gameObject.AddComponent<GearShifterProxy>();
            shifter.currentGearRatioPortId = FullPortId(comp, "GEAR_RATIO");
            shifter.isGearboxA = isGearboxA;
        }

        #endregion

        #region Controls

        protected ExternalControlDefinitionProxy CreateOverridableControl(OverridableControlType type, string? idOverride = null,
            float defaultValue = 0)
        {
            idOverride ??= type switch
            {
                OverridableControlType.TrainBrake => "brake",
                OverridableControlType.HeadlightsFront => "headlightsControlFront",
                OverridableControlType.HeadlightsRear => "headlightsControlRear",
                OverridableControlType.TrainBrakeCutout => "brakeCutout",
                OverridableControlType.Dynamo => "dynamoControl",
                OverridableControlType.AirPump => "compressorControl",
                _ => Enum.GetName(typeof(OverridableControlType), type).ToCamelCase(),
            };

            var control = CreateSimComponent<ExternalControlDefinitionProxy>(idOverride);
            control.defaultValue = defaultValue;
            control.saveState = type switch
            {
                OverridableControlType.HeadlightsFront => true,
                OverridableControlType.HeadlightsRear => true,
                OverridableControlType.CabLight => true,
                OverridableControlType.IndCabLight => true,
                OverridableControlType.TrainBrakeCutout => true,
                OverridableControlType.Dynamo => true,
                OverridableControlType.AirPump => true,
                _ => false,
            };

            var overrider = CreateSibling<OverridableControlProxy>(control);
            overrider.ControlType = type;
            overrider.portId = $"{idOverride}.EXT_IN";

            return control;
        }

        protected ExternalControlDefinitionProxy CreateExternalControl(string id, bool saveState = false, float defaultValue = 0)
        {
            var control = CreateSimComponent<ExternalControlDefinitionProxy>(id);
            control.saveState = saveState;
            control.defaultValue = defaultValue;
            return control;
        }

        protected ReverserDefinitionProxy CreateReverserControl(string? idOverride = null, bool isAnalog = false)
        {
            idOverride ??= "reverser";
            var control = CreateSimComponent<ReverserDefinitionProxy>(idOverride);
            control.isAnalog = isAnalog;
            var overrider = CreateSibling<OverridableControlProxy>(control);
            overrider.ControlType = OverridableControlType.Reverser;
            overrider.portId = $"{idOverride}.CONTROL_EXT_IN";
            return control;
        }

        protected SanderDefinitionProxy CreateSanderControl(string? idOverride = null)
        {
            idOverride ??= "sander";
            var control = CreateSimComponent<SanderDefinitionProxy>(idOverride);
            var overrider = CreateSibling<OverridableControlProxy>(control);
            overrider.ControlType = OverridableControlType.Sander;
            overrider.portId = $"{idOverride}.CONTROL_EXT_IN";
            return control;
        }

        #endregion

        #region Control Blockers

        protected ControlBlockerProxy AddControlBlocker(ExternalControlDefinitionProxy control, SimComponentDefinitionProxy blocking, string port, float threshold,
            ControlBlockerProxy.BlockerDefinition.BlockType blockerType, bool resetOnBlock = false)
        {
            return AddControlBlocker(control.GetComponent<OverridableControlProxy>(), blocking, port, threshold, blockerType, resetOnBlock);
        }

        protected ControlBlockerProxy AddControlBlocker(ReverserDefinitionProxy reverser, SimComponentDefinitionProxy blocking, string port, float threshold,
            ControlBlockerProxy.BlockerDefinition.BlockType blockerType, bool resetOnBlock = false)
        {
            return AddControlBlocker(reverser.GetComponent<OverridableControlProxy>(), blocking, port, threshold, blockerType, resetOnBlock);
        }

        protected ControlBlockerProxy AddControlBlocker(OverridableControlProxy control, SimComponentDefinitionProxy blocking, string port, float threshold,
            ControlBlockerProxy.BlockerDefinition.BlockType blockerType, bool resetOnBlock = false)
        {
            control.OnValidate();

            if (control.controlBlocker == null)
            {
                control.controlBlocker = control.gameObject.AddComponent<ControlBlockerProxy>();
            }

            var blockers = control.controlBlocker.blockers.ToList();

            blockers.Add(new ControlBlockerProxy.BlockerDefinition
            {
                blockerPortId = FullPortId(blocking, port),
                thresholdValue = threshold,
                blockType = blockerType,
            });

            control.controlBlocker.blockers = blockers.ToArray();
            control.controlBlocker.resetToZeroOnBlock = resetOnBlock;

            return control.controlBlocker;
        }

        protected ControlBlockerProxy AddEmptyControlBlocker(ExternalControlDefinitionProxy control)
        {
            return AddEmptyControlBlocker(control.GetComponent<OverridableControlProxy>());
        }

        protected ControlBlockerProxy AddEmptyControlBlocker(SanderDefinitionProxy control)
        {
            return AddEmptyControlBlocker(control.GetComponent<OverridableControlProxy>());
        }

        protected ControlBlockerProxy AddEmptyControlBlocker(OverridableControlProxy control)
        {
            control.OnValidate();

            if (control.controlBlocker == null)
            {
                control.controlBlocker = control.gameObject.AddComponent<ControlBlockerProxy>();
            }

            control.controlBlocker.blockedControlPortId = control.portId;

            return control.controlBlocker;
        }

        #endregion

        #region Lamps

        protected LampLogicDefinitionProxy CreateLamp(string id, DVPortValueType readerValueType,
            float offMin, float offMax, float onMin, float onMax, float blinkMin, float blinkMax,
            bool audioDrop = false, bool audioRaise = false)
        {
            var lamp = CreateLampBasic(id, readerValueType, offMin, offMax, audioDrop, audioRaise);

            AddOnRangeToLamp(lamp, onMin, onMax);
            AddBlinkRangeToLamp(lamp, blinkMin, blinkMax);

            return lamp;
        }

        protected LampLogicDefinitionProxy CreateLampOnOnly(string id, DVPortValueType readerValueType,
            float offMin, float offMax, float onMin, float onMax, bool audioDrop = false, bool audioRaise = false)
        {
            var lamp = CreateLampBasic(id, readerValueType, offMin, offMax, audioDrop, audioRaise);

            AddOnRangeToLamp(lamp, onMin, onMax);

            return lamp;
        }

        protected LampLogicDefinitionProxy CreateLampBlinkOnly(string id, DVPortValueType readerValueType,
            float offMin, float offMax, float blinkMin, float blinkMax, bool audioDrop = false, bool audioRaise = false)
        {
            var lamp = CreateLampBasic(id, readerValueType, offMin, offMax, audioDrop, audioRaise);

            AddBlinkRangeToLamp(lamp, blinkMin, blinkMax);

            return lamp;
        }

        protected LampLogicDefinitionProxy CreateLampDecreasingWarning(string id, DVPortValueType readerValueType,
            float max, float onTransition, float blinkTransition, float min, bool audio = true)
        {
            return CreateLamp(id, readerValueType, onTransition, max, blinkTransition, onTransition, min, blinkTransition, audio, false);
        }

        protected LampLogicDefinitionProxy CreateLampIncreasingWarning(string id, DVPortValueType readerValueType,
            float min, float onTransition, float blinkTransition, float max = float.PositiveInfinity, bool audio = false)
        {
            return CreateLamp(id, readerValueType, min, onTransition, onTransition, blinkTransition, blinkTransition, max, false, audio);
        }

        protected LampLogicDefinitionProxy CreateLampBasicControl(string id, float transition = 0.001f, bool limit1 = false)
        {
            return CreateLampOnOnly(id, DVPortValueType.CONTROL, 0, transition, transition, limit1 ? 1 : float.PositiveInfinity);
        }

        protected LampLogicDefinitionProxy CreateLampHeadlightControl(string id, float offMin = 0.4f, float offMax = 0.55f)
        {
            return CreateLampOnOnly(id, DVPortValueType.CONTROL, offMin, offMax, 0, 1);
        }

        private LampLogicDefinitionProxy CreateLampBasic(string id, DVPortValueType readerValueType,
            float min, float max, bool audioDrop, bool audioRaise)
        {
            var lamp = CreateSimComponent<LampLogicDefinitionProxy>(id);

            lamp.inputReader = new PortReferenceDefinition(readerValueType, "INPUT");

            lamp.offRangeMin = min;
            lamp.offRangeMax = max;

            lamp.playAudioOnValueDrop = audioDrop;
            lamp.playAudioOnValueRaise = audioRaise;

            return lamp;
        }

        private void AddOnRangeToLamp(LampLogicDefinitionProxy lamp, float min, float max)
        {
            lamp.onRangeUsed = true;
            lamp.onRangeMin = min;
            lamp.onRangeMax = max;
        }

        private void AddBlinkRangeToLamp(LampLogicDefinitionProxy lamp, float min, float max)
        {
            lamp.blinkRangeUsed = true;
            lamp.blinkRangeMin = min;
            lamp.blinkRangeMax = max;
        }

        #endregion

        protected string FullPortId(SimComponentDefinitionProxy component, string portId) => $"{component.ID}.{portId}";
        protected string FullFuseId(IndependentFusesDefinitionProxy fusebox, int index) => $"{fusebox.ID}.{fusebox.fuses[index].id}";
        protected string HeatPortId(HeatReservoirDefinitionProxy heat, int index) => $"{heat.ID}.HEAT_IN_{index}";

        #region Connections

        protected void ConnectPorts(string fullSourceId, string fullDestId)
        {
            _connectionDef.connections.Add(new PortConnectionProxy()
            {
                fullPortIdOut = fullSourceId,
                fullPortIdIn = fullDestId
            });
        }

        protected void ConnectPorts(SimComponentDefinitionProxy source, string sourceId, SimComponentDefinitionProxy dest, string destId)
        {
            _connectionDef.connections.Add(new PortConnectionProxy()
            {
                fullPortIdOut = FullPortId(source, sourceId),
                fullPortIdIn = FullPortId(dest, destId)
            });
        }

        protected void ConnectPortRef(string fullPortId, string fullRefId)
        {
            _connectionDef.portReferenceConnections.Add(new PortReferenceConnectionProxy()
            {
                portId = fullPortId,
                portReferenceId = fullRefId
            });
        }

        protected void ConnectPortRef(SimComponentDefinitionProxy refComp, string refId, SimComponentDefinitionProxy portComp, string portId)
        {
            _connectionDef.portReferenceConnections.Add(new PortReferenceConnectionProxy()
            {
                portReferenceId = FullPortId(refComp, refId),
                portId = FullPortId(portComp, portId)
            });
        }

        protected void ConnectHeatRef(HeatReservoirDefinitionProxy refComp, int index, SimComponentDefinitionProxy portComp, string portId = "HEAT_OUT")
        {
            _connectionDef.portReferenceConnections.Add(new PortReferenceConnectionProxy()
            {
                portReferenceId = HeatPortId(refComp, index),
                portId = FullPortId(portComp, portId)
            });
        }

        protected void ConnectLampRef(LampLogicDefinitionProxy refComp, SimComponentDefinitionProxy portComp, string portId)
        {
            _connectionDef.portReferenceConnections.Add(new PortReferenceConnectionProxy()
            {
                portReferenceId = FullPortId(refComp, refComp.inputReader.ID),
                portId = FullPortId(portComp, portId)
            });
        }

        // This may be used by as placeholder for unconnected ports, instead of needing to add a port later from scratch.
        // A few locomotives have empty ports defined like so.
        protected void ConnectEmptyPortRef(SimComponentDefinitionProxy refComp, string refId)
        {
            _connectionDef.portReferenceConnections.Add(new PortReferenceConnectionProxy()
            {
                portId = string.Empty,
                portReferenceId = FullPortId(refComp, refId)
            });
        }

        #endregion

        public abstract IEnumerable<string> GetSimFeatures(int basisIndex);

        public abstract void CreateSimForBasisImpl(int basisIndex);
    }
}
