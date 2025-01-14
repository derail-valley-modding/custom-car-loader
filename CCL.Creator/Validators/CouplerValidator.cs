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

            if (!livery.BufferType.IsDefined())
            {
                result.Fail($"{livery.id} - Buffer type not defined", livery);
            }

            var frontRig = livery.prefab!.transform.FindSafe(CarPartNames.Couplers.RIG_FRONT);
            if (!frontRig)
            {
                result.Fail("Missing front coupler rig " + CarPartNames.Couplers.RIG_FRONT, livery.prefab);
            }
            else
            {
                if (frontRig!.position.x != 0 || frontRig.position.y != 1.05f)
                {
                    result.Warning("Front coupler rig should be at x = 0, y = 1.05", frontRig);
                }

                if (livery.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.Buffers.FRONT_PADS)
                    {
                        var pad = frontRig.FindSafe(name);
                        if (!pad)
                        {
                            result.Warning("Missing buffer pad " + name, frontRig);
                        }
                    }
                }
            }

            var rearRig = livery.prefab.transform.FindSafe(CarPartNames.Couplers.RIG_REAR);
            if (!rearRig)
            {
                result.Fail("Missing rear coupler rig " + CarPartNames.Couplers.RIG_REAR, livery.prefab);
            }
            else
            {
                if (rearRig!.position.x != 0 || rearRig.position.y != 1.05f)
                {
                    result.Warning("Rear coupler rig should be at x = 0, y = 1.05", frontRig);
                }

                if (livery.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.Buffers.REAR_PADS)
                    {
                        var pad = rearRig.FindSafe(name);
                        if (!pad)
                        {
                            result.Warning("Missing buffer pad " + name, frontRig);
                        }
                    }
                }
            }

            return result;
        }
    }
}
