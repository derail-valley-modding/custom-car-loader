using DV.Tutorial.QT;

namespace CCL.Importer.Tutorial
{
    internal class TrainsetCompleteCondition : AQuickTutorialCondition
    {
        private TrainCar _car;
        private string _message;

        public TrainsetCompleteCondition(TrainCar car, string message = null!)
        {
            _car = car;
            _message = string.IsNullOrEmpty(message) ? "Trainset is not complete" : message;
        }

        public override string Check()
        {
            return CarManager.TryGetInstancedTrainset(_car, out _) switch
            {
                CarManager.TrainSetCompleteness.NotCCL => string.Empty,
                CarManager.TrainSetCompleteness.NotPartOfTrainset => string.Empty,
                CarManager.TrainSetCompleteness.NotComplete => _message,
                CarManager.TrainSetCompleteness.Complete => string.Empty,
                _ => string.Empty,
            };
        }
    }
}
