using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation.Steam;

namespace CCL.Creator.Wizards.SimSetup
{
    internal abstract partial class SimCreator
    {
        protected CoalPileSimControllerProxy CreateCoalPile(ResourceContainerProxy coal)
        {
            var pile = CreateSibling<CoalPileSimControllerProxy>(coal);
            pile.coalAvailablePortId = FullPortId(coal, "AMOUNT");
            pile.coalCapacityPortId = FullPortId(coal, "CAPACITY");
            pile.coalConsumePortId = FullPortId(coal, "CONSUME_EXT_IN");
            return pile;
        }

        protected void FillPortOverriderSteamer(BasePortsOverriderProxy overrider, BoilerDefinitionProxy? boiler,
            ManualOilingPointsDefinitionProxy? oilingPoints, MechanicalLubricatorDefinitionProxy? lubricator)
        {
            if (boiler != null)
            {
                overrider.boilerSpecialRequestPortId = FullPortId(boiler, "WATER_CHANGE_REQUESTED_EXT_IN");
            }

            if (oilingPoints != null)
            {
                overrider.oilingPointsSpecialRequestPortId = FullPortId(oilingPoints, "SPECIAL_REQUEST");
            }

            if (lubricator != null)
            {
                overrider.lubricatorSpecialRequestPortId = FullPortId(lubricator, "SPECIAL_REQUEST");
            }
        }
    }
}
