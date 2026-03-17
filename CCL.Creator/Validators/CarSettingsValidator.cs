using CCL.Types;

namespace CCL.Creator.Validators
{
    internal class CarSettingsValidator : CarValidator
    {
        public override string TestName => "Car Settings";

        public override ValidationResult Validate(CustomCarType car)
        {
            var result = Pass();

            if (string.IsNullOrWhiteSpace(car.id))
            {
                result.CriticalFail("Car ID is empty!", car);
                return result;
            }

            if (car.liveries.Count == 0)
            {
                result.CriticalFail("Car has no liveries!", car);
                return result;
            }

            if (car.liveries.ContainsDuplicates(x => x.id))
            {
                result.CriticalFail("Car has duplicate livery IDs!", car);
                return result;
            }

            if (car.KindSelection != DVTrainCarKind.Car)
            {
                if (car.unusedCarDeletePreventionMode == CustomCarType.UnusedCarDeletePreventionMode.None)
                {
                    result.Warning("Car is not of generic car kind but has no delete prevention set", car);
                }

                if (car.carIdPrefix != "-")
                {
                    result.Warning("Car is not of generic car kind but ID prefix is not \"-\"", car);
                }
            }

            return result;
        }
    }
}
