using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Simulation.Steam;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class ComponentValidator : LiveryValidator
    {
        public override string TestName => "Components";

        public override ValidationResult Validate(CustomCarType car)
        {
            var result = base.Validate(car);

            if (result.Status == ResultStatus.Critical) return result;

            var tested = result.Status != ResultStatus.Skipped;
            int count = 0;

            foreach (var prefab in car.AllPrefabs)
            {
                count += CheckPrefab(prefab, car.id, result);

                if (result.Status == ResultStatus.Critical) return result;
            }

            return (tested || count > 0) ? result : Skip();
        }

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            int count = 0;
            var result = Pass();

            foreach (var prefab in livery.AllPrefabs)
            {
                count += CheckPrefab(prefab, livery.id, result);

                if (result.Status == ResultStatus.Critical) return result;
            }

            count += TestLubricatorRatchet(livery, result);

            return count == 0 ? Skip() : result;
        }

        private static int CheckPrefab(GameObject prefab, string id, ValidationResult result)
        {
            var components = prefab.GetComponentsInChildren<ISelfValidation>();
            var count = components.Length;

            foreach (var comp in components)
            {
                if (!(comp is Component self)) continue;

                switch (comp.Validate(out var message, out var highlight))
                {
                    case SelfValidationResult.Info:
                        result.AddInformation($"{id} - {message}");
                        continue;
                    case SelfValidationResult.Warning:
                        result.Warning(AddCompToMessage(self, message), self, highlight);
                        continue;
                    case SelfValidationResult.Fail:
                        result.Fail(AddCompToMessage(self, message), self, highlight);
                        continue;
                    case SelfValidationResult.Critical:
                        result.CriticalFail(AddCompToMessage(self, message), self, highlight);
                        return count;
                    case SelfValidationResult.Pass:
                        continue;
                    default:
                        Debug.LogWarning($"Unknown self validation result from {self.name}/{self.GetType().Name}");
                        continue;
                }
            }

            var editorComps = prefab.GetComponentsInChildren<IEditorComponent>();
            count += editorComps.Length;

            foreach (var comp in editorComps)
            {
                if (!(comp is Component self)) continue;

                result.Fail(AddCompToMessage(self, "editor-only component must be removed before exporting"), self);
            }

            count += TestLODs(prefab, result);

            return count;

            static string AddCompToMessage(Component comp, string message)
            {
                return $"{comp.name}/{comp.GetType().Name}: {message}";
            }
        }

        private static int TestLODs(GameObject prefab, ValidationResult result)
        {
            var dict = new Dictionary<LODGroup, HashSet<Renderer>>();
            int count = 0;

            foreach (var group in prefab.GetComponentsInChildren<LODGroup>(true))
            {
                var set = new HashSet<Renderer>();

                foreach (var lod in group.GetLODs())
                {
                    set.UnionWith(lod.renderers);
                }

                foreach (var renderer in set)
                {
                    var groups = new HashSet<LODGroup> { group };

                    foreach (var pair in dict)
                    {
                        if (pair.Value.Contains(renderer))
                        {
                            groups.Add(pair.Key);
                        }
                    }

                    if (groups.Count > 1)
                    {
                        result.Fail($"Renderer {renderer.name} is in multiple LOD Groups: {string.Join(", ", groups.Select(x => x.name))}", renderer);
                    }
                }

                dict.Add(group, set);
                count++;
            }

            return count;
        }

        private static int TestLubricatorRatchet(CustomCarVariant livery, ValidationResult result)
        {
            var comps = livery.AllPrefabs.GetComponentsInChildren<LubricatorRatchetProxy>();

            if (comps.Count > 0 && !ComponentUtil.HasComponentInChildren<LubricatorRatchetDriverProxy>(livery.prefab!))
            {
                result.Fail($"Could not find any {nameof(LubricatorRatchetDriverProxy)}", livery.prefab);
            }

            return comps.Count;
        }
    }
}
