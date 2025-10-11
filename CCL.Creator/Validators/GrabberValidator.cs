using CCL.Types;
using CCL.Types.Components;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class GrabberValidator : LiveryValidator
    {
        public override string TestName => "Grabbers";

        public override ValidationResult Validate(CustomCarType car)
        {
            var result = base.Validate(car);

            foreach (var grabber in car.AllPrefabs.GetComponentsInChildren<IVanillaResourceGrabber>(true))
            {
                CheckGenericGrabber(grabber, result);
            }

            foreach (var grabber in car.AllPrefabs.GetComponentsInChildren<ICustomGrabberValidation>(true))
            {
                CheckMaterialGrabberRenderer(grabber, result);
            }

            return result;
        }

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            foreach (var grabber in livery.AllPrefabs.GetComponentsInChildren<IVanillaResourceGrabber>(true))
            {
                CheckGenericGrabber(grabber, result);
            }

            foreach (var grabber in livery.AllPrefabs.GetComponentsInChildren<ICustomGrabberValidation>(true))
            {
                CheckMaterialGrabberRenderer(grabber, result);
            }

            return result;
        }

        private void CheckGenericGrabber(IVanillaResourceGrabber grabber, ValidationResult result)
        {
            foreach (var item in grabber.GetReplacements())
            {
                if (grabber.GetScript() == null)
                {
                    result.Fail($"{grabber.GetType().Name} in {grabber.gameObject.GetPath()} does not have a valid script.",
                        grabber is Object obj ? obj : null);
                }

                if (!grabber.GetNames().Contains(item.ReplacementName))
                {
                    result.Fail($"{grabber.GetType().Name} in {grabber.gameObject.GetPath()} does not have a valid replacement ({item.ReplacementName}).",
                        grabber is Object obj ? obj : null);
                }
            }
        }

        private void CheckMaterialGrabberRenderer(ICustomGrabberValidation grabber, ValidationResult result)
        {
            if (!grabber.IsValid(out var error))
            {
                result.Fail(error, grabber is Object obj ? obj : null);
            }
        }
    }
}
