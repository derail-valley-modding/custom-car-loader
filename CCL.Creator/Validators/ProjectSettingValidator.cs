using CCL.Types;
using System.Linq;
using UnityEditor;

namespace CCL.Creator.Validators
{
    internal class ProjectSettingValidator : CarValidator
    {
        public override string TestName => "Project Settings";

        public override ValidationResult Validate(CustomCarType _)
        {
            // Obsolete VR settings.
#pragma warning disable 0618

            var result = Pass();

            if (!PlayerSettings.virtualRealitySupported)
            {
                result.Warning("VR support isn't enabled");
            }

            string[] sdks = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone);
            if (!sdks.Contains("Oculus"))
            {
                result.Warning("Oculus support isn't enabled");
            }
            if (!sdks.Contains("OpenVR"))
            {
                result.Warning("OpenVR support isn't enabled");
            }

            if (!PlayerSettings.singlePassStereoRendering)
            {
                result.Warning("VR Stereo Rendering Mode isn't set to Single Pass");
            }

            return result;

#pragma warning restore 0618
        }
    }
}
