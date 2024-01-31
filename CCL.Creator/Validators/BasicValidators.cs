using CCL.Types;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal class LiverySettingsValidator : LiveryValidator
    {
        public override string TestName => "Car Setup";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (!livery.prefab)
            {
                return CriticalFail("Livery must have a prefab assigned!");
            }
            if (livery.BaseCarType == BaseTrainCarType.NotSet)
            {
                return Fail("Base car type must be set");
            }
            return Pass();
        }
    }

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class RootTransformValidator : LiveryValidator
    {
        public override string TestName => "Root Transform";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (livery.prefab!.transform.position != Vector3.zero)
            {
                return Fail($"{livery.id} - Not at (0,0,0)");
            }
            if (livery.prefab.transform.eulerAngles != Vector3.zero)
            {
                return Fail($"{livery.id} - Non-zero rotation");
            }
            if (livery.prefab.transform.localScale != Vector3.one)
            {
                return Fail($"{livery.id} - Scale is not 1");
            }
            return Pass();
        }
    }

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class LODGroupValidator : LiveryValidator
    {
        public override string TestName => "LOD Group";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();
            var lodGroup = livery.prefab!.GetComponent<LODGroup>();
            if (lodGroup)
            {
                foreach (var lod in lodGroup.GetLODs())
                {
                    if (lod.renderers.Length == 0)
                    {
                        result.Warning("Missing renderers on LOD");
                    }
                }
            }
            return result;
        }
    }

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class InteriorTransformValidator : LiveryValidator
    {
        public override string TestName => "Interior";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            if (livery.interiorPrefab)
            {
                if (livery.interiorPrefab!.transform.position != Vector3.zero)
                {
                    result.Warning("Interior is not centered at (0,0,0)");
                }
            }

            return result;
        }
    }
}
