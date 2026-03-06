using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies.Audio;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(CarSettingsValidator))]
    internal class AudioValidator : CarValidator
    {
        public override string TestName => "Audio";

        public override ValidationResult Validate(CustomCarType car)
        {
            if (car.SimAudioPrefab == null) return Skip();

            var result = Pass();
            var prefab = car.SimAudioPrefab;

            CheckCylCocks(prefab, result);

            return result;
        }

        private void CheckCylCocks(GameObject prefab, ValidationResult result)
        {
            foreach (var comp in prefab.GetComponentsInChildren<CylinderCockLayeredPortReaderProxy>(true))
            {
                foreach (var audio in comp.cylCockAudio)
                {
                    if (audio == null)
                    {
                        result.Fail($"{comp.name}: null entries in audio array", comp);
                        return;
                    }

                    if (!ComponentUtil.HasComponent<LayeredAudioProxy>(audio, false) &&
                        !ComponentUtil.HasComponent<CopyVanillaAudioSystem>(audio, false))
                    {
                        result.Warning($"{audio.name} does not have a {nameof(LayeredAudioProxy)} " +
                            $"or {nameof(CopyVanillaAudioSystem)} for {nameof(CylinderCockLayeredPortReaderProxy)}, " +
                            $"make sure this is intentional", audio);
                    }
                }
            }
        }
    }
}
