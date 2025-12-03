using DV.Simulation.Cars;
using DV.Tutorial.QT;
using LocoSim.Resources;
using System.Linq;

namespace CCL.Importer.Tutorial
{
    internal class TrainsetResourceAvailableCondition : AQuickTutorialCondition
    {
        private ResourceContainerController?[] _controllers;
        private (ResourceContainerType, float)[] _requirements;

        public TrainsetResourceAvailableCondition(TrainCar[] trainset, (ResourceContainerType Type, float Target)[] requirements)
        {
            _controllers = trainset.Select(x => x.SimController?.resourceContainerController).ToArray();
            _requirements = requirements;
        }

        public override string Check()
        {
            foreach (var (type, target) in _requirements)
            {
                foreach (var controller in _controllers)
                {
                    if (controller == null) continue;

                    var container = controller.GetResourceContainer(type);

                    if (container != null && container.normalizedReadOutPort.Value <= target) return ResourceToMessage(container.resourceType);
                }
            }

            return string.Empty;
        }

        private static string ResourceToMessage(ResourceContainerType type) => type switch
        {
            ResourceContainerType.FUEL => "tutorial/cond/requires_fuel",
            ResourceContainerType.WATER or ResourceContainerType.COAL => "tutorial/cond/requires_coal_and_water",
            ResourceContainerType.ELECTRIC_CHARGE => "tutorial/cond/requires_power",
            _ => "ResourceAvailableCondition not fulfilled",
        };
    }
}
