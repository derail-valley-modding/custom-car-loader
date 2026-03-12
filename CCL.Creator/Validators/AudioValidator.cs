using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies.Audio;
using CCL.Types.Proxies.Ports;
using System.Linq;
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

            CheckPorts(prefab, result);
            CheckCylCocks(prefab, result);
            CheckChuffs(prefab, result);

            return result;
        }

        private static void CheckPorts(GameObject prefab, ValidationResult result)
        {
            foreach (var comp in prefab.GetComponentsInChildren<IHasPortIdFields>(true))
            {
                foreach (var port in comp.ExposedPortIdFields)
                {
                    if (!port.IsAssigned)
                    {
                        if (!port.IsMultiValue && port.Required)
                        {
                            result.Fail($"Port field {port.FullName} must be assigned", comp is Object obj ? obj : null, port.FieldName);
                        }
                    }
                }
            }
        }

        private static void CheckCylCocks(GameObject prefab, ValidationResult result)
        {
            foreach (var comp in prefab.GetComponentsInChildren<CylinderCockLayeredPortReaderProxy>(true))
            {
                foreach (var audio in comp.cylCockAudio)
                {
                    if (audio == null)
                    {
                        result.Fail($"{comp.name}/{nameof(CylinderCockLayeredPortReaderProxy)}: null entries in audio array",
                            comp, nameof(comp.cylCockAudio));
                        return;
                    }

                    if (!ComponentUtil.HasComponent<LayeredAudioProxy>(audio) &&
                        !ComponentUtil.HasComponent<CopyVanillaAudioSystem>(audio))
                    {
                        result.Warning($"{audio.name} does not have a {nameof(LayeredAudioProxy)} " +
                            $"or {nameof(CopyVanillaAudioSystem)} for {nameof(CylinderCockLayeredPortReaderProxy)}, " +
                            $"make sure this is intentional", audio, nameof(comp.cylCockAudio));
                    }
                }
            }
        }

        private static void CheckChuffs(GameObject prefab, ValidationResult result)
        {
            foreach (var comp in prefab.GetComponentsInChildren<ChuffClipsSimReaderProxy>(true))
            {
                // Individual chuffs.
                CheckArray(comp.lowPressureClips, "low pressure clips", nameof(comp.lowPressureClips));
                CheckArray(comp.mediumPressureClips, "medium pressure clips", nameof(comp.mediumPressureClips));
                CheckArray(comp.highPressureClips, "high pressure clips", nameof(comp.highPressureClips));
                CheckIndividualConfig(comp.regularChuffConfig, "chuff", nameof(comp.regularChuffConfig));

                // Water chuffs.
                //CheckArray(comp.waterChuffClips, "water chuff clips");
                CheckIndividualConfig(comp.waterChuffConfig, "water chuff", nameof(comp.waterChuffConfig));

                // Ash chuffs.
                //CheckArray(comp.ashChuffClips, "ash chuff clips");
                CheckIndividualConfig(comp.ashChuffConfig, "ash chuff", nameof(comp.ashChuffConfig));

                // Loops.
                CheckArray(comp.chuffLoops, "chuff loops", nameof(comp.chuffLoops));
                CheckArray(comp.waterChuffLoops, "water chuff loops", nameof(comp.waterChuffLoops));
                CheckArray(comp.ashChuffLoops, "ash chuff loops", nameof(comp.ashChuffLoops));

                void CheckArray<T>(T[] array, string name, string? highlight)
                {
                    if (array.Any(x => x == null))
                    {
                        result.Fail($"{comp.name}/{nameof(ChuffClipsSimReaderProxy)}: null entries in {name} array", comp, highlight);
                    }
                }

                void CheckIndividualConfig(IndividualChuffAudioSourceConfig config, string name, string? highlight)
                {
                    if (config == null)
                    {
                        result.Fail($"{comp.name}/{nameof(ChuffClipsSimReaderProxy)}: {name} config is null", comp, highlight);
                        return;
                    }

                    if (config.parent == null)
                    {
                        result.Fail($"{config.name}/{nameof(IndividualChuffAudioSourceConfig)}: parent is null", config);
                    }
                }
            }
        }
    }
}
