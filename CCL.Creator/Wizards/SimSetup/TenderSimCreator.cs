using CCL.Types.Proxies;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class TenderSimCreator : SimCreator
    {
        public TenderSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "S282B" };

        public override IEnumerable<string> GetSimFeatures(int basisIndex)
        {
            yield return "Water Storage";
            yield return "Coal Storage";
            yield return "Electric Connection To Front";
            yield return "Resource Connection To Front";
        }

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var coal = CreateResourceContainer(ResourceContainerType.Coal);
            CreateCoalPile(coal);
            CreateBroadcastProvider(coal, "NORMALIZED", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_COAL_NORMALIZED");
            CreateBroadcastProvider(coal, "CAPACITY", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_COAL_CAPACITY");

            var water = CreateResourceContainer(ResourceContainerType.Water);
            CreateBroadcastProvider(water, "NORMALIZED", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_NORMALIZED");
            CreateBroadcastProvider(water, "CAPACITY", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_CAPACITY");
            CreateBroadcastProvider(water, "AMOUNT", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_AMOUNT");
            CreateBroadcastConsumer(water, "CONSUME_EXT_IN", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_CONSUME", 0, true);

            var lightsF = CreateOverridableControl(OverridableControlType.HeadlightsFront);
            CreateBroadcastConsumer(lightsF, "EXT_IN", DVPortForwardConnectionType.COUPLED_FRONT, "HEADLIGHTS_FRONT", 0.4f, false);
            var lightsR = CreateOverridableControl(OverridableControlType.HeadlightsRear);
            CreateBroadcastConsumer(lightsR, "EXT_IN", DVPortForwardConnectionType.COUPLED_FRONT, "HEADLIGHTS_REAR", 0.4f, false);

            var dynamo = CreateSimComponent<ConfigurablePortDefinitionProxy>("dynamoFlowDummy");
            dynamo.port.ID = "DYNAMO_FLOW_NORMALIZED";
            dynamo.port.type = DVPortType.READONLY_OUT;
            dynamo.port.valueType = DVPortValueType.STATE;
            CreateBroadcastConsumer(dynamo, dynamo.port.ID, DVPortForwardConnectionType.COUPLED_FRONT, "DYNAMO_FLOW", 0, false);

            var fuseController = CreateSimComponent<FuseControllerDefinitionProxy>("electronicsFuseControllerDummy");

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", true)
            };

            fuseController.fuseId = FullFuseId(fusebox, 0);

            _baseControls.propagateNeutralStateToFront = true;

            ConnectPortRef(dynamo, dynamo.port.ID, fuseController, fuseController.controllingPort.ID);

            ApplyMethodToAll<IS282Defaults>(s => s.ApplyS282Defaults());
        }
    }
}
