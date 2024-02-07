using CCL.Types;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class CouplerValidator : LiveryValidator
    {
        public override string TestName => "Couplers & Buffers";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            var frontRig = livery.prefab!.transform.FindSafe(CarPartNames.COUPLER_RIG_FRONT);
            if (!frontRig)
            {
                result.Fail("Missing front coupler rig " + CarPartNames.COUPLER_RIG_FRONT);
            }
            else
            {
                if (frontRig!.position.x != 0 || frontRig.position.y != 1.05f)
                {
                    result.Warning("Front coupler rig should be at x = 0, y = 1.05");
                }

                if (livery.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.BUFFER_FRONT_PADS)
                    {
                        var pad = frontRig.FindSafe(name);
                        if (!pad)
                        {
                            result.Warning("Missing buffer pad " + name);
                        }
                    }
                }
            }

            var rearRig = livery.prefab.transform.FindSafe(CarPartNames.COUPLER_RIG_REAR);
            if (!rearRig)
            {
                result.Fail("Missing rear coupler rig " + CarPartNames.COUPLER_RIG_REAR);
            }
            else
            {
                if (rearRig!.position.x != 0 || rearRig.position.y != 1.05f)
                {
                    result.Warning("Rear coupler rig should be at x = 0, y = 1.05");
                }

                if (livery.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.BUFFER_REAR_PADS)
                    {
                        var pad = rearRig.FindSafe(name);
                        if (!pad)
                        {
                            result.Warning("Missing buffer pad " + name);
                        }
                    }
                }
            }

            return result;
        }
    }
}
