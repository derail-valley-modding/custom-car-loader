using DV.CabControls.Spec;
using DV.Damage;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(ExternalInteractableProcessor))]
    [RequiresStep(typeof(ProxyScriptProcessor))]
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
            }
            if (livery.externalInteractablesPrefab)
            {
                AddAdditionalControllers(livery.externalInteractablesPrefab);
            }

            // If we have something that gets referenced through the simConnections decoupling mechanism - these are generally things
            // that make ports exist.
            var simConnections = livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true);

            if (!simConnections && (livery.prefab.GetComponentsInChildren<SimComponentDefinition>(true).Length > 0))
            {
                AttachSimConnectionsToPrefab(livery.prefab);
            }

            // If we have something that can use a sim controller and don't already have a sim controller
            var needsSimController = livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true) ||
                livery.prefab.GetComponentsInChildren<ASimInitializedController>(true).Length > 0 &&
                !livery.prefab.GetComponentInChildren<SimController>(true);
            if (needsSimController)
            {
                var simController = livery.prefab.AddComponent<SimController>();
                simController.connectionsDefinition = livery.prefab.GetComponentInChildren<SimConnectionDefinition>(true) ?? AttachSimConnectionsToPrefab(livery.prefab);
                simController.otherSimControllers = livery.prefab.GetComponentsInChildren<ASimInitializedController>(true);
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
                damageController.mechanicalPTHealthStateExternalInPortIds = new string[0];
                damageController.mechanicalPTOffExternalInPortIds = new string[0];
                damageController.electricalPTDamagerPortIds = new string[0];
                damageController.electricalPTHealthStateExternalInPortIds = new string[0];
                damageController.electricalPTOffExternalInPortIds = new string[0];
            }
        }

        private static void AddAdditionalControllers(GameObject prefab)
        {
            if (prefab.GetComponentsInChildren<InteractablePortFeeder>().Length > 0)
            {
                var controller = prefab.AddComponent<InteractablePortFeedersController>();
                controller.entries = prefab.GetComponentsInChildren<InteractablePortFeeder>();
            }

            if (prefab.GetComponentsInChildren<IndicatorPortReader>().Length > 0)
            {
                var controller = prefab.AddComponent<IndicatorPortReadersController>();
                controller.entries = prefab.GetComponentsInChildren<IndicatorPortReader>();
            }
            // Add more wrapper controllers here - or possibly use MEF to initialize wrapper controllers?
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

    }
}
