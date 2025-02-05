using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
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
            Tender
        }

        private static SimCreatorWindow? _instance;

        [MenuItem("GameObject/CCL/Create Loco Simulation", false, 10)]
        public static void OnContextMenu(MenuCommand command)
        {
            var selection = (GameObject)command.context;

            _instance = GetWindow<SimCreatorWindow>();
            _instance.SetTarget(selection.transform.root.gameObject);
            _instance.titleContent = new GUIContent("CCL - Simulation Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Create Loco Simulation", true, 10)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }


        private GameObject _targetRoot;
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
            EditorGUILayout.BeginVertical("box");
            EditorStyles.label.wordWrap = true;

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

            string[] basisOptions = _creator.SimBasisOptions;
            _basisIndex = EditorGUILayout.Popup("Default Values", _basisIndex, basisOptions);

            EditorGUILayout.Space(18);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Simulation"))
            {
                _creator.CreateSimForBasis(_basisIndex);
                Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }

    internal abstract partial class SimCreator
    {
        public abstract string[] SimBasisOptions { get; }

        protected GameObject _root;
        protected GameObject _sim;
        protected DamageControllerProxy _damageController;
        protected SimConnectionsDefinitionProxy _connectionDef;
        protected BaseControlsOverriderProxy _baseControls;

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

            _damageController.speedToBrakeDamageCurve = new AnimationCurve()
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0, 0, 0, 0, 0.333f, 0.333f),
                    new Keyframe(2, 0, 0, 0, 0.333f, 0.333f),
                    new Keyframe(7.868f, 0.127f, 0.028f, 0.028f, 0.333f, 0.333f),
                    new Keyframe(29.032f, 0.671f, 0.015f, 0.015f, 0.333f, 0.333f),
                    new Keyframe(100, 1, 0, 0, 0.333f, 0.333f),
                }
            };

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

        protected ResourceContainerProxy CreateResourceContainer(ResourceContainerType type, string? idOverride = null)
        {
            idOverride ??= Enum.GetName(typeof(ResourceContainerType), type).ToLower();
            var container = CreateSimComponent<ResourceContainerProxy>(idOverride);
            container.type = type;
            return container;
        }

        protected CoalPileSimControllerProxy CreateCoalPile(ResourceContainerProxy coal)
        {
            var pile = CreateSibling<CoalPileSimControllerProxy>(coal);
            pile.coalAvailablePortId = FullPortId(coal, "AMOUNT");
            pile.coalCapacityPortId = FullPortId(coal, "CAPACITY");
            pile.coalConsumePortId = FullPortId(coal, "CONSUME_EXT_IN");
            return pile;
        }

        protected ExternalControlDefinitionProxy CreateOverridableControl(OverridableControlType type, string? idOverride = null)
        {
            idOverride ??= Enum.GetName(typeof(OverridableControlType), type).ToLower();
            var control = CreateSimComponent<ExternalControlDefinitionProxy>(idOverride);
            var overrider = CreateSibling<OverridableControlProxy>(control);
            overrider.ControlType = type;
            overrider.portId = $"{idOverride}.EXT_IN";

            control.saveState = type switch
            {
                OverridableControlType.TrainBrakeCutout => true,
                _ => false,
            };

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

        protected WaterDetectorDefinitionProxy CreateWaterDetector(string? idOverride = null)
        {
            idOverride ??= "waterDetector";
            var waterDetector = CreateSimComponent<WaterDetectorDefinitionProxy>(idOverride);
            var waterPortFeeder = CreateSibling<WaterDetectorPortFeederProxy>(waterDetector);
            waterPortFeeder.statePortId = FullPortId(waterDetector, "STATE_EXT_IN");
            return waterDetector;
        }

        protected ControlBlockerProxy AddControlBlocker(ExternalControlDefinitionProxy control, SimComponentDefinitionProxy blocking, string port, float threshold,
            ControlBlockerProxy.BlockerDefinition.BlockType blockerType)
        {
            return AddControlBlocker(control.GetComponent<OverridableControlProxy>(), blocking, port, threshold, blockerType);
        }

        protected ControlBlockerProxy AddControlBlocker(ReverserDefinitionProxy reverser, SimComponentDefinitionProxy blocking, string port, float threshold,
            ControlBlockerProxy.BlockerDefinition.BlockType blockerType)
        {
            return AddControlBlocker(reverser.GetComponent<OverridableControlProxy>(), blocking, port, threshold, blockerType);
        }

        protected ControlBlockerProxy AddControlBlocker(OverridableControlProxy control, SimComponentDefinitionProxy blocking, string port, float threshold,
            ControlBlockerProxy.BlockerDefinition.BlockType blockerType)
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

            return control.controlBlocker;
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

        protected string FullPortId(SimComponentDefinitionProxy component, string portId) => $"{component.ID}.{portId}";
        protected string FullFuseId(IndependentFusesDefinitionProxy fusebox, int index) => $"{fusebox.ID}.{fusebox.fuses[index].id}";
        protected string HeatPortId(HeatReservoirDefinitionProxy heat, int index) => $"{heat.ID}.HEAT_IN_{index}";

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

        public abstract void CreateSimForBasisImpl(int basisIndex);
    }
}
