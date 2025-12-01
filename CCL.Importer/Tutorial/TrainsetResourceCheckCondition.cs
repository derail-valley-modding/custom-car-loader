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
        private string _message;

        public TrainsetResourceAvailableCondition(TrainCar[] trainset, (ResourceContainerType Type, float Target)[] requirements, string message)
        {
            _controllers = trainset.Select(x => x.SimController?.resourceContainerController).ToArray();
            _requirements = requirements;
            _message = (string.IsNullOrEmpty(message) ? "ResourceAvailableCondition not fulfilled" : message);
        }

        public override string Check()
        {
            foreach (var (type, target) in _requirements)
            {
                foreach (var controller in _controllers)
                {
                    if (controller == null) continue;

                    var container = controller.GetResourceContainer(type);

                    if (container != null && container.normalizedReadOutPort.Value <= target) return _message;
                }
            }

            return string.Empty;
        }
    }
}
