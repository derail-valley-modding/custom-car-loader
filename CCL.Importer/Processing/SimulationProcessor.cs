using CCL.Importer.Components;
using CCL.Importer.Components.Controllers;
using CCL.Types.Components;
using DV.CabControls;
using DV.Damage;
using DV.HUD;
using DV.KeyboardInput;
using DV.MultipleUnit;
using DV.Rain;
using DV.RemoteControls;
using DV.Simulation.Brake;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Fuses;
using DV.Simulation.Ports;
using DV.Util;
using DV.Wheels;
using LocoSim.Definitions;
using LocoSim.DVExtensions.Test;
using LocoSim.Implementations.Wheels;
using System.ComponentModel.Composition;
using UnityEngine;
using VerletRope;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(ExternalInteractableProcessor))]
    [RequiresStep(typeof(ProxyScriptProcessor))]
    [RequiresStep(typeof(OilingPointsProcessor))]
    internal class SimulationProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            var livery = context.Car;

            // Map additional controllers for all prefab parts
            AddAdditionalControllers(livery.prefab);
            if (livery.interiorPrefab)
            {
                AddAdditionalControllers(livery.interiorPrefab);
                if (!livery.interiorPrefab.GetComponent<InteriorControlsManager>())
                {
                    livery.interiorPrefab.AddComponent<InteriorControlsManager>();
                }
            }
            if (livery.explodedInteriorPrefab)
            {
                AddAdditionalControllers(livery.explodedInteriorPrefab);
                if (!livery.explodedInteriorPrefab.GetComponent<InteriorControlsManager>())
                {
                    livery.explodedInteriorPrefab.AddComponent<InteriorControlsManager>();
                }
            }
            if (livery.externalInteractablesPrefab)
            {
                AddAdditionalControllers(livery.externalInteractablesPrefab);
            }
            if (livery.explodedExternalInteractablesPrefab)
            {
                AddAdditionalControllers(livery.explodedExternalInteractablesPrefab);
            }

            // Add Control Override components
            var baseOverrider = livery.prefab.GetComponentInChildren<BaseControlsOverrider>(true);
            var controls = livery.prefab.GetComponentsInChildren<OverridableBaseControl>();
            if (controls.Length > 0 && !baseOverrider)
            {
                baseOverrider = livery.prefab.AddComponent<BaseControlsOverrider>();
            }

            if (baseOverrider)
            {
                SetupControls(baseOverrider, livery.prefab);
            }

            // If we have something that gets referenced through the simConnections decoupling mechanism - these are generally things
            // that make ports exist.
            var simConnections = livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true);

            if (!simConnections && (livery.prefab.GetComponentsInChildren<SimComponentDefinition>(true).Length > 0))
            {
                simConnections = AttachSimConnectionsToPrefab(livery.prefab);
            }

            var broadcastController = SetupBroadcastPortsIfNeeded(livery.prefab);

            SimController simController = livery.prefab.GetComponentInChildren<SimController>(true);

            // If we have something that can use a sim controller and don't already have a sim controller
            var needsSimController = (livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true) ||
                broadcastController != null ||
                livery.prefab.GetComponentsInChildren<ASimInitializedController>(true).Length > 0) &&
                !simController;
            if (needsSimController)
            {
                simController = livery.prefab.AddComponent<SimController>();
                simController.connectionsDefinition = simConnections;
            }

            if (simController)
            {
                SetupSimController(simController, livery.prefab);

                // For debug.
                var data = livery.prefab.GetComponentInChildren<SimDataDisplaySimController>(true);

                if (data)
                {
                    data.simController = simController;
                }
            }

            // In the event we have a sim controller and *not* a damage controller, we need to add a dummy damage controller
            var damageController = livery.prefab.GetComponentInChildren<DamageController>(true);
            if (simController && !damageController)
            {
                damageController = AttachDummyDamageController(livery.prefab);
            }

            // Add the windows breaking controller manually now.
            if (damageController)
            {
                damageController.windows = livery.prefab.GetComponentInChildren<WindowsBreakingController>(true);
            }

            if (livery.prefab.TryGetComponent<MultipleUnitModule>(out var mum))
            {
                mum.controlsOverrider = baseOverrider;
            }
        }

        private static DamageController AttachDummyDamageController(GameObject prefab)
        {
            var damageController = prefab.GetComponentInChildren<DamageController>(true);

            if (damageController == null)
            {
                damageController = prefab.AddComponent<DamageController>();
                damageController.windows = null;
                damageController.bodyDamagerPortIds = new string[0];
                damageController.bodyHealthStateExternalInPortIds = new string[0];
                damageController.mechanicalPTDamagerPortIds = new string[0];
                damageController.mechanicalPTPercentualDamagerPortIds = new string[0];
                damageController.mechanicalPTHealthStateExternalInPortIds = new string[0];
                damageController.mechanicalPTOffExternalInPortIds = new string[0];
                damageController.electricalPTDamagerPortIds = new string[0];
                damageController.electricalPTHealthStateExternalInPortIds = new string[0];
                damageController.electricalPTOffExternalInPortIds = new string[0];
            }

            return damageController;
        }

        private static void AddAdditionalControllers(GameObject prefab)
        {
            AddController<InteractablesKeyboardControl, AKeyboardInput>(prefab);
            AddController<InteractablePortFeedersController, InteractablePortFeeder>(prefab);
            AddController<InteractableFuseFeedersController, InteractableFuseFeeder>(prefab);
            AddController<IndicatorPortReadersController, IndicatorPortReader>(prefab);
            AddController<LampPortReadersController, LampPortReader>(prefab);
            AddController<LampFuseReadersController, LampFuseReader>(prefab);
            AddController<AnimatorPortReadersController, AnimatorPortReader>(prefab);
            AddController<GenericPortReadersController, AGenericPortReader>(prefab);
            AddController<PositionSyncProviderController, PositionSyncProvider>(prefab);
            AddController<PositionSyncConsumerController, PositionSyncConsumer>(prefab);
            AddController<OilingPointsPortController, OilingPointPortFeederReader>(prefab);
            AddController<ControlsBlockController, ControlBlocker>(prefab);

            AddController<CoupledAttachmentController, CoupledAttachmentTag>(prefab);
            AddController<RopeInitialiseController, RopeBehaviour>(prefab);

            // Add more wrapper controllers here - or possibly use MEF to initialize wrapper controllers?
        }

        private static void AddController<TController, TComp>(GameObject prefab)
            where TController : ARefreshableChildrenController<TComp>
            where TComp : MonoBehaviour
        {
            var entries = prefab.GetComponentsInChildren<TComp>();

            if (entries.Length > 0)
            {
                var controller = prefab.GetOrAddComponent<TController>();
                controller.entries = entries;
            }
        }

        private static SimConnectionDefinition AttachSimConnectionsToPrefab(GameObject prefab)
        {
            // SimConnectionDefinition is a structure that holds all of the magical port generating items
            var simConnections = prefab.AddComponent<SimConnectionDefinition>();

            simConnections.executionOrder = prefab.GetComponentsInChildren<SimComponentDefinition>();
            simConnections.connections = prefab.GetComponentsInChildren<Connection>();
            simConnections.portReferenceConnections = prefab.GetComponentsInChildren<PortReferenceConnection>();

            return simConnections;
        }

        private static BroadcastPortController? SetupBroadcastPortsIfNeeded(GameObject prefab)
        {
            var providers = prefab.GetComponentsInChildren<BroadcastPortValueProvider>(true);
            var consumers = prefab.GetComponentsInChildren<BroadcastPortValueConsumer>(true);

            if (providers.Length > 0 || consumers.Length > 0)
            {
                var controller = prefab.AddComponent<BroadcastPortController>();

                controller.providers = providers;
                controller.consumers = consumers;

                return controller;
            }

            return null;
        }

        private void SetupControls(BaseControlsOverrider controls, GameObject prefab)
        {
            controls.throttle = prefab.GetComponentInChildren<ThrottleControl>();
            controls.brake = prefab.GetComponentInChildren<BrakeControl>();
            controls.brakeCutout = prefab.GetComponentInChildren<BrakeCutoutControl>();
            controls.independentBrake = prefab.GetComponentInChildren<IndependentBrakeControl>();
            controls.dynamicBrake = prefab.GetComponentInChildren<DynamicBrakeControl>();
            controls.reverser = prefab.GetComponentInChildren<ReverserControl>();
            controls.sander = prefab.GetComponentInChildren<SanderControl>();
            controls.horn = prefab.GetComponentInChildren<HornControl>();
            controls.headlightsFront = prefab.GetComponentInChildren<HeadlightsControlFront>();
            controls.headlightsRear = prefab.GetComponentInChildren<HeadlightsControlRear>();
            controls.starter = prefab.GetComponentInChildren<StarterControl>();
            controls.powerOff = prefab.GetComponentInChildren<PowerOffControl>();
            controls.dynamo = prefab.GetComponentInChildren<DynamoControl>();
            controls.airPump = prefab.GetComponentInChildren<AirPumpControl>();
            controls.cabLight = prefab.GetComponentInChildren<CabLightControl>();
            controls.indCabLight = prefab.GetComponentInChildren<IndCabLightControl>();
            controls.wipers = prefab.GetComponentInChildren<WipersControl>();
            controls.engineOnReader = prefab.GetComponentInChildren<EngineOnReader>();
        }

        private void SetupSimController(SimController simController, GameObject prefab)
        {
            simController.poweredWheels = prefab.GetComponentInChildren<PoweredWheelsManager>();
            simController.controlsOverrider = prefab.GetComponentInChildren<BaseControlsOverrider>();
            simController.portsOverrider = prefab.GetComponentInChildren<BasePortsOverrider>();
            simController.broadcastPortController = prefab.GetComponentInChildren<BroadcastPortController>();
            simController.controlsBlocker = prefab.GetComponentInChildren<ControlsBlockController>();
            simController.headlightsController = prefab.GetComponentInChildren<HeadlightsMainController>();
            simController.cabLightsController = prefab.GetComponentInChildren<CabLightsController>();
            simController.wipersController = prefab.GetComponentInChildren<WipersSimControlInput>();
            simController.particlesController = prefab.GetComponentInChildren<ParticlesPortReadersController>();
            simController.tractionPortsFeeder = prefab.GetComponentInChildren<TractionPortsFeeder>();
            simController.drivingForce = prefab.GetComponentInChildren<DrivingForce>();
            simController.gearShiftingController = prefab.GetComponentInChildren<ManualGearShiftingController>();
            simController.wheelslipController = prefab.GetComponentInChildren<WheelslipController>();
            simController.compressor = prefab.GetComponentInChildren<CompressorSimController>();
            simController.coalPile = prefab.GetComponentInChildren<CoalPileSimController>();
            simController.firebox = prefab.GetComponentInChildren<FireboxSimController>();
            simController.remoteController = prefab.GetComponentInChildren<RemoteControllerModule>();
            simController.environmentDamageController = prefab.GetComponentInChildren<EnvironmentDamageController>();

            simController.OnValidate();
        }
    }
}
