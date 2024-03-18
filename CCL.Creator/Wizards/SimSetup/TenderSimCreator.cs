﻿using CCL.Types.Proxies;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class TenderSimCreator : SimCreator
    {
        public TenderSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "S282B" };

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

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", true)
            };

            _baseControls.propagateNeutralStateToFront = true;

            ApplyMethodToAll<IS282Defaults>(s => s.ApplyS282Defaults());
        }
    }
}
