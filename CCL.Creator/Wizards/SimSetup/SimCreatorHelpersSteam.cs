using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Resources;

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
    }
}
