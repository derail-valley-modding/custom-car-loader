using DV.Damage;
using DV.HUD;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Fuses;
using DV.Simulation.Ports;
using DV.Util;
using LocoSim.Definitions;
using System.ComponentModel.Composition;
using UnityEngine;

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
            if (livery.externalInteractablesPrefab)
            {
                AddAdditionalControllers(livery.externalInteractablesPrefab);
            }

            // Add Control Override components
            var baseOverrider = livery.prefab.GetComponentInChildren<BaseControlsOverrider>(true);
            if ((livery.prefab.GetComponentsInChildren<OverridableBaseControl>().Length > 0) && !baseOverrider)
            {
                baseOverrider = livery.prefab.AddComponent<BaseControlsOverrider>();
            }
            baseOverrider?.OnValidate();

            // If we have something that gets referenced through the simConnections decoupling mechanism - these are generally things
            // that make ports exist.
            var simConnections = livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true);

            if (!simConnections && (livery.prefab.GetComponentsInChildren<SimComponentDefinition>(true).Length > 0))
            {
                AttachSimConnectionsToPrefab(livery.prefab);
            }

            var broadcastController = SetupBroadcastPortsIfNeeded(livery.prefab);

            // If we have something that can use a sim controller and don't already have a sim controller
            var needsSimController = (livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true) ||
                broadcastController != null ||
                livery.prefab.GetComponentsInChildren<ASimInitializedController>(true).Length > 0) &&
                !livery.prefab.GetComponentInChildren<SimController>(true);
            if (needsSimController)
            {
                var simController = livery.prefab.AddComponent<SimController>();
                simController.OnValidate();
            }

            // In the event we have a sim controller and *not* a damage controller, we need to add a dummy damage controller
            var needsDamageController = livery.prefab.GetComponentInChildren<SimController>(true) &&
                !livery.prefab.GetComponentInChildren<DamageController>(true);
            if (needsDamageController)
            {
                AttachDummyDamageController(livery.prefab);
            }
        }

        private static void AttachDummyDamageController(GameObject prefab)
        {
            if (!prefab.GetComponentInChildren<DamageController>())
            {
                var damageController = prefab.AddComponent<DamageController>();
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
        }

        private static void AddAdditionalControllers(GameObject prefab)
        {
            AddController<InteractablePortFeedersController, InteractablePortFeeder>(prefab);
            AddController<IndicatorPortReadersController, IndicatorPortReader>(prefab);
            AddController<InteractableFuseFeedersController, InteractableFuseFeeder>(prefab);
            AddController<AnimatorPortReadersController, AnimatorPortReader>(prefab);
            AddController<GenericPortReadersController, AGenericPortReader>(prefab);
            AddController<PositionSyncProviderController, PositionSyncProvider>(prefab);
            AddController<PositionSyncConsumerController, PositionSyncConsumer>(prefab);
            AddController<OilingPointsPortController, OilingPointPortFeederReader>(prefab);

            // Add more wrapper controllers here - or possibly use MEF to initialize wrapper controllers?
        }

        private static void AddController<TController, TComp>(GameObject prefab)
            where TController : ARefreshableChildrenController<TComp>
            where TComp : MonoBehaviour
        {
            var entries = prefab.GetComponentsInChildren<TComp>();

            if (entries.Length > 0)
            {
                var controller = prefab.AddComponent<TController>();
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
    }
}
